using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class Likes : BaseAPI
	{
		const string USER_LIKES = "https://api.twitter.com/1.1/favorites/list.json";

		public static async Task<List<Status>> GetUserLikes(LikesOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			HttpRequestMessage reqMsg = OAuthHelper.GetRequest(HttpMethod.Get, USER_LIKES, options);

			return await GetData<Statuses>(reqMsg);
		}
	}
}
