using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen {
	public class ExtensionScript {

		public String Name { get; set; }
		public int StartPosition { get; set; }
		public int Length { get; set; }

		/// <summary>
		/// Translates a global line number (for errors) to the local script line number.
		/// </summary>
		/// <param name="lineNumber"></param>
		/// <returns></returns>
		public int TranslateLineNumber(int lineNumber) {
			return lineNumber - StartPosition;
		}

	}
}
