using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace shelltwitlib.Helpers
{
	[DataContract]
	public class Error
	{
		[DataMember(Name="code")]
		public int Code { get; set; }

		[DataMember(Name = "message")]
		public string Message { get; set; }

		public override string ToString()
		{
			return $"{Code}:{Message}";
		}
	}

	[DataContract]
	public class UpdateError
	{
		[DataMember(Name = "errors")]
		public List<Error> Errors { get; set; }

		public static UpdateError GetFromStream(Stream stream)
		{
			UpdateError errors = new UpdateError();
			DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UpdateError));
			errors = (UpdateError)serializer.ReadObject(stream);

			return errors;
		}
	}
}
