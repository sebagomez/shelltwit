using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using Sebagomez.ShelltwitLib.Web;

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

		public static string EncodeString(string value)
		{
			return Uri.EscapeDataString(value);
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
			get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
		}

		public static string ExceptionMessage(Exception ex)
		{
			string message = ex.Message;
			if (ex is WebException wex)
			{
				HttpWebResponse res = (HttpWebResponse)wex.Response;
				if (res != null)
				{
					using (Stream str = res.GetResponseStream())
					using (StreamReader reader = new StreamReader(str))
						message = reader.ReadToEnd();
				}
			}
			else if (ex.InnerException != null)
				return ExceptionMessage(ex.InnerException);

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

		public static void Serialize<T>(T obj, string filename)
		{
			using (FileStream file = File.Open(filename, FileMode.Create))
			{
				DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(T));
				jsonSerializer.WriteObject(file, obj);
			}
		}

		public static T Deserialize<T>(string json)
		{
			System.Diagnostics.Debug.Write(json);

			using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
			{
				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
				return (T)serializer.ReadObject(ms);
			}
		}
	}
}
