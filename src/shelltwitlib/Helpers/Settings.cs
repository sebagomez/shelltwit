using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Sebagomez.ShelltwitLib.Helpers
{
	[DataContract]
	public class Settings
	{
		static string s_settingsFile = "twit.data";

		public static string TWIT_KEY = "TWIT_KEY";
		public static string TWIT_SECRET = "TWIT_SECRET";

		static Settings s_instance;

		[DataMember]
		public string ConsumerKey { get; set; }
		[DataMember]
		public string ConsumerSecret { get; set; }
		[DataMember]
		public string RefFile { get; set; }

		public static Settings Instance
		{
			get
			{
				if (s_instance == null)
				{
					s_instance = GetData(GetFullPath(s_settingsFile));
					while (s_instance != null && string.IsNullOrEmpty(s_instance.ConsumerKey) && string.IsNullOrEmpty(s_instance.ConsumerSecret) && !string.IsNullOrEmpty(s_instance.RefFile) && File.Exists(GetFullPath(s_instance.RefFile)))
						s_instance = GetData(GetFullPath(s_instance.RefFile));

					if (s_instance == null)
						s_instance = new Settings();
				}

				return s_instance;
			}
		}

		public Settings()
		{
			ConsumerKey = Environment.GetEnvironmentVariable(TWIT_KEY);
			ConsumerSecret = Environment.GetEnvironmentVariable(TWIT_SECRET);
		}

		public static void NewSettings(string key, string secret)
		{
			Settings s = new Settings { ConsumerKey = key, ConsumerSecret = secret };
			Util.Serialize(s, GetFullPath(s_settingsFile));
		}

		static string GetFullPath(string fileName)
		{
			return Path.Combine(AppContext.BaseDirectory, fileName);
		}

		static Settings GetData(string fileName)
		{
			try
			{
				using (FileStream file = File.Open(fileName, FileMode.Open))
				{
					DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Settings));
					Settings s = (Settings)jsonSerializer.ReadObject(file);
					return s;
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
