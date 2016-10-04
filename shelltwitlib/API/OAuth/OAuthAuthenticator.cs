using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Sebagomez.ShelltwitLib.Helpers;
using Sebagomez.ShelltwitLib.Web;

namespace Sebagomez.ShelltwitLib.API.OAuth
{
	public class OAuthAuthenticator
	{
		const string ACCESS_TOKEN = "https://api.twitter.com/oauth/access_token";
		const string REQUEST_TOKEN = "https://api.twitter.com/oauth/request_token";
		const string AUTHORIZE = "https://api.twitter.com/oauth/authorize";

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
			request.Method = HttpMethod.Post.Method;
			request.UserAgent = Constants.HEADERS.USER_AGENT_VALUE;
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;

			byte[] body = Util.GetUTF8EncodingBytes(AccessTokenRequestBody(decodedUsr, decodedPwd));

			using (Stream str = request.GetRequestStream())
				str.Write(body, 0, body.Length);


			string nonce = OAuthHelper.GetNonce();
			string timestamp = Util.EncodeString(OAuthHelper.GetTimestamp());

			Dictionary<string, string> parms = GetAccessTokenParms(nonce, timestamp, decodedUsr, decodedPwd);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, ACCESS_TOKEN, parms);
			string signature = SignBaseString(signatureBase, string.Empty);
			string authHeader = AuthorizationHeader(nonce, signature, timestamp, string.Empty);
			request.Headers.Add(Constants.HEADERS.AUTHORIZATION, authHeader);

			return GetResponseString(request);
		}

		static string AccessTokenRequestBody(string username, string password)
		{
			return $"x_auth_mode={OAuthHelper.CLIENT_AUTH}&x_auth_password={password}&x_auth_username={username}";
		}

		private static Dictionary<string, string> GetAccessTokenParms(string nonce, string timestamp, string username, string password)
		{
			Dictionary<string, string> dic = GetWebAccessTokenParms(nonce, timestamp, string.Empty);
			dic.Add(OAuthHelper.X_AUTH_MODE, OAuthHelper.CLIENT_AUTH);
			dic.Add(OAuthHelper.X_AUTH_PASSWORD, password);
			dic.Add(OAuthHelper.X_AUTH_USERNAME, username);

			return dic;
		}

		private static Dictionary<string, string> GetWebAccessTokenParms(string nonce, string timestamp, string callback)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(callback))
				dic.Add(OAuthHelper.OAUTH_CALLBACK, Util.EncodeString(callback));
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(CONSUMER_KEY));
			dic.Add(OAuthHelper.OAUTH_NONCE, nonce);
			dic.Add(OAuthHelper.OAUTH_SIGNATURE_METHOD, OAuthHelper.HMAC_SHA1);
			dic.Add(OAuthHelper.OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAuthHelper.OAUTH_VERSION, OAuthHelper.OAUTH_VERSION_10);

			return dic;
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

		internal static string AuthorizationHeader(string nonce, string signature, string timestamp, string oAuthToken)
		{
			return AuthorizationHeader(nonce, signature, timestamp, oAuthToken, false);
		}

		internal static string AuthorizationHeader(string nonce, string signature, string timestamp, string oAuthToken, bool withCallback)
		{
			string token = string.Empty;
			if (!string.IsNullOrEmpty(oAuthToken))
				token = OAuthHelper.OAUTH_TOKEN + "=\"" + Util.EncodeString(oAuthToken) + "\", ";

			if (withCallback)
				return $"OAuth oauth_callback=\"oob\", oauth_consumer_key=\"{Util.EncodeString(CONSUMER_KEY)}\", oauth_nonce=\"{nonce}\", oauth_signature=\"{Util.EncodeString(signature)}\", oauth_signature_method=\"HMAC-SHA1\", oauth_timestamp=\"{timestamp}\", {token}oauth_version=\"1.0\"";

			return $"OAuth oauth_nonce=\"{nonce}\", oauth_signature_method=\"{OAuthHelper.HMAC_SHA1}\", oauth_timestamp=\"{timestamp}\", oauth_consumer_key=\"{Util.EncodeString(CONSUMER_KEY)}\", {token} oauth_signature=\"{Util.EncodeString(signature)}\", oauth_version=\"1.0\"";
		}

		internal static string WebAuthorizationHeader(string nonce, string signature, string timestamp, string callback)
		{
			return $"OAuth oauth_nonce=\"{nonce}\", oauth_callback=\"{Util.EncodeString(callback)}\", oauth_signature_method=\"{OAuthHelper.HMAC_SHA1}\", oauth_timestamp=\"{timestamp}\", oauth_consumer_key=\"{Util.EncodeString(CONSUMER_KEY)}\", oauth_signature=\"{Util.EncodeString(signature)}\", oauth_version=\"1.0\"";
		}

		#region utils

		public static string SignBaseString(string signatureBase, string oAuthSecret)
		{
			HMACSHA1 hmacsha1 = new HMACSHA1();
			hmacsha1.Key = Util.GetUTF8EncodingBytes(string.Format("{0}&{1}", Util.EncodeString(CONSUMER_SECRET), string.IsNullOrEmpty(oAuthSecret) ? "" : Util.EncodeString(oAuthSecret)));

			byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
			byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

			return Convert.ToBase64String(hashBytes);
		}

		#endregion

	}
}
