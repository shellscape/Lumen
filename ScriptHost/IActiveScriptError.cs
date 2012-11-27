using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

	[Guid("EAE1BA61-A4ED-11cf-8F20-00805F2CD064"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IActiveScriptError {
		void GetExceptionInfo(out System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo);
		void GetSourcePosition(out uint sourceContext, out int lineNumber, out int characterPosition);
		void GetSourceLineText(out string sourceLine);
	}

}