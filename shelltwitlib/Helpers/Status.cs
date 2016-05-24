using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;

namespace shelltwitlib.Helpers
{
	[XmlRoot("statuses")]
	public class StatusSerializer : IEnumerable
	{
		public StatusSerializer()
		{
			Statuses = new Statuses();
		}

		[XmlElement("status")]
		public Statuses Statuses { get; set; }

		public IEnumerator GetEnumerator()
		{
			return Statuses.GetEnumerator();
		}

		public void Add(object item)
		{
			if (item is Status)
				Statuses.Add(item as Status);
		}
	}

	[CollectionDataContract]
	public class Statuses : List<Status> { }

	[DataContract]
	public class Status
	{
		[XmlElement("id")]
		[DataMember(Name = "id")]
		public string Id { get; set; }

		string m_innerText;

		[XmlElement("text")]
		[DataMember(Name = "text")]
		public string Text
		{
			get { return HttpUtility.HtmlDecode(m_innerText); }
			set { m_innerText = value; }
		}


		[XmlElement("user", typeof(User))]
		[DataMember(Name = "user")]
		public User User { get; set; }

		public override string ToString()
		{
			return Text;
		}
	}

	[DataContract]
	public class User
	{
		[XmlElement("id")]
		[DataMember(Name = "id")]
		public string Id { get; set; }
		[XmlElement("name")]
		[DataMember(Name = "name")]
		public string Name { get; set; }
		[XmlElement("screen_name")]
		[DataMember(Name = "screen_name")]
		public string ScreenName { get; set; }

		public override string ToString()
		{
			return ScreenName;
		}
	}
}
