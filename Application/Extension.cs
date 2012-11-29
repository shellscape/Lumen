using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lumen.Scripting;
using Lumen.Search;

namespace Lumen {
	public class Extension : IDisposable {

		private const String __extension = "__extension";

		private ScriptEngine _engine = null;
		private ParsedScript _parsed = null;
		private FileSystemWatcher _watcher = null;
		private Hashtable _watchedFiles = new Hashtable();
		private ExtensionScriptManager _scripts = new ExtensionScriptManager();

		private object _lock = new object();
		private Boolean _disposed = false;
		private String _extensionPath = null;

		public Extension(String path) {
			_extensionPath = path;
			_engine = new ScriptEngine();
			_watcher = new FileSystemWatcher(path) { EnableRaisingEvents = true };
			_watcher.Changed += _watcher_Changed;

			String entryPoint = Path.Combine(path, "extension.js");

			if (!File.Exists(entryPoint)) {
				throw new Exception("Extension must contain an extension.js file.");
			}

			var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();

			foreach (var filePath in files) {
				var file = new FileInfo(filePath);
				_watchedFiles.Add(filePath, file.LastWriteTimeUtc);
			}

			_engine.SetNamedItem("lumen", new LumenScript(this));

			try {

				_parsed = LoadScript(entryPoint);

				var require = new List<object>(); // _engine.GetList("extension.require");

				require.Add(@"\..\common.js");

				foreach (var script in require) {
					LoadScript(Path.Combine(path, ((String)script).Replace("/", @"\")));
				};

				ExtensionManager.Current.Add(this);
			}
			catch (ScriptException ex) {
				var s = _scripts.FromLineNumber(ex.Line);
				ex.Line = s.TranslateLineNumber(ex.Line);
				System.Windows.MessageBox.Show(ex.ErrorType + ": " + ex.Description + "\t\t\t" + s.Name + ":" + ex.Line);
			}
		}

		~Extension() {
			Dispose();
		}

		/// <summary>
		/// The javascript identifier of the extension.
		/// </summary>
		public String Name { get; set; }

		public List<ExtensionResult> GetResults(String term) {
			var results = new List<ExtensionResult>();

			var extResults = _parsed.CallMethod(__extension + ".results", term);

			// if it's null or not an array (__COMObject Type) then gtfo
			if (results == null || !results.IsComObject()) {
				return null;
			}

			using (var inspector = new Scripting.Inspecting.ObjectInspector(extResults)) {
				foreach (var result in inspector.GetList()) {
					
					// if they didn't return an array item of hash, continue
					if (result == null || !result.IsComObject()) {
						continue;
					}

					using(var insp = new Scripting.Inspecting.ObjectInspector(result)){
						var hash = insp.GetHash();
						if (!hash.ContainsKey("text")) {
							continue;
						}

						String text = (String)hash.TryGetValue("text");
						String command = (String)hash.TryGetValue("command");

						results.Add(new ExtensionResult() { Text = text, Command = command, Kind = this.Name });
					}
				}
			}

			return results;
		}

		private ParsedScript LoadScript(String scriptPath) {
			ParsedScript result = null;
			String script = String.Empty;
			var file = new FileInfo(scriptPath);
			var last = _scripts.Count > 0 ? _scripts.Last() : new ExtensionScript();
			int startPosition = last.StartPosition + last.Length;

			using (var sr = new StreamReader(scriptPath)) {
				script = sr.ReadToEnd();
			}
			try {
				result = _engine.Parse(script);
			}
			catch (ScriptException ex) {
				System.Windows.MessageBox.Show(ex.ErrorType + ": " + ex.Description + "\t\t\t" + file.Name+ ":" + ex.Line);
			}

			_scripts.Add(new ExtensionScript { Name = file.Name, StartPosition = startPosition, Length = script.Length });

			return result;
		}

		private void _watcher_Changed(object sender, FileSystemEventArgs e) {
			_watcher.EnableRaisingEvents = false;

			lock (_lock) {

				DateTime lastWrite = DateTime.UtcNow;
				var file = new FileInfo(e.FullPath);

				if (!_watchedFiles.ContainsKey(e.FullPath)) {
					_watchedFiles.Add(e.FullPath, file.LastWriteTimeUtc);
					lastWrite = file.LastWriteTimeUtc;
				}
				else {
					lastWrite = (DateTime)_watchedFiles[e.FullPath];
				}

				if (lastWrite != file.LastWriteTimeUtc) {
					// reload the extension
					var path = this._extensionPath;
					ExtensionManager.Current.Remove(this);
					this.Dispose();
					var extension = new Extension(path);
				}

				_watcher.EnableRaisingEvents = true;
			}
		}
				
		public void Dispose() {
			if (!_disposed) {
				_disposed = true;
				_engine.Dispose();
				_watcher.Dispose();
			}
		}
	}
}
