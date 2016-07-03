using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using shelltwitlib.API.OAuth;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Search
	{
		const string SEARCH_URL = "https://api.twitter.com/1.1/search/tweets.json";

		public static SearchResult SearchTweets(string pattern)
		{
			return SearchTweets(null, pattern);
		}

		public static SearchResult SearchTweets(TwUser user, string pattern)
		{
			if (user == null)
				user = TwUser.LoadCredentials();

			pattern = Util.EncodeString(HttpUtility.HtmlDecode(pattern));

			HttpWebRequest req = GetSearchRequest(user.OAuthToken, user.OAuthTokenSecret, pattern);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SearchResult));
			SearchResult result = (SearchResult)serializer.ReadObject(response.GetResponseStream());

			return result;
		}

		static HttpWebRequest GetSearchRequest(string oAuthToken, string oAuthSecret, string search)
		{
			string url = $"{SEARCH_URL}?q={search}&include_entities=0";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = HttpMethod.GET.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetSearchParms(nonce, timestamp, oAuthToken, search);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, SEARCH_URL, parms);
			string signature = OAuthHelper.SignBaseString(signatureBase, oAuthSecret);
			string authHeader = OAuthHelper.AuthorizationHeader(nonce, signature, timestamp, oAuthToken);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		static Dictionary<string, string> GetSearchParms(string nonce, string timestamp, string oAuthToken, string search)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(OAuthHelper.CONSUMER_KEY));
			dic.Add(OAuthHelper.OAUTH_SIGNATURE_METHOD, OAuthHelper.HMAC_SHA1);
			dic.Add(OAuthHelper.OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAuthHelper.OAUTH_NONCE, nonce);
			dic.Add(OAuthHelper.OAUTH_VERSION, OAuthHelper.OAUTH_VERSION_10);
			dic.Add(OAuthHelper.OAUTH_TOKEN, oAuthToken);
			if (!string.IsNullOrEmpty(search))
				dic.Add("q", search);
			dic.Add("include_entities", "0");

			return dic;
		}
	}
}
