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
		}

		public LumenCommand Command { get; private set; }
	}
}