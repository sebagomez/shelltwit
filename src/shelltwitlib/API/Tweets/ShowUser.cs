using System;
using System.Net.Http;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class UserData : BaseAPI
	{
		const string SHOW_USER_URL = "https://api.twitter.com/1.1/users/show.json";

		public static async Task<User> GetUser(UserShowOptions options)
		{
			if (string.IsNullOrEmpty(options.UserId) && string.IsNullOrEmpty(options.ScreenName))
				throw new Exception("You mst set either a screen name or a user id");

			if (options.User == null)
				options.User = AuthenticatedUser.CurrentUser;

			HttpRequestMessage reqMsg = OAuthHelper.GetRequest(HttpMethod.Get, SHOW_USER_URL, options);

			return await GetData<User>(reqMsg);
		}
	}
}
