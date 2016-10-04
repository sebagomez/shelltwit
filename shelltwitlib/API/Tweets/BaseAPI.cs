using System.Net.Http;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public abstract class BaseAPI
	{
		public static async Task<T> GetData<T>(HttpRequestMessage reqMsg)
		{
			HttpResponseMessage response = await Util.Client.SendAsync(reqMsg);

			if (!response.IsSuccessStatusCode)
				return default(T);

			return Util.Deserialize<T>(await response.Content.ReadAsStreamAsync());
		}
	}
}
