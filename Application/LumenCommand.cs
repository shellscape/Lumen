using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen {
	public class LumenCommand {

		public LumenCommand() {
			this.ID = Guid.NewGuid();
		}

		public Guid ID { get; private set; }
		public String Command { get; set; }
		public String ParameterHint { get; set; }
		public Extension Extension { get; set; }
	}
}
