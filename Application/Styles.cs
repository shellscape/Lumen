using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Lumen {
	public static class Styles {

		private static Style _resultStyle = null;
		private static Style _resultPartStyle = null;
		private static Style _resultKindStyle = null;
		private static Style _commandStyle = null;
		private static Style _commandPartStyle = null;

		public static Style Result { get { return _resultStyle; } }
		public static Style ResultHighlight { get { return _resultPartStyle; } }
		public static Style Category { get { return _resultKindStyle; } }
		public static Style Command { get { return _commandStyle; } }
		public static Style CommandHighlight { get { return _commandPartStyle; } }

		public static void Init(FrameworkElement element) {
			_resultStyle = element.FindResource("Result") as Style;
			_resultPartStyle = element.FindResource("ResultPart") as Style;
			_resultKindStyle = element.FindResource("ResultKind") as Style;
			_commandStyle = element.FindResource("Command") as Style;
			_commandPartStyle = element.FindResource("CommandPart") as Style;
		}
	}
}