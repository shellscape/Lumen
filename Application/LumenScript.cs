using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lumen.Extensions;

namespace Lumen {
	
	/// <summary>
	/// A script object class passed into each extension.
	/// The methods and properties here will be camel cased for javascript.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(true)]
	public class LumenScript {

		private Extension _extension = null;

		public LumenScript(Extension extension) {
			_extension = extension;
		}

		public String extensionName { set { _extension.Name = value; } }

		public void require(String what) {
			_extension.Require(what);
		}

		public void alert(String message) {
			System.Windows.MessageBox.Show(message);
		}

		public void run(String what, String parameters) {
			System.Windows.MessageBox.Show("run\n\nwhat: " + what + "\n\nparams: " + parameters);
		}
	}
}
