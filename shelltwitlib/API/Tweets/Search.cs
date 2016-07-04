using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using shelltwitlib.API.OAuth;
using shelltwitlib.API.Options;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Search
	{
		const int MAX_QUERY_LENGTH = 500;
		const string SEARCH_URL = "https://api.twitter.com/1.1/search/tweets.json";

		public static SearchResult SearchTweets(SearchOptions options)
		{
			if (string.IsNullOrEmpty(options.Query))
				throw new ArgumentNullException("query","The text query cannot be null");

			if (options.Query.Length > MAX_QUERY_LENGTH)
				throw new IndexOutOfRangeException($"Query too long. The query string cannot exceed {MAX_QUERY_LENGTH} chars.");

			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			options.Query = Util.EncodeString(HttpUtility.HtmlDecode(options.Query));

			HttpWebRequest req = GetSearchRequest(options);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SearchResult));
			SearchResult result = (SearchResult)serializer.ReadObject(response.GetResponseStream());

			return result;
		}

		static HttpWebRequest GetSearchRequest(SearchOptions options)
		{
			string url = $"{SEARCH_URL}?{options.GetUrlParameters()}";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = HttpMethod.GET.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetSearchParms(nonce, timestamp, options);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, SEARCH_URL, parms);
			string signature = OAuthAuthenticator.SignBaseString(signatureBase, options.User.OAuthTokenSecret);
			string authHeader = OAuthAuthenticator.AuthorizationHeader(nonce, signature, timestamp, options.User.OAuthToken);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		static Dictionary<string, string> GetSearchParms(string nonce, string timestamp, SearchOptions options)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(OAuthAuthenticator.CONSUMER_KEY));
			dic.Add(OAuthHelper.OAUTH_SIGNATURE_METHOD, OAuthHelper.HMAC_SHA1);
			dic.Add(OAuthHelper.OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAuthHelper.OAUTH_NONCE, nonce);
			dic.Add(OAuthHelper.OAUTH_VERSION, OAuthHelper.OAUTH_VERSION_10);
			dic.Add(OAuthHelper.OAUTH_TOKEN, options.User.OAuthToken);

			foreach (var item in options.GetParameters())
				dic.Add(item.Key, item.Value);

			return dic;
		}
	}
}
