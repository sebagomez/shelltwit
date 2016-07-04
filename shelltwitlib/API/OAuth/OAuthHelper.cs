using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using shelltwitlib.API.Options;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.OAuth
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
			return Util.EncodeString(Guid.NewGuid().ToString());
		}

		internal static string GetTimestamp()
		{
			TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return Convert.ToInt64(ts.TotalSeconds).ToString();            
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

		internal static HttpWebRequest GetRequest(HttpMethod method, string baseUrl, TwitterOptions options)
		{
			string url = baseUrl;
			if (method == HttpMethod.GET)
			{
				url = $"{baseUrl}?{options.GetUrlParameters()}";
				if (url.EndsWith("?"))
					url = url.Substring(0, url.Length - 1);
			}

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = method.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetParms(nonce, timestamp, options);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, baseUrl, parms);
			string signature = OAuthAuthenticator.SignBaseString(signatureBase, options.User.OAuthTokenSecret);
			string authHeader = OAuthAuthenticator.AuthorizationHeader(nonce, signature, timestamp, options.User.OAuthToken, options.AddOOB);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		static Dictionary<string, string> GetParms(string nonce, string timestamp, TwitterOptions options)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			if (options.AddOOB)
				dic.Add("oauth_callback", "oob");
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(OAuthAuthenticator.CONSUMER_KEY));
			dic.Add(OAuthHelper.OAUTH_NONCE, nonce);
			dic.Add(OAuthHelper.OAUTH_SIGNATURE_METHOD, OAuthHelper.HMAC_SHA1);
			dic.Add(OAuthHelper.OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAuthHelper.OAUTH_VERSION, OAuthHelper.OAUTH_VERSION_10);
			dic.Add(OAuthHelper.OAUTH_TOKEN, options.User.OAuthToken);

			foreach (var item in options.GetParameters())
				dic.Add(item.Key, item.Value);

			return dic;
		}
	}
}
