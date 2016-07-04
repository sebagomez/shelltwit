using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using shelltwitlib.API.OAuth;
using shelltwitlib.API.Options;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Mentions
	{
		const string MENTIONS_STATUS = "https://api.twitter.com/1.1/statuses/mentions_timeline.json";

		public static List<Status> GetMentions()
		{
			return GetMentions(new MentionsOptions());
		}

		public static List<Status> GetMentions(MentionsOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			HttpWebRequest req = OAuthHelper.GetRequest(HttpMethod.GET, MENTIONS_STATUS, options);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Statuses));
			Statuses ss = (Statuses)serializer.ReadObject(response.GetResponseStream());

			return ss;
		}
	}
}
