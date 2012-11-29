using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Search {

	[Flags]
	public enum WindowsSearchKind {
		// KIND_CALENDAR
		calendar = 1,
		// KIND_COMMUNICATION
		communication = 2,
		// KIND_CONTACT
		contact = 4,
		// KIND_DOCUMENT
		document = 8,
		// KIND_EMAIL
		email = 16,
		// KIND_FEED
		feed = 32,
		// KIND_FOLDER
		folder = 64,
		// KIND_GAME
		game = 128,
		// KIND_INSTANTMESSAGE
		instantmessage = 256,
		// KIND_JOURNAL
		journal = 512,
		// KIND_LINK
		link = 1024,
		// KIND_MOVIE
		movie = 2048,
		// KIND_MUSIC
		music = 4096,
		// KIND_NOTE
		note = 8192,
		// KIND_PICTURE
		picture = 16384,
		// KIND_PROGRAM
		program = 32768,
		// KIND_RECORDEDTV
		recordedtv = 131072,
		// KIND_SEARCHFOLDER
		searchfolder = 262144,
		// KIND_TASK
		task = 524288,
		// KIND_VIDEO
		video = 1048576,
		// KIND_WEBHISTORY
		webhistory = 2097152,

		// other
		file = 4194304
	}

	public class WindowsSearchResult {

		public String FileName { get; set; }
		public String FilePath { get; set; }
		public WindowsSearchKind Kind { get; set; }
		public int Rank { get; set; }

		public bool Touched { get; set; }

		public WindowsSearchResult() {

		}

	}
}
