using System.IO;
using System.Runtime.Serialization.Json;

namespace Sebagomez.ShelltwitLib.Entities
{
	public class Media
	{
		public long media_id { get; set; }
		public string media_id_string { get; set; }
		public int size { get; set; }
		public int expires_after_secs { get; set; }
		public Image image { get; set; }
	}

	public class Image
	{
		public string image_type { get; set; }
		public int w { get; set; }
		public int h { get; set; }
	}
}
