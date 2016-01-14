using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using shelltwitlib.API.OAuth;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Timeline
	{
		const string HOME_TIMELINE = "https://api.twitter.com/1.1/statuses/home_timeline.json";

		public static List<Status> GetTimeline()
		{
			return GetTimeline(null, null);
		}

		public static List<Status> GetTimeline(TwUser user, string lastTweet)
		{
			if (user == null)
				user = TwUser.LoadCredentials();

			HttpWebRequest req = GetTimelineRequest(user.OAuthToken, user.OAuthTokenSecret, lastTweet);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Statuses));
			Statuses ss = (Statuses)serializer.ReadObject(response.GetResponseStream());

			return ss;
		}

		static HttpWebRequest GetTimelineRequest(string oAuthToken, string oAuthSecret, string lastTweet)
		{
			string url = HOME_TIMELINE;
			if (!string.IsNullOrEmpty(lastTweet))
				url += "?since_id=" + lastTweet.Trim();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = HttpMethod.GET.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetTimelineParms(nonce, timestamp, oAuthToken, lastTweet);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, HOME_TIMELINE, parms);
			string signature = OAuthHelper.SignBaseString(signatureBase, oAuthSecret);
			string authHeader = OAuthHelper.AuthorizationHeader(nonce, signature, timestamp, oAuthToken);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		static Dictionary<string, string> GetTimelineParms(string nonce, string timestamp, string oAuthToken, string lastTweet)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(OAuthHelper.CONSUMER_KEY));
			dic.Add(OAuthHelper.OAUTH_SIGNATURE_METHOD, OAuthHelper.HMAC_SHA1);
			dic.Add(OAuthHelper.OAUTH_TIMESTAMP, timestamp);
			dic.Add(OAuthHelper.OAUTH_NONCE, nonce);
			dic.Add(OAuthHelper.OAUTH_VERSION, OAuthHelper.OAUTH_VERSION_10);
			dic.Add(OAuthHelper.OAUTH_TOKEN, oAuthToken);
			if (!string.IsNullOrEmpty(lastTweet))
				dic.Add("since_id", lastTweet);

			return dic;
		}
	}
}
