using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using shelltwitlib.API.OAuth;
using shelltwitlib.API.Options;
using shelltwitlib.Helpers;
using shelltwitlib.Web;

namespace shelltwitlib.API.Tweets
{
	public class Update
	{
		const string UPDATE_STATUS = "https://api.twitter.com/1.1/statuses/update.json";
		const string MEDIA_UPLOAD = "https://upload.twitter.com/1.1/media/upload.json";
		const string STATUS = "status";
		const string MEDIA = "media_ids";

		static Action<string> s_messageFunction;

		#region Update Status

		public static string UpdateStatus(string status)
		{
			return UpdateStatus(new UpdateOptions { Status = status });
		}

		public static void SetMessageAction(Action<string> func)
		{
			s_messageFunction = func;
		}

		static void WriteMessage(string message)
		{
			s_messageFunction?.Invoke(message);
		}

		public static string UpdateStatus(UpdateOptions options)
		{
			try
			{
				Regex regex = new Regex(@"\[(.*?)\]");
				foreach (System.Text.RegularExpressions.Match match in regex.Matches(options.Status))
				{
					options.Status = options.Status.Replace(match.Value, "");
					FileInfo file = new FileInfo(match.Value.Replace("[", "").Replace("]", ""));
					if (!file.Exists)
						throw new FileNotFoundException("File not found", file.FullName);
					options.MediaFiles.Add(file);
				}

				if (options.MediaFiles.Count > 4) //limited by the twitter API
					throw new ArgumentOutOfRangeException("media", "Up to 4 media files are allowed per tweet");

				if (options.User == null)
					options.User = AuthenticatedUser.LoadCredentials();
				options.Status = Util.EncodeString(HttpUtility.HtmlDecode(options.Status));

				if (options.HasMedia)
					UploadMedia(options);

				return InternalUpdateStatus(options);
			}
			catch (Exception ex)
			{
				return Util.ExceptionMessage(ex);
			}
		}

		private static string InternalUpdateStatus(UpdateOptions options)
		{
			string media = null;
			if (options.HasMedia)
				media = Util.EncodeString( string.Join(",", options.MediaIds.ToArray()));

			HttpWebRequest req = OAuthHelper.GetRequest(HttpMethod.POST, UPDATE_STATUS, options);

			byte[] bytes;
			using (Stream str = req.GetRequestStream())
			{
				bytes = Util.GetUTF8EncodingBytes($"{STATUS}={options.Status}");
				str.Write(bytes, 0, bytes.Length);

				if (options.HasMedia)
				{
					bytes = Util.GetUTF8EncodingBytes($"&{MEDIA}={media}");
					str.Write(bytes, 0, bytes.Length);
				}
			}

			req.PreAuthenticate = true;
			req.AllowWriteStreamBuffering = true;

			HttpWebResponse response = (HttpWebResponse)req.GetResponse();

			return response.StatusDescription;
		}

		private static void UploadMedia(UpdateOptions options)
		{
			foreach (FileInfo file in options.MediaFiles)
			{
				WriteMessage($"Uploading {file.Name}");
				HttpWebRequest req = OAuthHelper.GetRequest(HttpMethod.POST, MEDIA_UPLOAD, options, true);

				req.ContentType = Constants.CONTENT_TYPE.FORM_DATA;

				string boundary = GetMultipartBoundary(),
					separator = "--" + boundary,
					footer = separator + "--",
					shortFileName = file.Name,
					fileContentType = GetMimeType(shortFileName),
					fileHeader = string.Format("Content-Disposition: form-data; name=\"media\"; filename=\"{0}\"", shortFileName),
					contentType = "Content-Type: application/octet-stream";

				var encoding = System.Text.Encoding.UTF8;

				req.KeepAlive = false;
				req.ContentType = string.Format("multipart/form-data, boundary=\"{0}\"", boundary);

				string newLine = "\r\n";
				byte[] bytes;
				using (var s = req.GetRequestStream())
				{
					bytes = encoding.GetBytes(separator);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(fileHeader);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(contentType);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = File.ReadAllBytes(file.FullName);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(footer);
					s.Write(bytes, 0, bytes.Length);

					bytes = encoding.GetBytes(newLine);
					s.Write(bytes, 0, bytes.Length);

				}

				req.PreAuthenticate = true;
				req.AllowWriteStreamBuffering = true;

				HttpWebResponse response = (HttpWebResponse)req.GetResponse();

				Media media = Media.FromStream(response.GetResponseStream());

				options.MediaIds.Add(media.IdString);
			}
		}

		private static string GetMultipartBoundary()
		{
			return "======" +
				Guid.NewGuid().ToString().Substring(18).Replace("-", "") +
				"======";
		}

		private static string GetMimeType(String filename)
		{
			var extension = System.IO.Path.GetExtension(filename).ToLower();
			var regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension);

			string result =
				((regKey != null) && (regKey.GetValue("Content Type") != null))
				? regKey.GetValue("Content Type").ToString()
				: "image/unknown";
			return result;
		}

		#endregion
	}
}
