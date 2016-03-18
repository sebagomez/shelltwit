using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using shelltwitlib.API.OAuth;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Update
	{
		const string UPDATE_STATUS = "https://api.twitter.com/1.1/statuses/update.json";
		const string UPDATE_MEDIA_STATUS = "https://api.twitter.com/1.1/statuses/update_with_media.json";
		const string MEDIA_UPLOAD = "https://upload.twitter.com/1.1/media/upload.json";
		const string STATUS = "status";
		const string MEDIA = "media_ids";

		static Action<string> s_messageFunction;

		#region Update Status

		public static string UpdateStatus(string status)
		{
			return UpdateStatus(status, null, null);
		}

		public static string UpdateStatus(string status, TwUser user)
		{
			return UpdateStatus(status, user, null);
		}

		public static void SetMessageAction(Action<string> func)
		{
			s_messageFunction = func;
		}

		static void WriteMessage(string message)
		{
			if (s_messageFunction != null)
				s_messageFunction(message);
		}

		public static string UpdateStatus(string status, TwUser user, string replyId)
		{
			Regex regex = new Regex(@"\[(.*?)\]");
			List<FileInfo> media = new List<FileInfo>();
			foreach (System.Text.RegularExpressions.Match match in regex.Matches(status))
			{
				status = status.Replace(match.Value, "");
				FileInfo file = new FileInfo(match.Value.Replace("[", "").Replace("]", ""));
				if (!file.Exists)
					throw new FileNotFoundException("File not found", file.FullName);
				media.Add(file);
            }

			if (media.Count > 4) //limited by the twitter API
				throw new ArgumentOutOfRangeException("media", "Up to 4 media files are allowed per tweet");

			if (user == null)
				user = TwUser.LoadCredentials();
			string encodedStatus = Util.EncodeString(status);
			
			if (media.Count == 0)
				return InternalUpdateStatus(user, encodedStatus, replyId);
			else
				return InternalUpdateWithMedia(user, encodedStatus, replyId, media);
		}

		private static string InternalUpdateStatus(TwUser user, string encodedStatus, string replyId, List<string> mediaIds = null)
		{
			string media = null;
			if (mediaIds != null)
				media = Util.EncodeString( string.Join(",", mediaIds.ToArray()));

			HttpWebRequest req = GetUpdateStatusRequest(user.OAuthToken, user.OAuthTokenSecret, encodedStatus, replyId, media);

			byte[] bytes;
			using (Stream str = req.GetRequestStream())
			{
				bytes = Util.GetUTF8EncodingBytes(string.Format("{0}={1}", STATUS, encodedStatus));
				str.Write(bytes, 0, bytes.Length);

				if (!string.IsNullOrEmpty(media))
				{
					bytes = Util.GetUTF8EncodingBytes(string.Format("&{0}={1}", MEDIA, media));
					str.Write(bytes, 0, bytes.Length);
				}
			}

			req.PreAuthenticate = true;
			req.AllowWriteStreamBuffering = true;

			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			return response.StatusDescription;
		}

		private static string InternalUpdateWithMedia(TwUser user, string encodedStatus, string replyId, List<FileInfo> media)
		{
			return InternalUpdateStatus(user, encodedStatus, replyId, UploadMedia(user, media));
		}

		private static List<string> UploadMedia(TwUser user, List<FileInfo> mediaFiles)
		{
			List<string> ids = new List<string>();

			foreach (FileInfo file in mediaFiles)
			{
				WriteMessage($"Uploading {file.Name}");
				HttpWebRequest req = GetMediaUploadStatusRequest(user.OAuthToken, user.OAuthTokenSecret);

				string boundary = GetMultipartBoundary(),
					separator = "--" + boundary,
					footer = separator + "--",
					shortFileName = file.Name,
					fileContentType = GetMimeType(shortFileName),
					fileHeader = string.Format("Content-Disposition: form-data; name=\"media\"; filename=\"{0}\"", shortFileName),
					contentType = "Content-Type: application/octet-stream";

				var encoding = System.Text.Encoding.UTF8;

				req.KeepAlive = false;
				req.ContentType = string.Format("multipart/form-data, boundary=\"{0}\"", boundary);

				string newLine = "\r\n";
				byte[] bytes;
				using (var s = req.GetRequestStream())
				{
					bytes = encoding.GetBytes(separator);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(fileHeader);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(contentType);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = File.ReadAllBytes(file.FullName);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(footer);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

				}

				req.PreAuthenticate = true;
				req.AllowWriteStreamBuffering = true;

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();

				Media media = Media.FromStream(response.GetResponseStream());

				ids.Add(media.IdString);
			}

			return ids;
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

		static HttpWebRequest GetUpdateStatusRequest(string oAuthToken, string oAuthSecret, string encodedStatus, string replyId, string media = null)
		{
			string url = UPDATE_STATUS;
			if (!string.IsNullOrEmpty(replyId))
				url += "?in_reply_to_status_id=" + replyId;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = HttpMethod.POST.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetUpdateStatusParms(nonce, timestamp, oAuthToken, encodedStatus, replyId, media);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, UPDATE_STATUS, parms);
			string signature = OAuthHelper.SignBaseString(signatureBase, oAuthSecret);
			string authHeader = OAuthHelper.AuthorizationHeader(nonce, signature, timestamp, oAuthToken, true);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		static Dictionary<string, string> GetUpdateStatusParms(string nonce, string timestamp, string oAuthToken, string status, string replyId, string media = null)
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
			if (!string.IsNullOrEmpty(media))
				dic.Add(MEDIA, media);
			if (!string.IsNullOrEmpty(replyId))
				dic.Add("in_reply_to_status_id", replyId);

			return dic;
		}

		static HttpWebRequest GetMediaUploadStatusRequest(string oAuthToken, string oAuthSecret)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(MEDIA_UPLOAD);
			request.Method = HttpMethod.POST.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetUpdateStatusParms(nonce, timestamp, oAuthToken, null, null);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, MEDIA_UPLOAD, parms);
			string signature = OAuthHelper.SignBaseString(signatureBase, oAuthSecret);
			string authHeader = OAuthHelper.AuthorizationHeader(nonce, signature, timestamp, oAuthToken, true);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.FORM_DATA;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
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
