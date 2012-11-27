using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Lumen.Scripting.Inspecting {

	public class ComTypeLibrary {

		private System.Runtime.InteropServices.ComTypes.ITypeLib _typeLib;
		IntPtr _pTypeLibAttr = IntPtr.Zero;
		string _Name, _Description, _HelpFile;
		int _HelpContext;

		public ComTypeLibrary(System.Runtime.InteropServices.ComTypes.ITypeLib typeLib) {
			_typeLib = typeLib;
			_typeLib.GetLibAttr(out _pTypeLibAttr);
			_typeLib.GetDocumentation(-1, out _Name, out _Description, out _HelpContext, out _HelpFile);
		}

		public string Name { get { return this._Name; } }
		public string Description { get { return this._Description; } }
		public int HelpContext { get { return this._HelpContext; } }
		public string HelpFile { get { return this._HelpFile; } }

		public System.Runtime.InteropServices.ComTypes.TYPELIBATTR TypeLibAttr {
			get {
				return (System.Runtime.InteropServices.ComTypes.TYPELIBATTR)
					Marshal.PtrToStructure(_pTypeLibAttr, typeof(System.Runtime.InteropServices.ComTypes.TYPELIBATTR));
			}
		}

		public string TypeLibVersion {
			get {
				string version = String.Empty;
				try {
					version = TypeLibAttr.wMajorVerNum.ToString() + "." + TypeLibAttr.wMinorVerNum.ToString();
				}
				catch {
				}
				return version;
			}
		}

		public Version Version {
			get {
				return new Version(TypeLibAttr.wMajorVerNum, TypeLibAttr.wMinorVerNum, 0, 0);
			}
		}

		public override int GetHashCode() {
			return this.TypeLibAttr.guid.GetHashCode() +
				this.TypeLibAttr.wMajorVerNum.GetHashCode() +
				this.TypeLibAttr.wMinorVerNum.GetHashCode();
		}

		public override bool Equals(object obj) {
			ComTypeLibrary comTypeLibrary = obj as ComTypeLibrary;
			if (comTypeLibrary != null) {
				if (this.TypeLibAttr.guid.Equals(comTypeLibrary.TypeLibAttr.guid)) {
					if (this.TypeLibAttr.wMajorVerNum.Equals(comTypeLibrary.TypeLibAttr.wMajorVerNum)) {
						if (this.TypeLibAttr.wMinorVerNum.Equals(comTypeLibrary.TypeLibAttr.wMinorVerNum)) {
							return true;
						}
					}
				}
				return base.Equals(obj);
			}
			else {
				return base.Equals(obj);
			}
		}

		public override string ToString() {
			return this.Name + " - " + this.Description;
		}
	}
}

