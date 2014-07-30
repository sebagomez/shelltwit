using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using shelltwitlib.Web;
using shelltwitlib.Helpers;

namespace shelltwitlib.API.OAuth
{
	public class OAuthHelper
	{
		const string ACCESS_TOKEN = "https://api.twitter.com/oauth/access_token";
		const string REQUEST_TOKEN = "https://api.twitter.com/oauth/request_token";
		const string AUTHORIZE = "https://api.twitter.com/oauth/authorize";
		
		#region tokens

		public const string X_AUTH_USERNAME = "x_auth_username";
		public const string X_AUTH_PASSWORD = "x_auth_password";
		public const string X_AUTH_MODE = "x_auth_mode"; // must be client_auth 
		public const string CLIENT_AUTH = "client_auth"; 
		public const string OAUTH_CONSUMER_KEY = "oauth_consumer_key";
		public const string OAUTH_NONCE = "oauth_nonce";
		public const string OAUTH_SIGNATURE_METHOD = "oauth_signature_method"; // must be HMAC-SHA1 
		public const string HMAC_SHA1 = "HMAC-SHA1";
		public const string OAUTH_SIGNATURE = "oauth_signature";
		public const string OAUTH_TIMESTAMP = "oauth_timestamp";
		public const string OAUTH_VERSION = "oauth_version"; //must be 1.0
		public const string OAUTH_VERSION_10 = "1.0";
		public const string OAUTH_TOKEN = "oauth_token";
		public const string OAUTH_CALLBACK = "oauth_callback";
		public const string OAUTH_TOKEN_SECRET = "oauth_token_secret";
		public const string OAUTH_VERIFIER = "oauth_verifier";
		
		#endregion

		internal static string CONSUMER_KEY = "";
		internal static string CONSUMER_SECRET = "";

		public static void Initilize(string consumerKey, string consumerSecret)
		{
			CONSUMER_KEY = consumerKey;
			CONSUMER_SECRET = consumerSecret;
		}

		public static string GetAccessToken(string username, string password)
		{
			string decodedUsr = Util.EncodeString(username);
			string decodedPwd = Util.EncodeString(password);

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ACCESS_TOKEN);
			request.Method = HttpMethod.POST.ToString();
			request.UserAgent = Constants.USER_AGENT;
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;

			byte[] body = Util.GetUTF8EncodingBytes(AccessTokenRequestBody(decodedUsr, decodedPwd));

			using (Stream str = request.GetRequestStream())
				str.Write(body, 0, body.Length);


			string nonce = GetNonce();
			string timestamp = Util.EncodeString(GetTimestamp());

			Dictionary<string, string> parms = GetAccessTokenParms(nonce, timestamp, decodedUsr, decodedPwd);
			string signatureBase = SignatureBsseString(request.Method, ACCESS_TOKEN, parms);
			string signature = SignBaseString(signatureBase,string.Empty);
			string authHeader = AuthorizationHeader(nonce,signature,timestamp,string.Empty);
			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ServicePoint.Expect100Continue = false;

			return GetResponseString(request);
		}

