using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Helpers;
using Sebagomez.ShelltwitLib.Web;

namespace Sebagomez.ShelltwitLib.API.OAuth
{
	public class OAuthHelper
	{
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

		internal static string GetNonce()
		{
			return Util.EncodeString(Guid.NewGuid().ToString().Replace("-", ""));
		}

		internal static string GetTimestamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalSeconds).ToString();
		}

		internal static string SignatureBsseString(string method, string url, Dictionary<string, string> parms)
		{
			IEnumerable<KeyValuePair<string, string>> sortedParms = parms.OrderBy(parm => parm.Key);

			StringBuilder builder = new StringBuilder($"{method}&{Util.EncodeString(url)}&");
			foreach (KeyValuePair<string, string> p in sortedParms)
				builder.Append($"{p.Key}%3D{Util.EncodeString(p.Value)}%26");

			builder = builder.Remove(builder.Length - 3, 3);

			return builder.ToString();
		}

		internal static HttpRequestMessage GetRequest(HttpMethod method, string baseUrl, TwitterOptions options)
		{
			return GetRequest(method, baseUrl, options, false, false);
		}

		internal static HttpRequestMessage GetRequest(HttpMethod method, string baseUrl, TwitterOptions options, bool callBack, bool imageUpload)
		{
			string url = baseUrl;
			if (method == HttpMethod.Get)
			{
				url = $"{baseUrl}?{options.GetUrlParameters()}";
				if (url.EndsWith("?"))
					url = url.Substring(0, url.Length - 1);
			}

			string nonce = GetNonce();
			string timestamp = GetTimestamp();

			Dictionary<string, string> parms = GetParms(nonce, timestamp, options, callBack, imageUpload);
			string signatureBase = SignatureBsseString(method.Method, baseUrl, parms);
			string signature = OAuthAuthenticator.SignBaseString(signatureBase, options.User.OAuthTokenSecret);
			string authHeader = OAuthAuthenticator.AuthorizationHeader(nonce, signature, timestamp, options.User.OAuthToken, callBack, string.Empty);

			HttpRequestMessage reqMsg = new HttpRequestMessage(method, url);
			reqMsg.Headers.Add(Constants.HEADERS.AUTHORIZATION, authHeader);

			return reqMsg;
		}

		static Dictionary<string, string> GetParms(string nonce, string timestamp, TwitterOptions options, bool callBack, bool imageUpload)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			if (callBack)
				dic.Add("oauth_callback", "oob");
			dic.Add(OAUTH_CONSUMER_KEY, Util.EncodeString(OAuthAuthenticator.CONSUMER_KEY));
			dic.Add(OAUTH_NONCE, nonce);
			dic.Add(OAUTH_SIGNATURE_METHOD, HMAC_SHA1);
			dic.Add(OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAUTH_VERSION, OAUTH_VERSION_10);
			dic.Add(OAUTH_TOKEN, options.User.OAuthToken);

			if (!imageUpload)
			{
				foreach (var item in options.GetParameters())
					dic.Add(item.Key, item.Value);
			}

			return dic;
		}
	}
}
