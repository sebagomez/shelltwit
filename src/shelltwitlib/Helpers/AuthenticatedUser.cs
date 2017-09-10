using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Sebagomez.ShelltwitLib.API.OAuth;

namespace Sebagomez.ShelltwitLib.Helpers
{
	[DataContract]
	public class AuthenticatedUser
	{
		const string USER_FILE = "twit.usr";
		const string OAUTH_TOKEN = "oauth_token";
		const string OAUTH_TOKEN_SECRET = "oauth_token_secret";

		static string s_configFile = Path.Combine(Util.FilesLocation, USER_FILE);

		[XmlAttribute]
		[DataMember]
		public string OAuthToken { get; set; }

		[XmlAttribute]
		[DataMember]
		public string OAuthTokenSecret { get; set; }

		public AuthenticatedUser()
		{
		}

		public AuthenticatedUser(string token, string tokensecret)
		{
			OAuthToken = token;
			OAuthTokenSecret = tokensecret;
		}

		public static AuthenticatedUser GetUserCrdentials(string username)
		{
			username = username.Replace(Path.DirectorySeparatorChar, '.');

			string userPath = Path.Combine(Util.FilesLocation, username);

			if (!File.Exists(userPath))
				return null;

			return Deserialize();
		}

		public void SaveUserCredentials(string username)
		{
			username = username.Replace(Path.DirectorySeparatorChar, '.');

			string userPath = Path.Combine(Util.FilesLocation, username);

			Serialize(userPath);
		}

		public static AuthenticatedUser LoadCredentials()
		{
			AuthenticatedUser twiUser = new AuthenticatedUser();
			if (!File.Exists(s_configFile))
			{
				string token = OAuthAuthenticator.GetOAuthToken().Result;
				Console.WriteLine("Please open your favorite browser and go to this URL to authenticate with Twitter:");
				Console.WriteLine($"https://api.twitter.com/oauth/authorize?oauth_token={token}");
				Console.Write("Insert the pin here:");

				string pin = Console.ReadLine();

				string accessToken = OAuthAuthenticator.GetPINToken(token, pin).Result;
				twiUser.SerializeTokens(accessToken);
				Console.WriteLine("Sucess!");
				Console.WriteLine("");
			}
			else
			{
				twiUser = Deserialize();
			}

			return twiUser;
		}

		public void SerializeTokens(string accessTokens)
		{
			string[] tokens = accessTokens.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

			foreach (string tok in tokens)
			{
				string[] props = tok.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
				if (props[0] != OAUTH_TOKEN && props[0] != OAUTH_TOKEN_SECRET)
					continue;

				if (props[0] == OAUTH_TOKEN)
					OAuthToken = props[1];
				else if (props[0] == OAUTH_TOKEN_SECRET)
					OAuthTokenSecret = props[1];

				if (!string.IsNullOrEmpty(OAuthToken) && !string.IsNullOrEmpty(OAuthTokenSecret))
					break;
			}

			Serialize();
		}

		private void Serialize()
		{
			Serialize(s_configFile);
		}

		public void Serialize(string fileName)
		{
			Util.Serialize(this, fileName);
		}

		static AuthenticatedUser Deserialize()
		{
			AuthenticatedUser twiUser = null;

			try
			{

				using (FileStream file = File.Open(s_configFile, FileMode.Open))
				{
					twiUser = Util.Deserialize<AuthenticatedUser>(file);

					//DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(AuthenticatedUser));
					//twiUser = (AuthenticatedUser)jsonSerializer.ReadObject(file);
				}
			}
			catch (SerializationException)
			{
				XmlSerializer deserializer = new XmlSerializer(typeof(AuthenticatedUser));
				using (StreamReader reader = File.OpenText(s_configFile))
					twiUser = (AuthenticatedUser)deserializer.Deserialize(reader);

				twiUser.Serialize();
			}
			return twiUser;
		}

		public static void ClearCredentials()
		{
			if (File.Exists(s_configFile))
				File.Delete(s_configFile);
		}
	}
}
