using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace shelltwitlib.API.Options
{
	public class UpdateOptions : TwitterOptions
	{
		public string Status { get; set; }
		public string ReplyId { get; set; }
		public List<string> MediaIds { get; set; }
		public List<FileInfo> MediaFiles { get; set; }

		public UpdateOptions()
		{
			MediaIds = new List<string>();
			MediaFiles = new List<FileInfo>();
		}

		public bool HasMedia
		{
			get { return MediaFiles.Count > 0; }
		}

		Dictionary<string, string> m_parameters = null;
		public override Dictionary<string, string> GetParameters()
		{
			if (m_parameters == null)
			{
				m_parameters = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(ReplyId))
					m_parameters.Add("in_reply_to_status_id", ReplyId);
			}

			return m_parameters;
		}
	}
}
