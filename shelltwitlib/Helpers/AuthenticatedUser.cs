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

		static string s_configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), USER_FILE);

		public string Username { get; set; }

		public string Password { get; set; }

		[XmlAttribute]
		[DataMember]
		public string OAuthToken { get; set; }

		[XmlAttribute]
		[DataMember]
		public string OAuthTokenSecret { get; set; }

		public AuthenticatedUser()
		{
		}

		public AuthenticatedUser(string user, string password, string token, string tokensecret)
		{
			Username = user;
			Password = password;
			OAuthToken = token;
			OAuthTokenSecret = tokensecret;
		}

		public string GetUrlData()
		{
			return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(string.Format("{0}:{1}", Username, Password)));
		}

		public string GetKey(string consumerKey)
		{
			return "";
		}


		public static AuthenticatedUser GetUserCrdentials(string username)
		{
			username = username.Replace(Path.DirectorySeparatorChar, '.');

			string userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), username);

			if (!File.Exists(userPath))
				return null;

			return Deserialize();
		}

		public void SaveUserCredentials(string username)
		{
			username = username.Replace(Path.DirectorySeparatorChar, '.');

			string userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), username);

			Serialize(userPath);
		}

		public static AuthenticatedUser LoadCredentials()
		{
			AuthenticatedUser twiUser = new AuthenticatedUser();
			if (!File.Exists(s_configFile))
			{
				Console.WriteLine("Enter Twitter username...");
				twiUser.Username = Console.ReadLine();

				Console.WriteLine("Enter Twitter password...");
				twiUser.Password = Console.ReadLine();
			}
			else
			{
				twiUser = Deserialize();
			}

			if (string.IsNullOrEmpty(twiUser.OAuthToken) || string.IsNullOrEmpty(twiUser.OAuthTokenSecret))
				twiUser.SetOAuthCredentials().Wait();

			return twiUser;
		}

		public async Task SetOAuthCredentials()
		{
			string accessToken = await OAuthAuthenticator.GetAccessToken(Username, Password);
			string[] tokens = accessToken.Split(new char[] {'&'},StringSplitOptions.RemoveEmptyEntries);

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
			using (FileStream file = File.Open(s_configFile, FileMode.Create))
			{
				DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(AuthenticatedUser));
				jsonSerializer.WriteObject(file, this);
			}
		}

		static AuthenticatedUser Deserialize()
		{
			AuthenticatedUser twiUser = null;

			try
			{
				using (FileStream file = File.Open(s_configFile, FileMode.Open))
				{
					DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(AuthenticatedUser));
					twiUser = (AuthenticatedUser)jsonSerializer.ReadObject(file);
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
