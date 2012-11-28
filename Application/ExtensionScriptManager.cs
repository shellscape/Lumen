using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen {
	public class ExtensionScriptManager : List<ExtensionScript> {

		public ExtensionScript FromLineNumber(int lineNumber) {
			return this.Where(s => lineNumber >= s.StartPosition && lineNumber <= (s.StartPosition + s.Length)).FirstOrDefault();
		}

	}
}
