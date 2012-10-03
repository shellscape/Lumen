using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

  /// <summary>
  /// Defines a pre-parsed script object that can be evaluated at runtime.
  /// </summary>
  public class ParsedScript : IDisposable {

	private object _dispatch;
	private readonly ScriptEngine _engine;

	internal ParsedScript(ScriptEngine engine, IntPtr dispatch) {
	  this._engine = engine;
	  this._dispatch = Marshal.GetObjectForIUnknown(dispatch);
	}

	/// <summary>
	///   returns the list of functions/methods defined in the script.
	/// </summary>
	/// <returns>a List of strings, with the function names.</returns>
	/// <remarks>
	/// </remarks>
	public Dictionary<string, int> GetFunctions() {
	  var inspector = new Lumen.Scripting.Inspecting.ObjectInspector(_dispatch);
	  return inspector.GetMethods();
	}

	/// <summary>
	/// Calls a method.
	/// </summary>
	/// <param name="methodName">The method name. May not be null.</param>
	/// <param name="arguments">The optional arguments.</param>
	/// <returns>The call result.</returns>
	public object CallMethod(string methodName, params object[] arguments) {
	 
	  if(_dispatch == null) {
		throw new InvalidOperationException();
	  }

	  if(methodName == null) {
		throw new ArgumentNullException("methodName");
	  }

	  try {
		return _dispatch.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod, null, _dispatch, arguments);
	  }
	  catch {
		if(_engine._site._lastException != null)
		  throw _engine._site._lastException;

		throw;
	  }
	}

	#region .    Disposable

	protected virtual void Dispose(bool disposing) {
	  if(disposing) {
	  }

	  if(_dispatch != null) {
		Marshal.ReleaseComObject(_dispatch);
		_dispatch = null;
	  }
	}

	public void Dispose() {
	  this.Dispose(true);
	  GC.SuppressFinalize(this);
	}

	#endregion

  }

}