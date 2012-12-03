using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

using Lumen.Extensions;

namespace Lumen.Controls {

	public class CommandBlock : BaseBlock {

		public CommandBlock(LumenCommand command, String highlight) : base(command.Command, highlight, RowType.Command) {
			this.Command = command;
			if (_part != null) {
				_part.Style = Styles.CommandHighlight;
			}
		}

		protected override void OnSelectedChanged() {
			this.Style = Styles.SelectedCommand;
			this._part.Style = this.Style = Styles.SelectedCommandHighlight;
		}

		public LumenCommand Command { get; private set; }
	}
}