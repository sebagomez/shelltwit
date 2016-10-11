using System;
using System.Net.Http;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public abstract class BaseAPI
	{
		public static async Task<T> GetData<T>(HttpRequestMessage reqMsg)
		{
			HttpResponseMessage response = await Util.Client.SendAsync(reqMsg);

			if (!response.IsSuccessStatusCode)
			{
				UpdateError err = Util.Deserialize<UpdateError>(await response.Content.ReadAsStreamAsync());
				throw new Exception(err.ToString());
			}

			return Util.Deserialize<T>(await response.Content.ReadAsStreamAsync());
		}
	}
}
