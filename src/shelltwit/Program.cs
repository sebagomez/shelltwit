using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.Shelltwit
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Console.OutputEncoding = new UTF8Encoding();

				PrintHeader();

				BaseAPI.SetMessageAction(message => Console.WriteLine(message));

				Option o = Option.GetOption(args);
				if (o == null)
					throw new Exception("Invalid flag");

				o.Action(AuthenticatedUser.CurrentUser, args);
			}
			catch (WebException wex)
			{
				Console.WriteLine(wex.Message);

				HttpWebResponse res = (HttpWebResponse)wex.Response;
				if (res != null)
				{
					UpdateError errors = ShelltwitLib.Helpers.Util.Deserialize<UpdateError>(res.GetResponseStream());
					errors.errors.ForEach(e => Console.WriteLine(e.ToString()));
				}
			}
			catch (Exception ex)
			{
				PrintException(ex);
			}
			finally
			{
#if DEBUG
				if (Debugger.IsAttached)
				{
					Console.WriteLine("Press <enter> to exit...");
					Console.ReadLine();

				}
#endif
			}

			Environment.Exit(0);
		}


		private static void PrintException(Exception ex)
		{
			Console.WriteLine(ex.Message);
			Exception inner = ex.InnerException;
			while (inner != null)
			{
				PrintException(inner);
				inner = inner.InnerException;
			}
		}

		static void PrintHeader()
		{
			Assembly assembly = Assembly.GetEntryAssembly();
			IEnumerable<Attribute> assemblyAtt = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute));
			IEnumerable<Attribute> assemblyCop = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute));
			string title = ((AssemblyTitleAttribute)assemblyAtt.First()).Title;
			string copyRight = ((AssemblyCopyrightAttribute)assemblyCop.First()).Copyright;
			string version = assembly.GetName().Version.ToString();

			Console.WriteLine($"{title} version {version} for {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
			Console.WriteLine(copyRight);
			Console.WriteLine("");
		}
	}
}
