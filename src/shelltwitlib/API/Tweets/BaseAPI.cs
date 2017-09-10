using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public abstract class BaseAPI
	{
		static Action<string> s_messageFunction;

		public static void SetMessageAction(Action<string> func)
		{
			s_messageFunction = func;
		}

		public static void WriteMessage(string message)
		{
			s_messageFunction?.Invoke(message);
		}

		public static async Task<T> GetData<T>(HttpRequestMessage reqMsg)
		{
			HttpResponseMessage response = await Util.Client.SendAsync(reqMsg);
			Stream stream = await response.Content.ReadAsStreamAsync();

			if (!response.IsSuccessStatusCode)
			{
				UpdateError err = null;
				Stream aux = new MemoryStream();
				stream.CopyTo(aux);
				stream.Position = 0;
				aux.Position = 0;
				try
				{
					err = Util.Deserialize<UpdateError>(stream);
				}
				catch (Exception)
				{
					string strData;
					using (StreamReader reader = new StreamReader(aux))
						strData = reader.ReadToEnd();

					throw new Exception($"{response.StatusCode}:{strData}");
				}

				throw new Exception(err.ToString());
			}

			return Util.Deserialize<T>(stream);
		}
	}
}
