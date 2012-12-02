using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Extensions {
	public class ExtensionManager : List<Extension> {

		private static ExtensionManager _current = new ExtensionManager();

		public static ExtensionManager Current { get { return _current; } }

	}
}
