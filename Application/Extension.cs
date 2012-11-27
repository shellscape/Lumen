using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lumen.Scripting;

namespace Lumen {
	public class Extension : IDisposable {

		private ScriptEngine _engine = null;
		private ParsedScript _parsed = null;
		private FileSystemWatcher _watcher = null;
		private Hashtable _files = new Hashtable();
		private List<ExtensionScript> _scripts = new List<ExtensionScript>();

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
				_files.Add(filePath, file.LastWriteTimeUtc);
			}

			LoadScript(entryPoint);
			
			var require = _engine.GetList("extension.require");

			foreach (var script in require) {
				LoadScript(Path.Combine(path, (String)script));
			};
		}

		private void LoadScript(String scriptPath) {
			String script = String.Empty;
			var file = new FileInfo(scriptPath);
			var last = _scripts.Last();
			int startPosition = last.StartPosition + last.Length;

			using (var sr = new StreamReader(scriptPath)) {
				script = sr.ReadToEnd();
			}

			_engine.Parse(script);

			_scripts.Add(new ExtensionScript { Name = file.Name, StartPosition = startPosition, Length = script.Length });
		}

		void _watcher_Changed(object sender, FileSystemEventArgs e) {
			_watcher.EnableRaisingEvents = false;

			lock (_lock) {

				DateTime lastWrite = DateTime.UtcNow;
				var file = new FileInfo(e.FullPath);

				if (!_files.ContainsKey(e.FullPath)) {
					_files.Add(e.FullPath, file.LastWriteTimeUtc);
					lastWrite = file.LastWriteTimeUtc;
				}
				else {
					lastWrite = (DateTime)_files[e.FullPath];
				}

				if (lastWrite != file.LastWriteTimeUtc) {
					// reload the extension
				}

				_watcher.EnableRaisingEvents = true;
			}
		}

		~Extension() {
			Dispose();
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
