using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Sebagomez.ShelltwitLib.API.Options;
using Sebagomez.ShelltwitLib.API.Tweets;
using Sebagomez.ShelltwitLib.Entities;
using Sebagomez.ShelltwitLib.Helpers;

namespace Sebagomez.Shelltwit
{
	class Program
	{
		const string CLEAR =		"c";
		const string SEARCH =		"q";
		const string TIME_LINE =	"tl";
		const string HELP =			"?";
		const string MENTIONS =		"m";
		const string USER =			"u";
		const string LIKES =		"l";
		const string STREAMING =	"s";
		const string STREAMING_TL =	"stl";

		static void Main(string[] args)
		{
			try
			{
				Console.OutputEncoding = new UTF8Encoding();

				PrintHeader();

				BaseAPI.SetMessageAction(message => Console.WriteLine(message));

				AuthenticatedUser user = AuthenticatedUser.LoadCredentials();

				if (args.Length == 0)
				{
					Util.UserTimeLine(user);
					return;
				}

				if (args[0].StartsWith("-") || args[0].StartsWith("/"))
				{
					string flag = args[0].Substring(1).ToLower().Trim();
					switch (flag)
					{
						case CLEAR:
							Util.ClearCredentials();
							return;
						case HELP:
							ShowUsage();
							return;
						case TIME_LINE:
							Util.UserTimeLine(user);
							return;
						case MENTIONS:
							Util.UserMentions(user);
							return;
						case SEARCH:
							Util.UserSearch(user, args);
							return;
						case LIKES:
							Util.UserLikes(user);
							return;
						case USER:
							Util.UserTwits(user, args);
							return;
						case STREAMING:
							Util.StreamingTrack(user, args);
							return;
						case STREAMING_TL:
							Util.StreamingTimeLine(user);
							return;
						default:
							Console.WriteLine($"Invalid flag: {flag}");
							ShowUsage();
							return;
					}
				}

				if (args[0].StartsWith("\\") || (args.Length == 1 && args[0].Length == 1))
				{
					Console.WriteLine($"Really? do you really wanna twit \"{string.Join(" ",args)}\"?{Environment.NewLine}[T]wit, or [N]o sorry, I messed up...");
					ConsoleKeyInfo input = Console.ReadKey();
					while (input.Key != ConsoleKey.T && input.Key != ConsoleKey.N)
					{
						Console.WriteLine();
						Console.WriteLine("[T]wit, or [N]o sorry, I messed up...");
						input = Console.ReadKey();
					}
					Console.WriteLine();

					if (input.Key == ConsoleKey.N)
					{
						Console.WriteLine("That's what I thought! ;)");
						return;
					}
				}

				UpdateOptions updOptions = new UpdateOptions { Status = string.Join(" ", args), User = user };
				string response = Update.UpdateStatus(updOptions).GetAwaiter().GetResult();

				if (response != "OK")
					Console.WriteLine($"Response was not OK: {response}");
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

		static void ShowUsage()
		{
			Console.WriteLine("Usage: twit -q <query>|-c|-tl|-m|-l|-s <track>|-stl|-u <user>|-?|<status> [<mediaPath>]");
			Console.WriteLine("");
			Console.WriteLine("-c 		: clears user stored credentials");
			Console.WriteLine("-tl 		: show user's timeline (default)");
			Console.WriteLine("-q 		: query twits containing words");
			Console.WriteLine("-m 		: show user's mentions");
			Console.WriteLine("-u user		: show another user's timeline");
			Console.WriteLine("-s track	: live status with a specific track");
			Console.WriteLine("-stl		: streamed user tl");
			Console.WriteLine("-l		: user's likes (fka favorites)");
			Console.WriteLine("-? 		: show this help");
			Console.WriteLine("status	 	: status to update at twitter.com");
			Console.WriteLine("mediaPath	: full path, between brackets, to the media files (up to four) to upload.");
			Console.WriteLine("");
		}
	}
}
