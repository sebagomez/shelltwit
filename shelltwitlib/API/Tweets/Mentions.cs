using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Xml.Serialization;
using shelltwitlib.API.OAuth;
using shelltwitlib.API.Options;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Mentions
	{
		const string MENTIONS_STATUS = "https://api.twitter.com/1.1/statuses/mentions_timeline.json";

		public static List<Status> GetMentions()
		{
			return GetMentions(new MentionsOptions());
		}

		public static List<Status> GetMentions(MentionsOptions options)
		{
			if (options.User == null)
				options.User = AuthenticatedUser.LoadCredentials();

			HttpWebRequest req = GetMentionsRequest(options);
			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			if (response.StatusCode != HttpStatusCode.OK)
				return null;

			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Statuses));
			Statuses ss = (Statuses)serializer.ReadObject(response.GetResponseStream());

			return ss;
		}

		static HttpWebRequest GetMentionsRequest(MentionsOptions options)
		{
			string url = MENTIONS_STATUS;
			if (!string.IsNullOrEmpty(options.Since))
				url += "?since_id=" + options.Since.Trim();

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = HttpMethod.GET.ToString();

			string nonce = OAuthHelper.GetNonce();
			string timestamp = OAuthHelper.GetTimestamp();

			Dictionary<string, string> parms = GetMentionsParms(nonce, timestamp, options.User.OAuthToken, options.Since);
			string signatureBase = OAuthHelper.SignatureBsseString(request.Method, MENTIONS_STATUS, parms);
			string signature =  OAuthAuthenticator.SignBaseString(signatureBase, options.User.OAuthTokenSecret);
			string authHeader = OAuthAuthenticator.AuthorizationHeader(nonce, signature, timestamp, options.User.OAuthToken);

			request.Headers.Add(Constants.AUTHORIZATION, authHeader);
			request.ContentType = Constants.CONTENT_TYPE.X_WWW_FORM_URLENCODED;
			request.ServicePoint.Expect100Continue = false;
			request.UserAgent = Constants.USER_AGENT;

			return request;
		}

		static Dictionary<string, string> GetMentionsParms(string nonce, string timestamp, string oAuthToken, string lastTweet)
		{
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add(OAuthHelper.OAUTH_CONSUMER_KEY, Util.EncodeString(OAuthAuthenticator.CONSUMER_KEY));
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
