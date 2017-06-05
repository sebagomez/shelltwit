using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.Helpers;
using Sebagomez.ShelltwitLib.Web;

namespace Sebagomez.ShelltwitLib.API.OAuth
{
	public class OAuthAuthenticator
	{
		const string ACCESS_TOKEN = "https://api.twitter.com/oauth/access_token";
		const string REQUEST_TOKEN = "https://api.twitter.com/oauth/request_token";
		const string AUTHORIZE = "https://api.twitter.com/oauth/authorize";

		internal static string CONSUMER_KEY = Settings.Instance.ConsumerKey;
		internal static string CONSUMER_SECRET = Settings.Instance.ConsumerSecret;

		public static void Initilize(string consumerKey, string consumerSecret)
		{
			CONSUMER_KEY = consumerKey;
			CONSUMER_SECRET = consumerSecret;
		}

		static void OnAuthenticationNeeded()
		{
			if (string.IsNullOrEmpty(CONSUMER_KEY) || string.IsNullOrEmpty(CONSUMER_SECRET))
				throw new Exception("Missing Twitter app credentials (Key and/or Secret)");
		}

		public static async Task<string> GetOAuthToken()
		{
			OnAuthenticationNeeded();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = Util.EncodeString(OAuthHelper.GetTimestamp());
			string callback = "oob";

			Dictionary<string, string> dic = GetWebAccessTokenParms(nonce, timestamp, callback, null);

			string signatureBase = OAuthHelper.SignatureBsseString(HttpMethod.Post.Method, REQUEST_TOKEN, dic);
			string signature = SignBaseString(signatureBase, string.Empty);
			string authHeader = AuthorizationHeader(nonce, signature, timestamp, string.Empty, true, string.Empty);

			HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, REQUEST_TOKEN);

			reqMsg.Headers.Add(Constants.HEADERS.AUTHORIZATION, authHeader);

			HttpResponseMessage response = await Util.Client.SendAsync(reqMsg);
			if (!response.IsSuccessStatusCode)
				throw new Exception(await response.Content.ReadAsStringAsync());

			string strResponse = await response.Content.ReadAsStringAsync();

			const string oauth_token = "oauth_token=";
			int start = strResponse.IndexOf(oauth_token);
			int length = strResponse.IndexOf("&", start);

			return strResponse.Substring(start + oauth_token.Length, length - oauth_token.Length);

		}

		public static async Task<string> GetPINToken(string oAuthToken, string pin)
		{
			OnAuthenticationNeeded();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = Util.EncodeString(OAuthHelper.GetTimestamp());

			Dictionary<string, string> dic = GetWebAccessTokenParms(nonce, timestamp, null, pin);

			string signatureBase = OAuthHelper.SignatureBsseString(HttpMethod.Post.Method, ACCESS_TOKEN, dic);
			string signature = SignBaseString(signatureBase, string.Empty);
			string authHeader = AuthorizationHeader(nonce, signature, timestamp, oAuthToken, false, pin);

			HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, ACCESS_TOKEN);

			reqMsg.Headers.Add(Constants.HEADERS.AUTHORIZATION, authHeader);

			HttpResponseMessage response = await Util.Client.SendAsync(reqMsg);
			if (!response.IsSuccessStatusCode)
				throw new Exception(await response.Content.ReadAsStringAsync());

