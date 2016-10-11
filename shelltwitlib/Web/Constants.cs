
using System.Net;


namespace Sebagomez.ShelltwitLib.Web
{
	internal class Constants
	{
		public class CONTENT_TYPE
		{
			public const string X_WWW_FORM_URLENCODED = "application/x-www-form-urlencoded";
			public const string FORM_DATA = "multipart/form-data";
			public const string OCTET_STREAM = "application/octet-stream";
		}

		public const string FORM_DATA = "form-data; name=\"media\"; filename=\"{0}\"";

		public class HEADERS
		{
			public const string AUTHORIZATION = "Authorization";
			public static readonly string USER_AGENT_VALUE = WebUtility.UrlEncode("@sebagomez shelltwit");
			public const string USER_AGENT = "User-Agent";
			public const string CONTENT_TYPE = "Content-Type";
			public const string CONTENT_DISPOSITION = "Content-Disposition";
		}
	}
}
