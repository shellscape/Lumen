using System.Linq;

namespace System.Collections.Generic {
  
  public static class IEnumerableExtensions {

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
