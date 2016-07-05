using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Sebagomez.BitLyHelper
{
	public class Util
	{
		const string API_URL = "http://api.bit.ly/shorten?version={0}&longUrl={1}&login={2}&apiKey={3}&format=xml";
		const string API_VERSION = "2.0.1";
		const string API_LOGIN = "sebagomez";
		const string API_KEY = "R_bdaed2b56fd1f15c99364abfea6e9b3a";

		public static string GetShortenString(string[] args)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string word in args)
			{
				string newWord = word;
				try
				{
					Uri url = new Uri(word);
					if (url.Host.ToLower() != "bit.ly")
						newWord = ShortenUrl(HttpUtility.UrlEncode(url.ToString()));
				}
				catch {	}

				builder.AppendFormat("{0} ",newWord);
			}

			return builder.ToString().Remove(builder.Length -1);
		}

		public static string GetShortenString(string status)
		{
			return GetShortenString(status.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
		}

		public static string ShortenUrl(string url)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(API_URL, API_VERSION, url, API_LOGIN, API_KEY));
			request.Method = "GET";
			request.ContentType = "application/x-www-form-urlencoded";
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			string data = string.Empty;
			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				data = reader.ReadToEnd();

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
