using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.Entities
{
	public class Statuses : List<Status> { }

	public class Status
	{
		public string created_at { get; set; }
		public long id { get; set; }
		public string id_str { get; set; }
		public string text { get; set; }
		public string full_text { get; set; }
		public bool truncated { get; set; }
		public string source { get; set; }
		public long? in_reply_to_status_id { get; set; }
		public string in_reply_to_status_id_str { get; set; }
		public long? in_reply_to_user_id { get; set; }
		public string in_reply_to_user_id_str { get; set; }
		public string in_reply_to_screen_name { get; set; }
		public User user { get; set; }
		public object geo { get; set; }
		public object coordinates { get; set; }
		public object contributors { get; set; }
		public Status retweeted_status { get; set; }
		public Status extended_tweet { get; set; }
		public bool is_quote_status { get; set; }
		public int retweet_count { get; set; }
		public int favorite_count { get; set; }
		public bool favorited { get; set; }
		public bool retweeted { get; set; }
		public bool? possibly_sensitive { get; set; }
		public string lang { get; set; }

		string m_resolvedText;
		public string ResolvedText
		{
			get
			{
				if (string.IsNullOrEmpty(m_resolvedText))
					m_resolvedText = extended_tweet != null ? extended_tweet.full_text : (retweeted_status != null ? retweeted_status.full_text : (!string.IsNullOrEmpty(full_text) ? full_text : text)); // sometimes text is truncated while the retweeted text isn't

				return m_resolvedText;
			}
		}

		public override string ToString()
		{
			return $"{user.name} (@{user.screen_name}): {ResolvedText}";
		}
	}
}
