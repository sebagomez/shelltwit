using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.ShelltwitLib.API.Options
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
				m_parameters.Add("status", Status);
				if (!string.IsNullOrEmpty(ReplyId))
					m_parameters.Add("in_reply_to_status_id", ReplyId);
				if (HasMedia)
					m_parameters.Add("media_ids", Util.EncodeString(string.Join(",", MediaIds.ToArray())));
			}

			return m_parameters;
		}
	}
}
