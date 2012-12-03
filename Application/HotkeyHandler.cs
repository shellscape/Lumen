using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;

namespace Lumen {

	public enum AccessModifierKeys {
		/// <summary>
		/// Alt key
		/// </summary>
		Alt = 0x1,

		/// <summary>
		/// Control key
		/// </summary>
		Control = 0x2,

		/// <summary>
		/// Shift key
		/// </summary>
		Shift = 0x4,

		/// <summary>
		/// Window key
		/// </summary>
		Win = 0x8
	}

	public class HotKeyEventArgs : EventArgs {
		public HotKeyEventArgs(AccessModifierKeys pModKeys, Key pKey) {
			ModifierKeys = pModKeys;
			Key = pKey;
		}
		public Key Key { get; private set; }
		public AccessModifierKeys ModifierKeys { get; private set; }
	}

	public class HotKeyHandeler : IDisposable {

		public const int WM_HOTKEY = 0x312;

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

		[DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("kernel32", SetLastError = true)]
		private static extern short GlobalAddAtom(string lpString);

		[DllImport("kernel32", SetLastError = true)]
		private static extern short GlobalDeleteAtom(short nAtom);

		private Hashtable keyIDs = new Hashtable();

		public event EventHandler<HotKeyEventArgs> HotKeyPressed;

		public IntPtr Handle { get; private set; }
		public AccessModifierKeys ModifierKeys { get; private set; }

		~HotKeyHandeler() {
			Dispose();
		}

		public void Dispose() {
			UnegisterHotKey();
		}

		public HotKeyHandeler(IntPtr Handle) {
			this.Handle = Handle;
		}

		public HotKeyHandeler(Window window) {
			this.Handle = new WindowInteropHelper(window).Handle;
			HwndSource src = HwndSource.FromHwnd(Handle);
			src.AddHook(new HwndSourceHook(WndProc));
		}

		private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			// address the messages you are receiving using msg, wParam, lParam
			if (msg == WM_HOTKEY) {
				short hkeyid = (short)wParam;
				if (keyIDs.ContainsKey(hkeyid)) {
					if (HotKeyPressed != null) {
						int key = (int)(((int)lParam >> 16) & 0xFFFF);
						AccessModifierKeys modifier = (AccessModifierKeys)((int)lParam & 0xFFFF);

						HotKeyPressed(this, new HotKeyEventArgs(modifier, KeyInterop.KeyFromVirtualKey(key)));
					}
				}
			}
			return IntPtr.Zero;
		}

		public void RegisterHotKey(AccessModifierKeys modifiers, Key key) {
			// Generates unique ID
			string atomName = modifiers.ToString() + key.ToString() + this.GetType().FullName;
			short hKeyID = GlobalAddAtom(atomName);
			ModifierKeys = modifiers;

			if (hKeyID != 0) {
				if (!RegisterHotKey(Handle, hKeyID, (int)modifiers, KeyInterop.VirtualKeyFromKey(key)))
					throw new ArgumentException("Hotkey combination could not be registered.");
				else
					keyIDs.Add(hKeyID, hKeyID);
			}
			else
				throw new ArgumentException("Hotkey ID not generated!");

		}

		public void UnegisterHotKey() {
			HwndSource src = HwndSource.FromHwnd(Handle);
			if (src != null) {
				src.RemoveHook(new HwndSourceHook(this.WndProc));
			}
			foreach (short id in keyIDs.Values) {
				UnregisterHotKey(Handle, id);
				GlobalDeleteAtom(id);
			}

		}

	}

}
