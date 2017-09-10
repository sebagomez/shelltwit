using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace Sebagomez.ShelltwitLib.Helpers
{
	public class Settings
	{
		static string s_settingsFile = "twit.data";

		static Settings s_instance;

		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
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
						//throw new Exception("Missing configuration file");
				}

				return s_instance;
			}
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
					return (Settings)jsonSerializer.ReadObject(file);
				}
			}
			catch
			{
				return null;
			}
		}
	}
}
