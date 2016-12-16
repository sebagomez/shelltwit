using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using Sebagomez.ShelltwitLib.API.OAuth;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class StreamingFilter : BaseAPI
	{
		const string STATUS_FILTER = "https://stream.twitter.com/1.1/statuses/filter.json";

		public static IEnumerable<Status> GetStreamingTimeline(StreammingFilterOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			using (HttpClient httpClient = new HttpClient())
			{
				httpClient.Timeout = TimeSpan.FromMilliseconds(Timeout.Infinite);

				List<KeyValuePair<string, string>> p = new List<KeyValuePair<string, string>>();

				foreach (var item in options.GetParameters())
					p.Add(item);

				var formUrlEncodedContent = new FormUrlEncodedContent(p);
				formUrlEncodedContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

				HttpRequestMessage request = OAuthHelper.GetRequest(HttpMethod.Post, STATUS_FILTER, options);
				request.Content = formUrlEncodedContent;

				var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
				using (var reader = new StreamReader(response.Content.ReadAsStreamAsync().Result))
				{
					while (!reader.EndOfStream)
					{
						string json = reader.ReadLine();
						if (!string.IsNullOrWhiteSpace(json))
							yield return Util.Deserialize<Status>(json);
					}
				}
			}

		}
	}

}
