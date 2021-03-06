using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

using Lumen.Scripting;

namespace Lumen.Scripting.Inspecting {

	public class ObjectInspector : IDisposable {
		private object _object;
		private IDispatch _dispatch;
		private IntPtr _pTypeAttr = IntPtr.Zero;
		private System.Runtime.InteropServices.ComTypes.ITypeInfo _typeInfo;
		private string _typeName, _typeDescription, _typeHelpFile;
		private int _typeHelpContext;
		private ComTypeLibrary _comTypeLibrary;

		public ObjectInspector(object comObject) {
			System.Runtime.InteropServices.ComTypes.ITypeLib ppTLB = null;
			int pIndex = 0;

			_dispatch = comObject as IDispatch;


			if (_dispatch != null) {
				_object = comObject;
				_typeInfo = _dispatch.GetTypeInfo(0, 0);
				_typeInfo.GetTypeAttr(out _pTypeAttr);
				_typeInfo.GetDocumentation(-1,
												 out _typeName,
												 out _typeDescription,
												 out _typeHelpContext,
												 out _typeHelpFile);
				_typeInfo.GetContainingTypeLib(out ppTLB, out pIndex);
				_comTypeLibrary = new ComTypeLibrary(ppTLB);
			}
			else {
				throw new InvalidComObjectException();
			}
		}

		~ObjectInspector() {
			Dispose();
		}

		public void Dispose() {
			try {
				if (_typeInfo != null) {
					_typeInfo.ReleaseTypeAttr(_pTypeAttr);
					Marshal.FinalReleaseComObject(_typeInfo);
				}

				// The _object and _dispatch are COM objects. Because they were
				// not created by this class, they must not be Released by this
				// Dispose() method.  If they DO get released, then if the actual
				// owner of the object tries to use it later, the dreaded "COM
				// object that has been separated from its underlying RCW cannot
				// be used" exception will occur.
				//
				// if (_object != null)
				// {
				//     Marshal.ReleaseComObject(_object);
				//     _object = null;
				// }
				// if (_dispatch != null)
				// {
				//     Marshal.ReleaseComObject(_dispatch);
				//     _dispatch = null;
				// }
			}
			catch {
			}
		}

		public List<String> GetPropertyNames() {
			var list = new List<String>();
			var add = new Action<String, Int32>((s, ignored) => { list.Add(s); });
			GetElementNames(System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_PROPERTYGET, add);
			return list;
		}

		public Dictionary<String, Int32> GetMethods() {
			var dict = new Dictionary<String, Int32>();
			var add = new Action<String, Int32>((s, i) => { dict.Add(s, i); });
			GetElementNames(System.Runtime.InteropServices.ComTypes.INVOKEKIND.INVOKE_FUNC, add);
			return dict;
		}

		public List<String> GetNames() {
			var list = new List<String>();

			for (int i = 0; i < this.TypeAttr.cVars; i++) {
				var pDesc = new IntPtr(0);
				String[] names = new String[100];
				int pcNames;

				this._typeInfo.GetVarDesc(i, out pDesc);

				var varDesc = (System.Runtime.InteropServices.ComTypes.VARDESC)Marshal.PtrToStructure(pDesc, typeof(System.Runtime.InteropServices.ComTypes.VARDESC));

				this._typeInfo.ReleaseVarDesc(pDesc);

				this._typeInfo.GetNames(varDesc.memid, names, 1, out pcNames);

				String name = names[0];

				list.Add(name);
			}

			return list;
		}

		public T GetValue<T>(String name) {
			object res = _dispatch.GetType().InvokeMember(name, System.Reflection.BindingFlags.GetProperty, null, _dispatch, null);

			return res.Convert<T>();
		}

		public List<object> GetList() {
			List<object> result = new List<object>();
			List<String> names = GetNames();

			foreach (var name in names) {
				var value = GetValue<object>(name);

				if (value.GetType().GUID.Equals(Guid.Empty)) {
					using (var inspector = new ObjectInspector(value)) {
						var valueNames = inspector.GetNames();
						object newValue = null;

						if (valueNames.Count > 0) {
							if (valueNames[0] == "0") {
								newValue = inspector.GetList();
							}
							else {
								newValue = inspector.GetHash();
							}
							result.Add(newValue);
						}
					}
				}
				else {
					result.Add(value);
				}

			}

			return result;
		}

		public Hashtable GetHash() {
			Hashtable result = new Hashtable();
			List<String> names = GetNames();

			foreach (var name in names) {
				var value = GetValue<object>(name);

				if (value.GetType().GUID.Equals(Guid.Empty)) {
					using (var inspector = new ObjectInspector(value)) {
						var valueNames = inspector.GetNames();
						object newValue = null;

						if (valueNames.Count > 0) {
							if (valueNames[0] == "0") {
								newValue = inspector.GetList();
							}
							else {
								newValue = inspector.GetHash();
							}
							result.Add(name, newValue);
						}
					}
				}
				else {
					result.Add(name, value);
				}
			}

			return result;
		}

		public void GetElementNames(System.Runtime.InteropServices.ComTypes.INVOKEKIND kind, Action<String, Int32> add) {
			try {
				for (int i = 0; i < this.TypeAttr.cFuncs; i++) {
					IntPtr pFuncDesc = IntPtr.Zero;
					System.Runtime.InteropServices.ComTypes.FUNCDESC funcDesc;
					string strName, strDocString, strHelpFile;
					int dwHelpContext;

					_typeInfo.GetFuncDesc(i, out pFuncDesc);
					funcDesc = (System.Runtime.InteropServices.ComTypes.FUNCDESC)
						Marshal.PtrToStructure(pFuncDesc, typeof(System.Runtime.InteropServices.ComTypes.FUNCDESC));

					if (funcDesc.invkind == kind) {
						_typeInfo.GetDocumentation(funcDesc.memid, out strName, out strDocString, out dwHelpContext, out strHelpFile);
						add(strName, funcDesc.cParams);
					}
				}
			}
			catch (System.Exception ex) {
				throw ex;
			}
		}

		public string TypeName { get { return this._typeName; } }
		public string TypeFullName { get { return this.ComTypeLibrary.Name + "." + this._typeName; } }
		public string TypeDescription { get { return this._typeDescription; } }
		public int TypeHelpContext { get { return this._typeHelpContext; } }
		public string TypeHelpFile { get { return this._typeHelpFile; } }
		public object WrappedComObject { get { return _dispatch; } }
		public ComTypeLibrary ComTypeLibrary { get { return _comTypeLibrary; } }

		public System.Runtime.InteropServices.ComTypes.TYPEATTR TypeAttr {
			get {
				return (System.Runtime.InteropServices.ComTypes.TYPEATTR)
					Marshal.PtrToStructure(_pTypeAttr,
											 typeof(System.Runtime.InteropServices.ComTypes.TYPEATTR));
			}
		}

		public string TypeVersion {
			get {
				string version = String.Empty;
				try {
					version = TypeAttr.wMajorVerNum.ToString() + "." + TypeAttr.wMinorVerNum.ToString();
				}
				catch {
				}
				return version;
			}
		}
	}
}
