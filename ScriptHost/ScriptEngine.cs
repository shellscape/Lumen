using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

	/// <summary>
	/// Represents a Windows Script Engine such as JScript, VBScript, etc.
	/// </summary>
	public class ScriptEngine : IDisposable {

		private IActiveScript _engine;
		private IActiveScriptParse32 _parse32;
		private IActiveScriptParse64 _parse64;
		internal ScriptSite _site;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptEngine"/> class.
		/// </summary>
		/// <param name="language">The scripting language. Standard Windows Script engines names are 'jscript' or 'vbscript'.</param>
		public ScriptEngine() {

			var guid = new System.Guid("{16d51579-a30b-4c8b-a276-0ff4dc41e755}"); // Chakra IE9/IE10 JS Engine
			Type t = Type.GetTypeFromCLSID(guid, true);

			_engine = Activator.CreateInstance(t) as IActiveScript;

			if (_engine == null) {
				throw new Exception("Unable to initialize the Chakra engine.");
			}

			_site = new ScriptSite();
			_engine.SetScriptSite(_site);

			if (IntPtr.Size == 4) {
				_parse32 = _engine as IActiveScriptParse32;
				_parse32.InitNew();
			}
			else {
				_parse64 = _engine as IActiveScriptParse64;
				_parse64.InitNew();
			}
		}

		/// <summary>
		/// Adds the name of a root-level item to the scripting engine's name space.
		/// </summary>
		/// <param name="name">The name. May not be null.</param>
		/// <param name="value">The value. It must be a ComVisible object.</param>
		public void SetNamedItem(string name, object value) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			_engine.AddNamedItem(name, ScriptItem.IsVisible | ScriptItem.IsSource);
			_site._namedItems[name] = value;
		}

		/// <summary>
		/// Evaluates an expression using the specified language.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="expression">The expression. May not be null.</param>
		/// <returns>The result of the evaluation.</returns>
		public static object Eval(string language, string expression) {
			return Eval(language, expression, null);
		}

		/// <summary>
		/// Evaluates an expression using the specified language, with an optional array of named items.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="expression">The expression. May not be null.</param>
		/// <param name="namedItems">The named items array.</param>
		/// <returns>The result of the evaluation.</returns>
		public static object Eval(string language, string expression, params KeyValuePair<string, object>[] namedItems) {
			if (language == null) {
				throw new ArgumentNullException("language");
			}

			if (expression == null) {
				throw new ArgumentNullException("expression");
			}

			using (ScriptEngine engine = new ScriptEngine()) {
				if (namedItems != null) {
					foreach (KeyValuePair<string, object> kvp in namedItems) {
						engine.SetNamedItem(kvp.Key, kvp.Value);
					}
				}
				return engine.Eval(expression);
			}
		}

		/// <summary>
		/// Evaluates an expression.
		/// </summary>
		/// <param name="expression">The expression. May not be null.</param>
		/// <returns>The result of the evaluation.</returns>
		public object Eval(string expression) {
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}

			return Parse(expression, true);
		}

		/// <summary>
		/// Parses the specified text and returns an object that can be used for evaluation.
		/// </summary>
		/// <param name="text">The text to parse.</param>
		/// <returns>An instance of the ParsedScript class.</returns>
		public ParsedScript Parse(string text) {
			if (text == null) {
				throw new ArgumentNullException("text");
			}

			return (ParsedScript)Parse(text, false);
		}

		private object Parse(string text, bool expression) {

			const string varName = "x___";
			System.Runtime.InteropServices.ComTypes.EXCEPINFO exceptionInfo;
			object result;
			IntPtr dispatch;
			ScriptText flags = ScriptText.None;

			_engine.SetScriptState(ScriptState.Connected);

			if (expression) {
				flags |= ScriptText.IsExpression;
			}

			try { // immediate expression computation seems to work only for 64-bit so hack something for 32-bit...

				if (_parse32 != null) {
					if (expression) {
						text = varName + "=" + text; // should work for jscript & vbscript at least...
					}
					_parse32.ParseScriptText(text, null, null, null, IntPtr.Zero, 0, flags, out result, out exceptionInfo);
				}
				else {
					_parse64.ParseScriptText(text, null, null, null, IntPtr.Zero, 0, flags, out result, out exceptionInfo);
				}
			}
			catch {
				if (_site._lastException != null) {
					throw _site._lastException;
				}

				throw;
			}

			if (expression) { // continue 32-bit hack...

				if (_parse32 != null) {

					_engine.GetScriptDispatch(null, out dispatch);

					object dp = Marshal.GetObjectForIUnknown(dispatch);

					try {
						var res = dp.GetType().InvokeMember(varName, BindingFlags.GetProperty, null, dp, null);
						return res;
					}
					catch {
						if (_site._lastException != null)
							throw _site._lastException;

						throw;
					}
				}
				return result;
			}

			_engine.GetScriptDispatch(null, out dispatch);

			ParsedScript parsed = new ParsedScript(this, dispatch);

			return parsed;
		}

		public List<object> GetList(String expression) {
			var result = this.Eval(expression);
			using (var inspector = new Inspecting.ObjectInspector(result)) {
				var list = inspector.GetList();

				return list;
			}
		}

		public System.Collections.Hashtable GetHash(String expression) {
			var result = this.Eval(expression);
			using (var inspector = new Inspecting.ObjectInspector(result)) {
				var hash = inspector.GetHash();

				return hash;
			}
		}

		#region .    Disposable    .

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
			}

			if (_parse32 != null) {
				Marshal.ReleaseComObject(_parse32);
				_parse32 = null;
			}

			if (_parse64 != null) {
				Marshal.ReleaseComObject(_parse64);
				_parse64 = null;
			}

			if (_engine != null) {
				Marshal.ReleaseComObject(_engine);
				_engine = null;
			}
		}

		#endregion

	}
}