using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sebagomez.BitLyHelper
{
	public class Util
	{
		const string API_URL = "http://api.bit.ly/shorten?version={0}&longUrl={1}&login={2}&apiKey={3}&format=xml";
		const string API_VERSION = "2.0.1";
		const string API_LOGIN = "sebagomez";
		const string API_KEY = "R_bdaed2b56fd1f15c99364abfea6e9b3a";

		static HttpClient s_client = new HttpClient();

		public static async Task<string> GetShortenString(string[] args)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string word in args)
			{
				string newWord = word;
				try
				{
					Uri url = new Uri(word);
					if (url.Host.ToLower() != "bit.ly")
						newWord = await ShortenUrl(WebUtility.UrlEncode(url.ToString()));
				}
				catch {	}

				builder.AppendFormat("{0} ",newWord);
			}

			return builder.ToString().Remove(builder.Length -1);
		}

		public static async Task<string> GetShortenString(string status)
		{
			return await GetShortenString(status.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
		}

		public static async Task<string> ShortenUrl(string url)
		{
			string data = await s_client.GetStringAsync(string.Format(API_URL, API_VERSION, url, API_LOGIN, API_KEY));

			XDocument xDoc = XDocument.Parse(data);

			return GetShortUrl(xDoc);
		}

		private static string GetShortUrl(XDocument xDoc)
		{
			string shortUrl = (from n in xDoc.Descendants()
							   where n.Name == "shortUrl"
							   select n.Value).First();
			return shortUrl;
		}
	}
}
