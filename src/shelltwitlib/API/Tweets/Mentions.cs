using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class Mentions : BaseAPI
	{
		const string MENTIONS_STATUS = "https://api.twitter.com/1.1/statuses/mentions_timeline.json";

		public static async Task<List<Status>> GetMentions(MentionsOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			HttpRequestMessage reqMsg = OAuthHelper.GetRequest(HttpMethod.Get, MENTIONS_STATUS, options);

			return await GetData<Statuses>(reqMsg);
		}
	}
}
