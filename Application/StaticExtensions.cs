using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen {
	public static class StaticExtensions {

		public static Boolean IsComObject(this object value) {
			return value.GetType().GUID.Equals(Guid.Empty);
		}

		public static object TryGetValue(this Hashtable hash, object key) {
			if (!hash.ContainsKey(key)) {
				return null;
			}
			return hash[key];
		}
	}
}
