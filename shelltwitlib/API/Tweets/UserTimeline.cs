using System.Collections.Generic;
using System.Net;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;
using Sebagomez.ShelltwitLib.Web;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class UserTimeline
	{
		const string USER_TIMELINE = "https://api.twitter.com/1.1/statuses/user_timeline.json";

		public static List<Status> GetUserTimeline(UserTimelineOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			HttpWebRequest req = OAuthHelper.GetRequest(HttpMethod.GET, USER_TIMELINE, options);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			return Util.Deserialize<Statuses>(response.GetResponseStream());
		}
	}
}
