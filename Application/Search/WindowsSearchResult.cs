using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Search {
	
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
