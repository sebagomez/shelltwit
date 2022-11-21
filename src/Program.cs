using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sebagomez.Shelltwit.Misc;
using Sebagomez.Shelltwit.Security;
using Sebagomez.TwitterLib.API.Tweets;
using Sebagomez.TwitterLib.Entities;

namespace Sebagomez.Shelltwit
{
	class Program
	{
		static async Task Main(string[] args)
		{
			try
			{
				Console.OutputEncoding = new UTF8Encoding();

				AppDomain.CurrentDomain.ProcessExit += (s, a) => Console.ResetColor();

				PrintHeader();

				BaseAPI.SetMessageAction(message =>
				{
					PrintActions.Print(message);
				});

				Option o = Option.GetOption(args);
				if (o == null)
					throw new Exception($"Invalid flags:{string.Join(" ",args)}");

				if (o.Short == "c")
					o.Action(null, args); //No authentication needed
				else
					o.Action(await CredentialsManager.LoadCredentials(), args);
			}
			catch (WebException wex)
			{
				PrintActions.PrintError(wex.Message);

				HttpWebResponse res = (HttpWebResponse)wex.Response;
				if (res != null)
				{
					UpdateError errors = TwitterLib.Helpers.Util.Deserialize<UpdateError>(res.GetResponseStream());
					errors.errors.ForEach(e => PrintActions.PrintError(e.ToString()));
				}
			}
			catch (Exception ex)
			{
				PrintException(ex);
				System.Environment.Exit(1);
			}

			System.Environment.Exit(0);
		}

		private static void PrintException(Exception ex)
		{
			PrintActions.PrintError(ex.Message);
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

			string build = System.Environment.GetEnvironmentVariable("APPBUILD");
			if (!string.IsNullOrEmpty(build))
				build = $"-{build}";

			PrintActions.Print($"🐤 {title} version {version}{build}");
			PrintActions.Print(copyRight);
			PrintActions.Print("");
		}
	}
}
