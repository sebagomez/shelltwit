using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace shelltwitlib.Helpers
{
	[DataContract]
	internal class Media
	{
		[DataMember(Name= "media_id")]
		public long Id { get; set; }
		[DataMember(Name= "media_id_string")]
		public string IdString { get; set; }
		[DataMember(Name = "size")]
		public int Size { get; set; }
		[DataMember(Name = "expires_after_secs")]
		public int Expires { get; set; }
		[DataMember(Name = "image")]
		public Image Image { get; set; }

		public static Media FromStream(Stream stream)
		{
			DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Media));
			return (Media)jsonSerializer.ReadObject(stream);
		}
	}

	[DataContract]
	internal class Image
	{
		[DataMember(Name = "image_type")]
		public string Type { get; set; }
		[DataMember(Name = "w")]
		public int Width { get; set; }
		[DataMember(Name = "h")]
		public int Height { get; set; }
	}
}
