using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

	internal class ScriptSite : IActiveScriptSite {

		private const int TYPE_E_ELEMENTNOTFOUND = unchecked((int)(0x8002802B));

		internal ScriptException _lastException;
		internal Dictionary<string, object> _namedItems = new Dictionary<string, object>();

		void IActiveScriptSite.GetLCID(out int lcid) {
			lcid = Thread.CurrentThread.CurrentCulture.LCID;
		}

		void IActiveScriptSite.GetItemInfo(string name, ScriptInfo returnMask, out IntPtr item, IntPtr typeInfo) {
			if ((returnMask & ScriptInfo.ITypeInfo) == ScriptInfo.ITypeInfo)
				throw new NotImplementedException();

			object value;
			if (!_namedItems.TryGetValue(name, out value))
				throw new COMException(null, TYPE_E_ELEMENTNOTFOUND);

			item = Marshal.GetIUnknownForObject(value);
		}

		void IActiveScriptSite.OnScriptError(IActiveScriptError scriptError) {

			uint sourceContext;
			int lineNumber;
			int characterPosition;
			String message = "Script exception: {1}. Error number {0} (0x{0:X8}): {2} at line {3}, column {4}.";
			String sourceLine = null;
			System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo;

			try {
				scriptError.GetSourceLineText(out sourceLine);

				if (!String.IsNullOrEmpty(sourceLine)) {
					message += " Source line: '{5}'.";
				}
			}
			catch { // happens most of the time, but we should still try it.
			}

			scriptError.GetSourcePosition(out sourceContext, out lineNumber, out characterPosition);

			lineNumber++;
			characterPosition++;

			scriptError.GetExceptionInfo(out exceptionInfo);

			_lastException = new ScriptException(String.Format(message, exceptionInfo.scode, exceptionInfo.bstrSource, exceptionInfo.bstrDescription, lineNumber, characterPosition, sourceLine)) {
				Column = characterPosition,
				Description = exceptionInfo.bstrDescription,
				Line = lineNumber,
				Number = exceptionInfo.scode,
				Text = sourceLine
			};

		}

		void IActiveScriptSite.OnEnterScript() {
			_lastException = null;
		}

		void IActiveScriptSite.OnLeaveScript() {
		}

		#region .    IActiveScriptSite    .

		void IActiveScriptSite.GetDocVersionString(out string version) {
			version = null;
		}

		void IActiveScriptSite.OnScriptTerminate(object result, System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo) {
		}

		void IActiveScriptSite.OnStateChange(ScriptState scriptState) {
		}

		#endregion
	}

}