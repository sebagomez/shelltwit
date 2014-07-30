using System.Text;
using System;

namespace shelltwitlib.Helpers
{
	public class Util
	{
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
	}
}
