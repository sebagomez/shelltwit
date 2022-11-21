using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sebagomez.TwitterLib.API.OAuth;
using Sebagomez.TwitterLib.Helpers;

namespace Sebagomez.Shelltwit.Security
{
	internal class CredentialsManager
	{
		static readonly string TWITTER_API_KEY = "TWITTER_API_KEY";
		static readonly string TWITTER_API_SECRET = "TWITTER_API_SECRET";

		static string s_lastUser => Path.Combine(AppContext.BaseDirectory, "last.usr");
		static string s_userFileExt => ".data";
		static string s_userFileTemplate => "{0}" + s_userFileExt;

		public static AuthenticatedUser LoadCredentials()
		{
			string username = GetLastUserName();
			AuthenticatedUser user;
			if (!string.IsNullOrEmpty(username))
			{
				user = AuthenticatedUser.Deserialize(GetUserDataPath(username));
				if (user != null && user.IsOk())
					return user;
			}

			string twitterKey = Environment.GetEnvironmentVariable(TWITTER_API_KEY);
			if (string.IsNullOrEmpty(twitterKey))
			{
				Console.WriteLine("Insert your app Twitter Key");
				twitterKey = Console.ReadLine();
			}
			string twitterSecret = Environment.GetEnvironmentVariable(TWITTER_API_SECRET);
			if (string.IsNullOrEmpty(twitterSecret))
			{
				Console.WriteLine("Insert your app Twitter Key Secret");
				twitterSecret = Console.ReadLine();
			}

			if (string.IsNullOrEmpty(twitterKey) || string.IsNullOrEmpty(twitterSecret))
				throw new ApplicationException($"No {TWITTER_API_KEY} and/or {TWITTER_API_SECRET} set");

			user = AuthenticateUser(twitterKey, twitterSecret);

			if (user != null)
			{
				user.Serialize(GetUserDataPath(user.ScreenName));
				File.WriteAllLines(s_lastUser, new string[] { user.ScreenName });
			}

			return user;
		}

		static string GetLastUserName()
		{
			string username = "";
			if (File.Exists(s_lastUser))
			{
				using (StreamReader file = File.OpenText(s_lastUser))
					username = file.ReadLine();

				if (string.IsNullOrEmpty(username))
					File.Delete(s_lastUser);
			}

			return username;
		}

		static string GetUserDataPath(string username) => Path.Combine(AppContext.BaseDirectory, string.Format(s_userFileTemplate, username));
		

		public static void ClearCredentials()
		{
			File.Delete(s_lastUser);
			foreach (string userFile in Directory.GetFiles(AppContext.BaseDirectory, $"*{s_userFileExt}"))
				File.Delete(userFile);
		}

		private static void SetSettings()
		{
			if (File.Exists("Properties\\launchSettings.json"))
			{
				using (var file = File.OpenText("Properties\\launchSettings.json"))
				{
					var reader = new JsonTextReader(file);
					var jObject = JObject.Load(reader);

					var variables = jObject
						.GetValue("profiles")
						.SelectMany(profiles => profiles.Children())
						.SelectMany(profile => profile.Children<JProperty>())
						.Where(prop => prop.Name == "environmentVariables")
						.SelectMany(prop => prop.Value.Children<JProperty>())
						.ToList();

					foreach (var variable in variables)
					{
						Environment.SetEnvironmentVariable(variable.Name, variable.Value.ToString());
					}
				}
			}
		}

		private static AuthenticatedUser AuthenticateUser(string twitterKey, string twitterSecret)
		{
			AuthenticatedUser twiUser = new AuthenticatedUser
			{
				AppSettings = new AppCredentials() { AppKey = twitterKey, AppSecret = twitterSecret }
			};
			Console.Write("Getting Twitter authentication token...");
			string token = OAuthAuthenticator.GetOAuthToken(twitterKey, twitterSecret).Result;
			Console.WriteLine("done!");
			Console.WriteLine("Please open your favorite browser and go to this URL to authenticate with Twitter:");
			Console.WriteLine($"https://api.twitter.com/oauth/authorize?oauth_token={token}");
			Console.Write("Insert the pin here:");
			string pin = Console.ReadLine();

			Console.Write("Getting Twitter access token...");
			string accessToken = OAuthAuthenticator.GetPINToken(token, pin, twitterKey, twitterSecret).Result;
			twiUser.ParseTokens(accessToken);
			Console.WriteLine("done!");

			Console.WriteLine($"Welcome {twiUser.ScreenName}!");
			Console.WriteLine("");

			return twiUser;
		}
	}
}
