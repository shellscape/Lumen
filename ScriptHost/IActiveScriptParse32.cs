using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

	[Guid("BB1A2AE2-A4F9-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptParse32 {
		void InitNew();
		void AddScriptlet(string defaultName, string code, string itemName, string subItemName, string eventName, string delimiter, IntPtr sourceContextCookie, uint startingLineNumber, ScriptText flags, out string name, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
		void ParseScriptText(string code, string itemName, object context, string delimiter, IntPtr sourceContextCookie, uint startingLineNumber, ScriptText flags, out object result, out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
	}

}