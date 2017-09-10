namespace Sebagomez.ShelltwitLib.API.Tweets
{
	public class StreamingUser : StreamingBase
	{
		const string USER_STREAM = "https://userstream.twitter.com/1.1/user.json";

		protected override string GetStreamingUrl()
		{
			return USER_STREAM;
		}

	}
}
