using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class MentionsOptions : TwitterOptions
	{
		public string Since { get; set; }

		Dictionary<string, string> m_parameters = null;

		public override Dictionary<string, string> GetParameters()
		{
			if (m_parameters == null)
			{
				m_parameters = new Dictionary<string, string>();
				if (!string.IsNullOrEmpty(Since))
					m_parameters.Add("since_id", Since);
			}

			return m_parameters;
		}
	}
}
