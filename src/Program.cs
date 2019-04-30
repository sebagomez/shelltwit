﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Sebagomez.TwitterLib.API.Tweets;
using Sebagomez.TwitterLib.Entities;
using Sebagomez.TwitterLib.Helpers;

namespace Sebagomez.Shelltwit
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				Console.OutputEncoding = new UTF8Encoding();

				AppDomain.CurrentDomain.ProcessExit += (s, a) => Console.ResetColor();

				PrintHeader();

				BaseAPI.SetMessageAction(message =>
				{
					Util.Print(message);
				});

				Option o = Option.GetOption(args);
				if (o == null)
					throw new Exception("Invalid flag");

				o.Action(AuthenticatedUser.CurrentUser, args);
			}
			catch (WebException wex)
			{
				Util.PrintError(wex.Message);

				HttpWebResponse res = (HttpWebResponse)wex.Response;
				if (res != null)
				{
					UpdateError errors = TwitterLib.Helpers.Util.Deserialize<UpdateError>(res.GetResponseStream());
					errors.errors.ForEach(e => Util.PrintError(e.ToString()));
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
					Util.PrintWarning("Press <enter> to exit...");
					Console.ReadLine();
				}
#endif
			}

			Environment.Exit(0);
		}


		private static void PrintException(Exception ex)
		{
			Util.PrintError(ex.Message);
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

			Util.Print($"{title} version {version}{build} running on {System.Runtime.InteropServices.RuntimeInformation.OSDescription}");
			Util.Print(copyRight);
			Util.Print("");
		}
	}
}