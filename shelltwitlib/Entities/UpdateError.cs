using System.Collections.Generic;

namespace Sebagomez.ShelltwitLib.Entities
{
	public class Error
	{
		public int code { get; set; }

		public string message { get; set; }

		public override string ToString()
		{
			return $"{code}:{message}";
		}
	}

	public class UpdateError
	{
		public List<Error> errors { get; set; }
	}
}
