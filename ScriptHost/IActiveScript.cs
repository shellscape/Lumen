using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

	[Guid("BB1A2AE1-A4F9-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScript {
		void SetScriptSite(IActiveScriptSite pass);
		void GetScriptSite(Guid riid, out IntPtr site);
		void SetScriptState(ScriptState state);
		void GetScriptState(out ScriptState scriptState);
		void Close();
		void AddNamedItem(string name, ScriptItem flags);
		void AddTypeLib(Guid typeLib, uint major, uint minor, uint flags);
		void GetScriptDispatch(string itemName, out IntPtr dispatch);
		void GetCurrentScriptThreadID(out uint thread);
		void GetScriptThreadID(uint win32ThreadId, out uint thread);
		void GetScriptThreadState(uint thread, out ScriptThreadState state);
		void InterruptScriptThread(uint thread, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo, uint flags);
		void Clone(out IActiveScript script);
	}
}