		public static string GetWebAccessToken(string oAuthToken, string oAuthTokenSecret, string oAuthVerifier)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ACCESS_TOKEN);
			request.Method = HttpMethod.POST.ToString();
			request.UserAgent = Constants.USER_AGENT;
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;

			//byte[] body = Util.GetUTF8EncodingBytes(AccessTokenRequestBody(decodedUsr, decodedPwd));

			//using (Stream str = request.GetRequestStream())
			//    str.Write(body, 0, body.Length);


			string nonce = GetNonce();
			string timestamp = Util.EncodeString(GetTimestamp());

			Dictionary<string, string> parms = GetWebRequestTokenParms(nonce, timestamp, oAuthVerifier);
			string signatureBase = SignatureBsseString(request.Method, ACCESS_TOKEN, parms);
			string signature = SignBaseString(signatureBase, oAuthTokenSecret);
			string authHeader = AuthorizationHeader(nonce, signature, timestamp, oAuthToken);
			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ServicePoint.Expect100Continue = false;

			return GetResponseString(request);
		}

		public static string GetWebRequestToken(string callback)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(REQUEST_TOKEN);
			request.Method = HttpMethod.POST.ToString();
			request.UserAgent = Constants.USER_AGENT;
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;

			string nonce = GetNonce();
			string timestamp = Util.EncodeString(GetTimestamp());

			Dictionary<string, string> parms = GetWebAccessTokenParms(nonce, timestamp, callback);
			string signatureBase = SignatureBsseString(request.Method, REQUEST_TOKEN, parms);
			string signature = SignBaseString(signatureBase, null);
			string authHeader = WebAuthorizationHeader(nonce, signature, timestamp, callback);
			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ServicePoint.Expect100Continue = false;

			return GetResponseString(request);
		}

		public static string GetWebAuthorizationUrl(string oAuthToken)
		{
			return string.Format(AUTHORIZE + "?" + OAUTH_TOKEN + "={0}", oAuthToken);
		}

		private static string GetResponseString(HttpWebRequest request)
		{
			string responseData = null;

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using (Stream str = response.GetResponseStream())
			using (StreamReader reader = new StreamReader(str))
				responseData = reader.ReadToEnd();
			
			return responseData;
		}

		internal static string GetNonce()
		{
			return Util.EncodeString(Guid.NewGuid().ToString());
		}

		public static string SignBaseString(string signatureBase, string oAuthSecret)
		{
			HMACSHA1 hmacsha1 = new HMACSHA1();
			hmacsha1.Key = Util.GetUTF8EncodingBytes(string.Format("{0}&{1}", Util.EncodeString(CONSUMER_SECRET), string.IsNullOrEmpty(oAuthSecret) ? "" : Util.EncodeString(oAuthSecret)));

			byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
			byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

			return Convert.ToBase64String(hashBytes);
		}

		private static Dictionary<string, string> GetAccessTokenParms(string nonce, string timestamp, string username, string password)
		{
			Dictionary<string, string> dic = GetWebAccessTokenParms(nonce,timestamp, string.Empty);
			dic.Add(X_AUTH_MODE, CLIENT_AUTH);
			dic.Add(X_AUTH_PASSWORD, password);
			dic.Add(X_AUTH_USERNAME, username);

			return dic;
		}

		private static Dictionary<string, string> GetWebAccessTokenParms(string nonce, string timestamp, string callback)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(callback))
				dic.Add(OAUTH_CALLBACK, Util.EncodeString(callback));
			dic.Add(OAUTH_CONSUMER_KEY, Util.EncodeString(CONSUMER_KEY));
			dic.Add(OAUTH_NONCE, nonce);
			dic.Add(OAUTH_SIGNATURE_METHOD, HMAC_SHA1);
			dic.Add(OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAUTH_VERSION, OAUTH_VERSION_10);

			return dic;
		}

		private static Dictionary<string, string> GetWebRequestTokenParms(string nonce, string timestamp, string oAuthVerifier)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add(OAUTH_CONSUMER_KEY, Util.EncodeString(CONSUMER_KEY));
			dic.Add(OAUTH_NONCE, nonce);
			dic.Add(OAUTH_SIGNATURE_METHOD, HMAC_SHA1);
			dic.Add(OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAUTH_VERIFIER, oAuthVerifier);
			dic.Add(OAUTH_VERSION, OAUTH_VERSION_10);

			return dic;
		}

		static string AccessTokenRequestBody(string username, string password)
		{
			return string.Format("x_auth_mode={0}&x_auth_password={1}&x_auth_username={2}", CLIENT_AUTH, password, username);
		}

		internal static string GetTimestamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalSeconds).ToString();            
		}

		internal static string AuthorizationHeader(string nonce, string signature, string timestamp, string oAuthToken)
		{
			return AuthorizationHeader(nonce, signature, timestamp, oAuthToken, false);
		}

		internal static string AuthorizationHeader(string nonce, string signature, string timestamp, string oAuthToken, bool withCallback)
		{
			string token = string.Empty;
			if (!string.IsNullOrEmpty(oAuthToken))
				token = OAUTH_TOKEN + "=\"" + Util.EncodeString(oAuthToken) + "\", ";

			if (withCallback)
			    return string.Format("OAuth oauth_callback=\"oob\", oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", oauth_signature=\"{2}\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"{3}\", " + token + "oauth_version=\"1.0\"", Util.EncodeString(CONSUMER_KEY), nonce, Util.EncodeString(signature),timestamp );
				
			return string.Format("OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " + token + "oauth_signature=\"{4}\", oauth_version=\"1.0\"", nonce, HMAC_SHA1, timestamp, Util.EncodeString(CONSUMER_KEY), Util.EncodeString(signature));
		}

		private static string WebAuthorizationHeader(string nonce, string signature, string timestamp, string callback)
		{
			return string.Format("OAuth oauth_nonce=\"{0}\", oauth_callback=\"{1}\", oauth_signature_method=\"{2}\", oauth_timestamp=\"{3}\", oauth_consumer_key=\"{4}\", oauth_signature=\"{5}\", oauth_version=\"1.0\"", nonce, Util.EncodeString(callback), HMAC_SHA1, timestamp, Util.EncodeString(CONSUMER_KEY), Util.EncodeString(signature));
		}

		internal static string SignatureBsseString(string method, string url, Dictionary<string,string> parms)
		{
			IEnumerable<KeyValuePair<string,string>> sortedParms = parms.OrderBy(parm => parm.Key);

			StringBuilder builder = new StringBuilder(method + "&" + Util.EncodeString(url) + "&");
			foreach (KeyValuePair<string, string> p in sortedParms)
				builder.AppendFormat("{0}%3D{1}%26", p.Key, Util.EncodeString(p.Value));

			builder = builder.Remove(builder.Length - 3, 3);

			return builder.ToString();
		}
	}
}
