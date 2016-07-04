using System;
using System.Net;
using System.Runtime.Serialization.Json;
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

			HttpWebRequest req = OAuthHelper.GetRequest(HttpMethod.GET, SEARCH_URL, options);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(SearchResult));
			SearchResult result = (SearchResult)serializer.ReadObject(response.GetResponseStream());

			return result;
		}
	}
}
