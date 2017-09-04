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
		const string CLEAR = "/c";
		const string SEARCH = "/q";
		const string TIME_LINE = "/tl";
		const string HELP = "/?";
		const string MENTIONS = "/m";
		const string USER = "/u";
		const string LIKES = "/l";
		const string STREAMING = "/s";

		static void Main(string[] args)
		{
			try
			{
				//Debug.Assert(false, "Attach VS here!");

				Console.OutputEncoding = new UTF8Encoding();

				PrintHeader();

				BaseAPI.SetMessageAction(message => Console.WriteLine(message));

				if (args.Length == 0)
				{
					PrintTwits(Timeline.GetTimeline().Result);
					return;
				}


				if (args[0].StartsWith("/"))
				{
					string flag = args[0].ToLower().Trim();
					switch (flag)
					{
						case CLEAR:
							AuthenticatedUser.ClearCredentials();
							Console.WriteLine("User credentials cleared!");
							return;
						case HELP:
							ShowUsage();
							return;
						case TIME_LINE:
							PrintTwits(Timeline.GetTimeline().Result);
							return;
						case MENTIONS:
							PrintTwits(Mentions.GetMentions().Result);
							return;
						case SEARCH:
							SearchOptions options = new SearchOptions { Query = string.Join(" ", args.Skip(1)), User = AuthenticatedUser.LoadCredentials() };
							PrintTwits(Search.SearchTweets(options).Result);
							return;
						case LIKES:
							PrintTwits(Likes.GetUserLikes(new LikesOptions()).Result);
							return;
						case USER:
							if (args.Length != 2)
								throw new ArgumentNullException("screenname","The user' screen name must be provided");
							UserTimelineOptions usrOptions = new UserTimelineOptions { ScreenName = args[1] };
							PrintTwits(UserTimeline.GetUserTimeline(usrOptions).Result);
							return;
						case STREAMING:
							if (args.Length == 1)
								throw new ArgumentNullException("streaming", "The track must be provided");
							string track = string.Join(" ", args.Skip(1));
							StreammingFilterOptions streamingOptions = new StreammingFilterOptions { Track = track };
							Console.WriteLine($"Starting live streaming for '{track}', press ctrl+c to quit");
							foreach (Status s in StreamingFilter.GetStreamingTimeline(streamingOptions))
								PrintTwit(s);
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

				string response = Update.UpdateStatus(string.Join(" ", args)).Result;

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

		static void PrintTwits(List<Status> twits)
		{
			if (twits == null)
				Console.WriteLine("No twits :(");
			else
				twits.ForEach(twit => PrintTwit(twit));
		}

		static void PrintTwit(Status twit)
		{
			Console.WriteLine($"{twit.user.name} (@{twit.user.screen_name}): {twit.text}");
		}

		static void PrintTwits(SearchResult results)
		{
			if (results.statuses.Length == 0)
				Console.WriteLine("Sorry, no tweets found :(");
			else
				PrintTwits(results.statuses.ToList<Status>());
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
			Console.WriteLine("Usage: twit /q <query>|/c|/tl|/m|/l|/s <track>|/u <user>|/?|<status> [<mediaPath>]");
			Console.WriteLine("");
			Console.WriteLine("/c 		: clears user stored credentials");
			Console.WriteLine("/tl 		: show user's timeline (default)");
			Console.WriteLine("/q 		: query twits containing words");
			Console.WriteLine("/m 		: show user's mentions");
			Console.WriteLine("/u user		: show another user's timeline");
			Console.WriteLine("/s track	: live status with a specific track");
			Console.WriteLine("/l		: user's likes (fka favorites)");
			Console.WriteLine("/? 		: show this help");
			Console.WriteLine("status	 	: status to update at twitter.com");
			Console.WriteLine("mediaPath	: full path, between brackets, to the media files (up to four) to upload.");
			Console.WriteLine("");
		}
	}
}
