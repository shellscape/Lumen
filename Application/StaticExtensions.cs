using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lumen {
	public static class StaticExtensions {

		public static void ClearRows(this Grid grid) {
			grid.Children.Clear();
			grid.RowDefinitions.Clear();
		}

		public static void AddRow(this Grid grid, TextBlock block) {
			Grid.SetRow(block, grid.Children.Count);

			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			grid.Children.Add(block);
		}

		public static Boolean IsComObject(this object value) {
			return value.GetType().GUID.Equals(Guid.Empty);
		}

		public static object TryGetValue(this Hashtable hash, object key) {
			if (!hash.ContainsKey(key)) {
				return null;
			}
			return hash[key];
		}

		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T, object> method) {
			return source.Distinct(new GenericComparer<T>(method));
		}

		class GenericComparer<T> : IEqualityComparer<T> {

			private Func<T, object> _method;

			public GenericComparer(Func<T, object> method) {
				this._method = method;
			}

			bool IEqualityComparer<T>.Equals(T x, T y) {
				bool res = this._method(x).Equals(this._method(y));
				return res;
			}

			int IEqualityComparer<T>.GetHashCode(T obj) {
				return this._method(obj).GetHashCode();
			}
		}

	}
}
