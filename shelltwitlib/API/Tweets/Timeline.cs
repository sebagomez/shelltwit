using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using shelltwitlib.API.OAuth;
using shelltwitlib.API.Options;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Timeline
	{
		const string HOME_TIMELINE = "https://api.twitter.com/1.1/statuses/home_timeline.json";

		public static List<Status> GetTimeline()
		{
			return GetTimeline(new TimelineOptions());
		}

		public static List<Status> GetTimeline(TimelineOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			HttpWebRequest req = OAuthHelper.GetRequest(HttpMethod.GET, HOME_TIMELINE, options);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Statuses));
			Statuses ss = (Statuses)serializer.ReadObject(response.GetResponseStream());

			return ss;
		}
	}
}
