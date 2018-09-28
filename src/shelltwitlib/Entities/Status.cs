using System;
using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.Entities
{
	public class Statuses : List<Status> { }

	public class Status
	{
		public string created_at { get; set; }
		public long id { get; set; }
		public string text { get; set; }
		public string full_text { get; set; }
		public string source { get; set; }
		public object in_reply_to_status_id { get; set; }
		public object in_reply_to_user_id { get; set; }
		public object in_reply_to_screen_name { get; set; }
		public User user { get; set; }
		public bool is_quote_status { get; set; }
		public string lang { get; set; }
		public Status extended_tweet { get; set; }
		public Status retweeted_status { get; set; }
		public long quoted_status_id { get; set; }
		public Status quoted_status { get; set; }

		private string Text
		{
			get
			{
				return full_text ?? text;
			}
		}

		string m_resolvedText;
		public string ResolvedText
		{
			get
			{
				if (string.IsNullOrEmpty(m_resolvedText))
				{
					if (extended_tweet != null)
						m_resolvedText = extended_tweet.ResolvedText;
					else if (quoted_status != null)
						m_resolvedText = $"{Text}{Environment.NewLine}\"{quoted_status.ResolvedText}\"";
					else if (retweeted_status != null)
						m_resolvedText = retweeted_status.ResolvedText;
					else
						m_resolvedText = Text;
				}

				return m_resolvedText;
			}
		}

		public override string ToString()
		{
			return $"{user.name} (@{user.screen_name}): {ResolvedText}";
		}
	}
}
