using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Lumen.Scripting.Inspecting {

	[Guid("00020400-0000-0000-c000-000000000046"),
	InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]

	public interface IDispatch {
		int GetTypeInfoCount();

		System.Runtime.InteropServices.ComTypes.ITypeInfo GetTypeInfo(
			[MarshalAs(UnmanagedType.U4)] int iTInfo, [MarshalAs(UnmanagedType.U4)] int lcid
		);

		void GetTypeInfo(uint iTInfo, int lcid, out IntPtr info);

		[PreserveSig]
		int GetIDsOfNames(ref Guid riid,
							[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)] string[] rgsNames,
							int cNames,
							int lcid,
							[MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);

		[PreserveSig]
		int Invoke(int dispIdMember,
					 ref Guid riid,
					 [MarshalAs(UnmanagedType.U4)] int lcid,
					 [MarshalAs(UnmanagedType.U4)] int dwFlags,
					 ref System.Runtime.InteropServices.ComTypes.DISPPARAMS pDispParams,
					 [Out, MarshalAs(UnmanagedType.LPArray)] object[] pVarResult,
					 ref System.Runtime.InteropServices.ComTypes.EXCEPINFO pExcepInfo,
					 [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] pArgErr);
	}
}

