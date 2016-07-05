
namespace Sebagomez.ShelltwitLib.Web
{
	internal class Constants
	{
		public class CONTENT_TYPE
		{
			public const string X_WWW_FORM_URLENCODED = "application/x-www-form-urlencoded";
			public const string FORM_DATA = "multipart/form-data";
		}

		public const string AUTHORIZATION = "Authorization";
		public const string USER_AGENT = "@sebagomez shelltwit";
	}

	enum HttpMethod { GET, POST }
}
