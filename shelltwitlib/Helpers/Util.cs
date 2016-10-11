using System.Text;
using System;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using Sebagomez.ShelltwitLib.Web;
using System.Reflection;

namespace Sebagomez.ShelltwitLib.Helpers
{
	public class Util
	{
		const string MICROSOFT_WINDOWS = "Microsoft Windows";

		static HttpClient s_client = new HttpClient();

		static Util()
		{
			s_client.DefaultRequestHeaders.ExpectContinue = false;
			s_client.DefaultRequestHeaders.Add(Constants.HEADERS.USER_AGENT, Constants.HEADERS.USER_AGENT_VALUE);
		}
	
		public static HttpClient Client
		{
			get { return s_client; }
		}

		static string UNRESERVED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

		//http://en.wikipedia.org/wiki/Percent-encoding
		//http://www.w3schools.com/tags/ref_urlencode.asp see 'Try It Yourself' to see if this function is encoding well
		//This should be encoded according to RFC3986 http://tools.ietf.org/html/rfc3986
		//I could not find any native .net function to achieve this
		/// <summary>
		/// Encodes a string according to RFC 3986
		/// </summary>
		/// <param name="value">string to encode</param>
		/// <returns></returns>
		public static string EncodeString(string value)
		{
			StringBuilder sb = new StringBuilder();
			foreach (char c in value)
			{
				if (UNRESERVED_CHARS.IndexOf(c) != -1)
					sb.Append(c);
				else
				{
					byte[] encoded = Encoding.UTF8.GetBytes(new char[] { c });
					for (int i = 0; i < encoded.Length; i++)
					{
						sb.Append('%');
						sb.Append(encoded[i].ToString("X2"));
					}
				}
			}
			return sb.ToString();
		}

		public static byte[] GetUTF8EncodingBytes(string value)
		{
			return Encoding.UTF8.GetBytes(value);
		}

		public static string GetTokenValue(string token)
		{
			return token.Substring(token.IndexOf("=") + 1);
		}

		public static string[] GetStringTokens(string fullString)
		{
			return fullString.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string FilesLocation
		{
			get
			{
				if (System.Runtime.InteropServices.RuntimeInformation.OSDescription.StartsWith(MICROSOFT_WINDOWS))
					return Environment.GetEnvironmentVariable("LOCALAPPDATA"); //Windows
				else //MacOs or Linux
				{
					string home = Environment.GetEnvironmentVariable("HOME");
					if (!string.IsNullOrEmpty(home))
						return home;
					else
						return new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
				}
			}
		}

		public static string ExceptionMessage(Exception ex)
		{
			string message = ex.Message;
			WebException wex = ex as WebException;
			if (wex != null)
			{
				HttpWebResponse res = (HttpWebResponse)wex.Response;
				if (res != null)
				{
					using (Stream str = res.GetResponseStream())
					using (StreamReader reader = new StreamReader(str))
						message = reader.ReadToEnd();
				}
			}

			return message;
		}

		public static T Deserialize<T>(Stream stream)
		{
#if DEBUG
			string strData;
			using (StreamReader reader = new StreamReader(stream))
				strData = reader.ReadToEnd();

			System.Diagnostics.Debug.Write(strData);

			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(strData)))
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
				return (T)serializer.ReadObject(ms);
			}
#else

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
			return (T)serializer.ReadObject(stream);
#endif
		}
	}
}
