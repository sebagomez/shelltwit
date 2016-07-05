using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;
using Sebagomez.ShelltwitLib.Web;

namespace Sebagomez.ShelltwitLib.API.Tweets
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

			return Util.Deserialize<Statuses>(response.GetResponseStream());
		}
	}
}
