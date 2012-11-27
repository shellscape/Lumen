using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

	[Flags]
	internal enum ScriptText {
		None = 0,
		DelayExecution = 1,
		IsVisible = 2,
		IsExpression = 32,
		IsPersistent = 64,
		HostManageSource = 128
	}

	[Flags]
	internal enum ScriptInfo {
		None = 0,
		IUnknown = 1,
		ITypeInfo = 2
	}

	[Flags]
	internal enum ScriptItem {
		None = 0,
		IsVisible = 2,
		IsSource = 4,
		GlobalMembers = 8,
		IsPersistent = 64,
		CodeOnly = 512,
		NoCode = 1024
	}

	internal enum ScriptThreadState {
		NotInScript = 0,
		Running = 1
	}

	internal enum ScriptState {
		Uninitialized = 0,
		Started = 1,
		Connected = 2,
		Disconnected = 3,
		Closed = 4,
		Initialized = 5
	}

}