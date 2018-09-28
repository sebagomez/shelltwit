using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class StreamingEndpoint : BaseAPI
	{
		const string STATUS_FILTER = "https://stream.twitter.com/1.1/statuses/filter.json";

		protected void EncodeOptions(StreamingOptions options)
		{
			options.Track = string.IsNullOrEmpty(options.Track) ? options.Track : Util.EncodeString(WebUtility.HtmlDecode(options.Track));
			options.Follow = string.IsNullOrEmpty(options.Follow) ? options.Follow : Util.EncodeString(WebUtility.HtmlDecode(options.Follow));
		}

		public IEnumerable<Status> GetStreamingStatus(StreamingOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.CurrentUser;

			if (!string.IsNullOrWhiteSpace(options.Follow) && !long.TryParse(options.Follow, out long id))
			{
				User data = Task.Run( () => UserData.GetUser(new UserShowOptions { User = options.User, ScreenName = options.Follow })).Result;
				options.Follow = data.id_str;
			}

			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);
				List<KeyValuePair<string, string>> p = new List<KeyValuePair<string, string>>();

				foreach (var item in options.GetParameters())
					p.Add(item);

				var formUrlEncodedContent = new FormUrlEncodedContent(p);
				formUrlEncodedContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

				EncodeOptions(options);
				HttpRequestMessage request = OAuthHelper.GetRequest(HttpMethod.Post, STATUS_FILTER, options);
				request.Content = formUrlEncodedContent;

				var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
				if (!response.IsSuccessStatusCode)
				{
					using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
					{
						string message = reader.ReadToEnd();
						if (!string.IsNullOrEmpty(message))
							throw new Exception($"{response.StatusCode}:{message}");
						else
							throw new Exception($"{response.StatusCode}:{response.ReasonPhrase}");
					}
				}

				using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
				{
					while (!reader.EndOfStream)
					{
						string json = reader.ReadLine();
						if (!string.IsNullOrWhiteSpace(json))
						{
							Status status = Util.Deserialize<Status>(json);
							if (status != null && !string.IsNullOrEmpty(status.ResolvedText))
								yield return status;
						}
					}
				}
			}

		}
	}
}
