using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Sebagomez.Shelltwit.Security
{
	internal class VaultCredntials
	{
		const string SHELLTWIT_API_KEY = "SHELLTWIT-API-KEY";
		const string SHELLTWIT_API_SECRET = "SHELLTWIT-API-SECRET";
		const string VAULT_URL = "VAULT_URL";
		const string APPLICATION_ID = "APPLICATION_ID";
		const string APPLICATION_SECRET = "APPLICATION_SECRET";

		private string m_vaultUrl;
		public string VaultUrl
		{
			get
			{
				if (string.IsNullOrEmpty(m_vaultUrl))
					m_vaultUrl = Environment.GetEnvironmentVariable(VAULT_URL);

				return m_vaultUrl;
			}
		}

		private string m_appId;
		public string ApplicationId
		{
			get
			{
				if (string.IsNullOrEmpty(m_appId))
					m_appId = Environment.GetEnvironmentVariable(APPLICATION_ID);

				return m_appId;
			}
		}

		private string m_appSecret;
		public string ApplicationSecret
		{
			get
			{
				if (string.IsNullOrEmpty(m_appSecret))
					m_appSecret = Environment.GetEnvironmentVariable(APPLICATION_SECRET);

				return m_appSecret;
			}
		}


		object _lock = new object();

		private string m_apiSecret;
		public string SHELLTWIT_SECRET
		{
			get
			{
				if (string.IsNullOrEmpty(m_apiSecret))
				{
					lock (_lock)
					{
						if (string.IsNullOrEmpty(m_apiSecret))
							LoadCredentials();
					}
				}
				return m_apiSecret;
			}
		}

		private string m_apiKey;
		public string SHELLTWIT_KEY
		{
			get
			{
				if (string.IsNullOrEmpty(m_apiKey))
				{
					lock (_lock)
					{
						if (string.IsNullOrEmpty(m_apiKey))
							LoadCredentials();
					}
				}
				return m_apiKey;
			}
		}

		public void LoadCredentials()
		{
			Task.Run(async () =>
			{
				m_apiSecret = await GetSecretAsync(VaultUrl, SHELLTWIT_API_SECRET);
				m_apiKey = await GetSecretAsync(VaultUrl, SHELLTWIT_API_KEY);
			}).GetAwaiter().GetResult();

		}

		private async Task<string> GetSecretAsync(string vaultUrl, string vaultKey)
		{
			var client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessTokenAsync), new HttpClient());
			var secret = await client.GetSecretAsync(vaultUrl, vaultKey);

			return secret.Value;
		}

		private async Task<string> GetAccessTokenAsync(string authority, string resource, string scope)
		{
			var appCredentials = new ClientCredential(ApplicationId, ApplicationSecret);
			var context = new AuthenticationContext(authority, TokenCache.DefaultShared);

			var result = await context.AcquireTokenAsync(resource, appCredentials);

			return result.AccessToken;
		}
	}
}
