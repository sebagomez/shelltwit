using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using shelltwitlib.Web;
using shelltwitlib.Helpers;
using shelltwitlib.API.OAuth;
using System.Linq;
using System.Text.RegularExpressions;

namespace shelltwitlib.API.Tweets
{
	public class Update
	{
		const string UPDATE_STATUS = "https://api.twitter.com/1.1/statuses/update.json";
		const string UPDATE_MEDIA_STATUS = "https://api.twitter.com/1.1/statuses/update_with_media.json";
		const string STATUS = "status";
		const string MEDIA = "media[]";

		#region Update Status

		public static string UpdateStatus(string status)
		{
			return UpdateStatus(status, null, null);
		}

		public static string UpdateStatus(string status, TwUser user, string replyId)
		{
			string filePath = null;
			string encodedPath = null;
			Regex regex = new Regex(@"\[(.*)\]", RegexOptions.Singleline);
			if (regex.Matches(status).Count > 1)
				throw new Exception("Only one file can be uploaded per update");

			Match match = regex.Match(status);
			if (match != null && !string.IsNullOrEmpty(match.Value))
			{
				status = status.Replace(match.Value, "");
				filePath = match.Value.Replace("[", "").Replace("]", "");
				if (!File.Exists(filePath))
					throw new FileNotFoundException("Media file not found", filePath);
				encodedPath = Util.EncodeString(filePath);
			}

			if (status.Length > 140)
				throw new Exception(string.Format("Status must be at most 140 characters long. Length:{0}", status.Length));

			if (user == null)
				user = TwUser.LoadCredentials();
			string encodedStatus = Util.EncodeString(status);
			
			HttpWebRequest req;
			byte[] bytes;
			if (string.IsNullOrEmpty(filePath))
			{
				req = GetUpdateStatusRequest(user.OAuthToken, user.OAuthTokenSecret, encodedStatus, replyId);
				bytes = Util.GetUTF8EncodingBytes(string.Format("{0}={1}", STATUS, encodedStatus));

				req.ContentLength = bytes.Length;
				using (Stream str = req.GetRequestStream())
					str.Write(bytes, 0, bytes.Length);
			}
			else
			{
				req = GetUpdateMediaStatusRequest(user.OAuthToken, user.OAuthTokenSecret);

				string boundary = GetMultipartBoundary(),
					separator = "--" + boundary,
					footer = "\r\n" + separator + "--\r\n",
					shortFileName = Path.GetFileName(filePath),
					fileContentType = GetMimeType(shortFileName),
					fileHeader = string.Format("Content-Disposition: file; " +
											   "name=\"media[]\"; filename=\"{0}\"",
											   shortFileName);

				var contents = new System.Text.StringBuilder();
				contents.AppendLine(separator)
					.AppendLine("Content-Disposition: form-data; name=\"status\"")
					.AppendLine();
				contents.AppendLine(status);
				contents.AppendLine(separator);
				contents.AppendLine(fileHeader);
				contents.AppendLine(string.Format("Content-Type: {0}", fileContentType));
				contents.AppendLine();

				req.ContentType = "multipart/form-data; boundary=" + boundary;

				var encoding = System.Text.Encoding.UTF8;
				using (var s = req.GetRequestStream())
				{
					bytes = encoding.GetBytes(contents.ToString());
					s.Write(bytes, 0, bytes.Length);
					bytes = File.ReadAllBytes(filePath);
					s.Write(bytes, 0, bytes.Length);
					bytes = encoding.GetBytes(footer);
					s.Write(bytes, 0, bytes.Length);
				}

			}

			req.PreAuthenticate = true;
			req.AllowWriteStreamBuffering = true;

			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			return response.StatusDescription;
		}

		private static string GetMultipartBoundary()
		{
			return "======" +
				Guid.NewGuid().ToString().Substring(18).Replace("-", "") +
				"======";
		}

		private static string GetMimeType(String filename)
		{
			var extension = System.IO.Path.GetExtension(filename).ToLower();
			var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);

			string result =
				((regKey != null) && (regKey.GetValue("Content Type") != null))
				? regKey.GetValue("Content Type").ToString()
				: "image/unknown";
			return result;
		}

		static HttpWebRequest GetUpdateStatusRequest(string oAuthToken, string oAuthSecret, string encodedStatus, string replyId )
		{
			string url = UPDATE_STATUS;
			if (!string.IsNullOrEmpty(replyId))
				url += "?in_reply_to_status_id=" + replyId;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = HttpMethod.POST.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetUpdateStatusParms(nonce, timestamp, oAuthToken, encodedStatus, replyId);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, UPDATE_STATUS, parms);
			string signature = OAuthHelper.SignBaseString(signatureBase, oAuthSecret);
			string authHeader = OAuthHelper.AuthorizationHeader(nonce, signature, timestamp, oAuthToken, true);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		static Dictionary<string, string> GetUpdateStatusParms(string nonce, string timestamp, string oAuthToken, string status, string replyId)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("oauth_callback", "oob");
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(OAuthHelper.CONSUMER_KEY));
			dic.Add(OAuthHelper.OAUTH_NONCE, nonce);
			dic.Add(OAuthHelper.OAUTH_SIGNATURE_METHOD, OAuthHelper.HMAC_SHA1);
			dic.Add(OAuthHelper.OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAuthHelper.OAUTH_VERSION, OAuthHelper.OAUTH_VERSION_10);
			dic.Add(OAuthHelper.OAUTH_TOKEN, oAuthToken);
			if (!string.IsNullOrEmpty(status))
				dic.Add(STATUS, status);
			if (!string.IsNullOrEmpty(replyId))
				dic.Add("in_reply_to_status_id", replyId);

			return dic;
		}

		static HttpWebRequest GetUpdateMediaStatusRequest(string oAuthToken, string oAuthSecret)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UPDATE_MEDIA_STATUS);
			request.Method = HttpMethod.POST.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetUpdateStatusParms(nonce, timestamp, oAuthToken, null, null);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, UPDATE_MEDIA_STATUS, parms);
			string signature = OAuthHelper.SignBaseString(signatureBase, oAuthSecret);
			string authHeader = OAuthHelper.AuthorizationHeader(nonce, signature, timestamp, oAuthToken, true);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.FORM_DATA;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		#endregion
	}
}
