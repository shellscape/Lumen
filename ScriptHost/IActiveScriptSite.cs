using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

  [Guid("DB01A1E3-A42B-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
  private interface IActiveScriptSite {
	void GetLCID(out int lcid);
	void GetItemInfo(string name, ScriptInfo returnMask, out IntPtr item, IntPtr typeInfo);
	void GetDocVersionString(out string version);
	void OnScriptTerminate(object result, System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
	void OnStateChange(ScriptState scriptState);
	void OnScriptError(IActiveScriptError scriptError);
	void OnEnterScript();
	void OnLeaveScript();
  }
}
  