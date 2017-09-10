namespace Sebagomez.ShelltwitLib.Helpers
{
	public class WebAccessToken
	{
		public const string SessionID = "WebAccessToken";

		public string OAuthToken { get; private set; }
		public string OAuthTokenSecret { get; private set; }
		public string UserId { get; private set; }
		public string ScreenName { get; private set; }

		const string OAUTH_TOKEN = "oauth_token";
		const string OAUTH_TOKEN_SECRET = "oauth_token_secret";
		const string USER_ID = "user_id";
		const string SCREEN_NAME = "screen_name";

		public WebAccessToken(string serializedTokens)
		{
			string[] tokens = Util.GetStringTokens(serializedTokens);
			foreach (string token in tokens)
			{
				if (token.StartsWith(string.Format("{0}=", OAUTH_TOKEN)))
					OAuthToken = Util.GetTokenValue(token);

				if (token.StartsWith(string.Format("{0}=", OAUTH_TOKEN_SECRET)))
					OAuthTokenSecret = Util.GetTokenValue(token);

				if (token.StartsWith(string.Format("{0}=", USER_ID)))
					UserId = Util.GetTokenValue(token);

				if (token.StartsWith(string.Format("{0}=", SCREEN_NAME)))
					ScreenName = Util.GetTokenValue(token);
			}
		}

		
	}
}