			return await response.Content.ReadAsStringAsync();
		}

		public static async Task<string> GetAccessToken(string username, string password)
		{
			OnAuthenticationNeeded();

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
				throw new Exception("Empty user and/or password");

			string decodedUsr = Util.EncodeString(username);
			string decodedPwd = Util.EncodeString(password);

			HttpRequestMessage reqMsg = new HttpRequestMessage(HttpMethod.Post, ACCESS_TOKEN);

			var postData = new List<KeyValuePair<string, string>>();
			postData.Add(new KeyValuePair<string, string>("x_auth_mode", OAuthHelper.CLIENT_AUTH));
			postData.Add(new KeyValuePair<string, string>("x_auth_password", password));
			postData.Add(new KeyValuePair<string, string>("x_auth_username", username));

			reqMsg.Content = new FormUrlEncodedContent(postData);
			reqMsg.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED);

			string nonce = OAuthHelper.GetNonce();
			string timestamp = Util.EncodeString(OAuthHelper.GetTimestamp());

			Dictionary<string, string> parms = GetAccessTokenParms(nonce, timestamp, decodedUsr, decodedPwd);
			string signatureBase = OAuthHelper.SignatureBsseString(HttpMethod.Post.Method, ACCESS_TOKEN, parms);
			string signature = SignBaseString(signatureBase, string.Empty);
			string authHeader = AuthorizationHeader(nonce, signature, timestamp, string.Empty, string.Empty);

			reqMsg.Headers.Add(Constants.HEADERS.AUTHORIZATION, authHeader);

			HttpResponseMessage response = await Util.Client.SendAsync(reqMsg);
			if (!response.IsSuccessStatusCode)
				throw new Exception(await response.Content.ReadAsStringAsync());

			return await response.Content.ReadAsStringAsync();
		}

		private static Dictionary<string, string> GetAccessTokenParms(string nonce, string timestamp, string username, string password)
		{
			Dictionary<string, string> dic = GetWebAccessTokenParms(nonce, timestamp, null, null);
			dic.Add(OAuthHelper.X_AUTH_MODE, OAuthHelper.CLIENT_AUTH);
			dic.Add(OAuthHelper.X_AUTH_PASSWORD, password);
			dic.Add(OAuthHelper.X_AUTH_USERNAME, username);

			return dic;
		}

		private static Dictionary<string, string> GetWebAccessTokenParms(string nonce, string timestamp, string callback, string verifier)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			if (!string.IsNullOrEmpty(callback))
				dic.Add(OAuthHelper.OAUTH_CALLBACK, Util.EncodeString(callback));
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(CONSUMER_KEY));
			dic.Add(OAuthHelper.OAUTH_NONCE, nonce);
			dic.Add(OAuthHelper.OAUTH_SIGNATURE_METHOD, OAuthHelper.HMAC_SHA1);
			dic.Add(OAuthHelper.OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAuthHelper.OAUTH_VERSION, OAuthHelper.OAUTH_VERSION_10);
			if (!string.IsNullOrEmpty(verifier))
				dic.Add(OAuthHelper.OAUTH_VERIFIER, verifier);

			return dic;
		}

		internal static string AuthorizationHeader(string nonce, string signature, string timestamp, string oAuthToken, string pin)
		{
			return AuthorizationHeader(nonce, signature, timestamp, oAuthToken, false, pin);
		}

		internal static string AuthorizationHeader(string nonce, string signature, string timestamp, string oAuthToken, bool withCallback, string pin)
		{
			OnAuthenticationNeeded();

			string token = string.Empty;
			if (!string.IsNullOrEmpty(oAuthToken))
				token = $"{OAuthHelper.OAUTH_TOKEN}=\"{Util.EncodeString(oAuthToken)}\", ";

			string callBack = "";
			if (withCallback)
				callBack = $"{OAuthHelper.OAUTH_CALLBACK}=\"oob\", ";
			if (!string.IsNullOrEmpty(pin))
				pin = $"{OAuthHelper.OAUTH_VERIFIER}=\"{pin}\", ";

			return $"OAuth {callBack}oauth_nonce=\"{nonce}\", oauth_signature_method=\"{OAuthHelper.HMAC_SHA1}\", oauth_timestamp=\"{timestamp}\", oauth_consumer_key=\"{Util.EncodeString(CONSUMER_KEY)}\", {token} oauth_signature=\"{Util.EncodeString(signature)}\", {pin}oauth_version=\"1.0\"";
		}

		internal static string WebAuthorizationHeader(string nonce, string signature, string timestamp, string pin)
		{
			OnAuthenticationNeeded();

			return $"OAuth oauth_nonce=\"{nonce}\", oauth_signature_method=\"{OAuthHelper.HMAC_SHA1}\", oauth_timestamp=\"{timestamp}\", oauth_consumer_key=\"{Util.EncodeString(CONSUMER_KEY)}\", oauth_signature=\"{Util.EncodeString(signature)}\", oauth_verifier=\"{pin}\", oauth_version=\"1.0\"";
		}

		#region utils

		public static string SignBaseString(string signatureBase, string oAuthSecret)
		{
			OnAuthenticationNeeded();

			HMACSHA1 hmacsha1 = new HMACSHA1();
			hmacsha1.Key = Util.GetUTF8EncodingBytes(string.Format("{0}&{1}", Util.EncodeString(CONSUMER_SECRET), string.IsNullOrEmpty(oAuthSecret) ? "" : Util.EncodeString(oAuthSecret)));

			byte[] dataBuffer = Encoding.ASCII.GetBytes(signatureBase);
			byte[] hashBytes = hmacsha1.ComputeHash(dataBuffer);

			return Convert.ToBase64String(hashBytes);
		}

		#endregion

	}
}
