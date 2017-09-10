using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class UserTimeline : BaseAPI
	{
		const string USER_TIMELINE = "https://api.twitter.com/1.1/statuses/user_timeline.json";

		public static async Task<List<Status>> GetUserTimeline(UserTimelineOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			HttpRequestMessage reqMsg = OAuthHelper.GetRequest(HttpMethod.Get, USER_TIMELINE, options);

			return await GetData<Statuses>(reqMsg);
		}
	}
}
