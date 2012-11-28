using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Scripting {
	public static class StaticExtensions {

		public static String GetDescription(this Enum value) {
			DescriptionAttribute attribute = value.GetType()
					.GetField(value.ToString())
					.GetCustomAttributes(typeof(DescriptionAttribute), false)
					.SingleOrDefault() as DescriptionAttribute;
			return attribute == null ? value.ToString() : attribute.Description;
		}

		public static T Convert<T>(this object input) {

			try {
				var result = (T)input;
				return result;
			}
			catch {
				var converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter != null) {
					try {
						return (T)converter.ConvertTo(input, typeof(T));
					}
					catch { }
				}
			}
			return default(T);
		}

	}
}
