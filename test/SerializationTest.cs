using System;
using System.IO;
using System.Runtime.Serialization.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace shelltwit_tester
{
	[TestClass]
	public class SerializationTest : BaseTests
	{
		[TestMethod]
		public void DeserializeStatuses()
		{
			try
			{
				string path = Path.Combine(AppContext.BaseDirectory, "serialized.json");

				Sebagomez.ShelltwitLib.Entities.Statuses ss = null;

				DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Statuses));
				using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
					ss = (Statuses)serializer.ReadObject(file); ;

				Assert.IsTrue(ss.Count == 2, "Failed to load every status");
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}

		[TestMethod]
		public void SerializeStatuses()
		{
			try
			{
				string path = Path.Combine(AppContext.BaseDirectory, "serialized.json");

				Sebagomez.ShelltwitLib.Entities.Statuses ss = new Sebagomez.ShelltwitLib.Entities.Statuses();
				ss.Add(new Sebagomez.ShelltwitLib.Entities.Status() { id = 1, text = "One", user = new Sebagomez.ShelltwitLib.Entities.User { id = 1, name = "User1", screen_name = "screenName1" } });
				ss.Add(new Sebagomez.ShelltwitLib.Entities.Status() { id = 2, text = "Two", user = new Sebagomez.ShelltwitLib.Entities.User { id = 2, name = "User2", screen_name = "ScreenName2" } });


				Util.Serialize(ss, path);

				//DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Statuses));
				//using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
				//	serializer.WriteObject(file, ss);
			}
			catch (Exception ex)
			{
				Assert.Fail(Util.ExceptionMessage(ex));
			}
		}
	}
}
