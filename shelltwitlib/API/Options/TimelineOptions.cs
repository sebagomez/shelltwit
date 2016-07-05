using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sebagomez.ShelltwitLib.API.Options
{
	public class TimelineOptions : TwitterOptions
	{
		public int SinceId { get; set; }
		Dictionary<string, string> m_parameters = null;
		public override Dictionary<string, string> GetParameters()
		{
			if (m_parameters == null)
			{
				m_parameters = new Dictionary<string, string>();
				if (SinceId != 0)
					m_parameters.Add("since_id", SinceId.ToString());
			}

			return m_parameters;
		}
	}
}
