using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;

namespace Sebagomez.ShelltwitLib.Helpers
{
	public class Settings
	{
		static string s_settingsFile = "twit.data";
		static string s_baseDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;

		static Settings s_instance;

		public string ConsumerKey { get; set; }
		public string ConsumerSecret { get; set; }
		public string RefFile { get; set; }

		public static Settings Instance
		{
			get { return s_instance; }
		}

		static Settings()
		{
			try
			{
				s_instance = GetData(Path.Combine(s_baseDirectory, s_settingsFile));
				while (s_instance != null && string.IsNullOrEmpty(s_instance.ConsumerKey) && string.IsNullOrEmpty(s_instance.ConsumerSecret) && !string.IsNullOrEmpty(s_instance.RefFile) && File.Exists(Path.Combine( s_baseDirectory,s_instance.RefFile)))
					s_instance = GetData(Path.Combine(s_baseDirectory, s_instance.RefFile));
			}
			catch { }
		}

		static Settings GetData(string fileName)
		{
			using (FileStream file = File.Open(fileName, FileMode.Open))
			{
				DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Settings));
				return (Settings)jsonSerializer.ReadObject(file);
			}
		}
	}
}
