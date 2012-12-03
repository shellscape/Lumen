using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Lumen.Search;
using Lumen.Scripting;
using Lumen.Controls;
using Lumen.Extensions;

namespace Lumen.Windows {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Main : Window {

		private HotKeyHandeler _hotkeys;
		private List<LumenCommand> _registeredCommands = new List<LumenCommand>();
		private List<Search.WindowsSearchKind> _searchKinds = new List<Search.WindowsSearchKind>();
		private List<BaseBlock> _virtualBlocks = new List<BaseBlock>();
		private String _buffer = String.Empty;
		private bool _ignoreChange = false;

		private Search.WindowsSearch _windowsSearch = new Search.WindowsSearch();

		private int _selectedIndex = -1;
		private int _initialWidth = 0;
		private int _initialHeight = 0;
		private int _resultHeight = 0;

		private object _lock = new object();

		public Main() {
			InitializeComponent();

			this.WindowStartupLocation = WindowStartupLocation.Manual;
			this.Top = this.Left = 0;
			this.Focusable = true;
			this.ShowInTaskbar = false;

			var style = (Style)FindResource("CommandInput");
			var block = new RichTextBox() { Style = style };

			this.Loaded += delegate(object sender, RoutedEventArgs e) {
				this.Visibility = Visibility.Hidden;

				_hotkeys = new HotKeyHandeler(this);
				_hotkeys.RegisterHotKey(AccessModifierKeys.Win | AccessModifierKeys.Alt, Key.Space);
				_hotkeys.HotKeyPressed += _hotkeys_HotKeyPressed;
			};

			_initialWidth = block.Width == 0 ? 660 : (int)block.Width;

			style = (Style)FindResource("Result");
			var resultBlock = new TextBlock() { Style = style };

			_resultHeight = (int)resultBlock.Height;

			_TextCommand.TextChanged += _TextCommand_TextChanged;
			_BorderMain.SizeChanged += _Canvas_SizeChanged;

			ExtensionManager.Current.ForEach((e) => {
				var commands = e.GetCommands();
				commands.ForEach((c) => {
					_registeredCommands.Add(new LumenCommand() { Command = c, ParameterHint = "" });
				});
			});

			if (Search.WindowsSearchProvider.IsAvailable) {
				_windowsSearch.ResultsChanged += WindowsSearch_ResultsChanged;
			}

			Styles.Init(this);
			PrepareCategories();
		}

		protected override void OnDeactivated(EventArgs e) {
			base.OnDeactivated(e);
			this.Visibility = Visibility.Hidden;
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);

			if (e.Key == Key.Escape) {
				this.Visibility = Visibility.Hidden;
			}
			else if (e.Key == Key.Enter || e.Key == Key.Return) {
				// TODO: execute the command
			}
			else if (e.Key == Key.Down || e.Key == Key.Up) {

				if (e.Key == Key.Down) {
					_selectedIndex = Math.Min(_selectedIndex + 1, _virtualBlocks.Count - 1);
				}
				else if (e.Key == Key.Up) {
					_selectedIndex = Math.Max(_selectedIndex - 1, 0);
				}

				if (_virtualBlocks.Count == 0) {
					_selectedIndex = -1;
				}
				else {

					var selected = _virtualBlocks.Where(b => b.Selected == true).FirstOrDefault();

					if (selected != null) {
						selected.Selected = false;
					}
					_virtualBlocks[_selectedIndex].Selected = true;
				}
			}
		}

		private void _hotkeys_HotKeyPressed(object sender, HotKeyEventArgs e) {
			this.Visibility = System.Windows.Visibility.Visible;
			this.Activate();
			_TextCommand.Focus();
		}

		private void _Canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
			if (_initialHeight == 0) {
				_initialHeight = (int)(_BorderHeader.ActualHeight + _BorderMain.ActualHeight);
			}
			double screenMax = System.Windows.SystemParameters.PrimaryScreenHeight - Math.Max(100, _resultHeight * 4);
			this.Height = Math.Max(_initialHeight, Math.Min(_BorderHeader.ActualHeight + _BorderMain.ActualHeight, screenMax));
		}

		private void _TextCommand_TextChanged(object sender, EventArgs e) {

			if (_ignoreChange) {
				return;
			}

			ResizeCommand();
			ProcessCommand();
			ProcessSearch();
		}

		private void WindowsSearch_ResultsChanged(object sender, EventArgs e) {

			Action method = delegate() {
				_Progress.Visibility = System.Windows.Visibility.Hidden;
				RenderSearchResults();
			};

			if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread) {
				method();
			}
			else {
				this.Dispatcher.BeginInvoke(method, null);
			}
		}

		private void UpdateVirtualRows<T>(Grid grid) where T : BaseBlock {
			_virtualBlocks.RemoveAll(v => v.GetType() == typeof(T));
			_virtualBlocks.AddRange(grid.Children.OfType<T>());

			if (_selectedIndex > _virtualBlocks.Count) {
				_selectedIndex = -1;
			}

			//var selected = _virtualBlocks.Where(b => b.Selected == true).FirstOrDefault();

			//if (selected != null) {
			//	selected.Selected = false;
			//}

			//_virtualBlocks[_selectedIndex].Selected = true;
		}

		private void PrepareCategories() {

			// setup the display order of the _searchKinds
			_searchKinds = new List<WindowsSearchKind>{
				WindowsSearchKind.program,
				WindowsSearchKind.document,
				WindowsSearchKind.folder,
				WindowsSearchKind.picture,
				WindowsSearchKind.link,
				WindowsSearchKind.music,
				WindowsSearchKind.movie,
				WindowsSearchKind.video,
				WindowsSearchKind.email,
				WindowsSearchKind.file,

				// misc stuff that hardly ever makes the results
				WindowsSearchKind.contact,
				WindowsSearchKind.calendar,
				WindowsSearchKind.communication,
				WindowsSearchKind.feed,
				WindowsSearchKind.game,
				WindowsSearchKind.instantmessage,
				WindowsSearchKind.journal,
				WindowsSearchKind.note,
				WindowsSearchKind.recordedtv,
				WindowsSearchKind.searchfolder,
				WindowsSearchKind.task,
				WindowsSearchKind.webhistory
			};
		}

		private void ResizeCommand() {
			var text = _TextCommand.Document.GetFormattedText();
			var width = text.WidthIncludingTrailingWhitespace + 20;

			_buffer = _TextCommand.Document.GetText().Trim();
			_TextCommand.Width = _BorderMain.Width = Math.Max(width, _initialWidth);
		}

		private void ProcessSearch() {

			_GridExtensionResults.ClearRows();
			_GridResults.ClearRows();

			if (_buffer.Length > 2) {
				_Progress.Visibility = System.Windows.Visibility.Visible;

				List<ExtensionResult> results = new List<ExtensionResult>();

				ExtensionManager.Current.ForEach((e) => {
					var extResults = e.GetResults(_buffer);
					if (extResults != null) {
						results.AddRange(extResults);
					}
				});

				_GridExtensionResults.Children.Clear();
				_GridExtensionResults.RowDefinitions.Clear();

				String previous = String.Empty;

				foreach (var result in results) {
					var block = new ExtensionSearchBlock(result, _buffer.ToString()) { Width = _BorderMain.Width, ShowCategory = previous != result.Extension.Name };

					_GridExtensionResults.AddRow(block);
					UpdateVirtualRows<ExtensionSearchBlock>(_GridExtensionResults);

					previous = result.Extension.Name;
				}

				_windowsSearch.Search(_buffer);
			}
		}

		private void RenderSearchResults() {

			_GridResults.ClearRows();

			// take the top three in each category (this is how spotlight does it)
			foreach (var resultKind in _searchKinds) {

				bool showCategory = true;

				var subset = from result in _windowsSearch.Results
										 where (result.Kind & resultKind) == resultKind && result.Touched == false
										 orderby result.Rank descending
										 select result;

				foreach (var result in subset.Take(3)) {

					var fileName = result.FileName;
					var buffer = _buffer.ToString();

					result.Touched = true;
					result.Kind = resultKind;

					// window search: if the search term doesn't appear in the filename, skip it.
					int pos = fileName.IndexOf(buffer, StringComparison.CurrentCultureIgnoreCase);
					if (pos < 0) {
						continue;
					}

					var block = new WindowsSearchBlock(result, _buffer.ToString()) { Width = _BorderMain.Width, ShowCategory = showCategory };

					_GridResults.AddRow(block);
					UpdateVirtualRows<WindowsSearchBlock>(_GridResults);

					if (showCategory) {
						showCategory = false;
					}
				}

				showCategory = true;
			}
		}

		private void ProcessCommand() {
			var distances = new List<LumenCommandDistance>();
			LumenCommand topCommand = null;

			if (_buffer.Length > 0) {

				foreach (var command in _registeredCommands) {
					if (!command.Command.Contains(_buffer)) {
						continue;
					}
					int distance = LevenshteinDistance(_buffer, command.Command);
					distances.Add(new LumenCommandDistance() { Distance = distance, Command = command });
				}
				distances = distances.OrderBy(o => o.Distance).ToList();
			}

			if (distances.Count > 0) {
				topCommand = distances.Take(1).SingleOrDefault().Command;
			}

			FormatPrompt(topCommand);

			List<LumenCommand> commands = distances.Select(o => o.Command).Take(10).ToList();

			_GridCommands.ClearRows();

			foreach (var command in commands) {
				var block = new CommandBlock(command, _buffer.ToString()) { Width = _BorderMain.Width };
				_GridCommands.AddRow(block);
				UpdateVirtualRows<CommandBlock>(_GridCommands);
			}
		}

		private void FormatPrompt(LumenCommand command) {
			var buffer = _buffer.ToString();
			var text = String.Empty;
			var hint = String.Empty;

			if (command != null) {
				text = command.Command;
				hint = command.ParameterHint;
			}

			_Command.Text = String.Empty;

			if (!text.StartsWith(buffer)) {
				return;
			}

			_Command.Inlines.Add(new TextBlock() {
				Foreground = Brushes.Transparent,
				Text = buffer
			});

			_Command.Inlines.Add(new TextBlock() {
				Style = (Style)FindResource("CommandPreview"),
				Text = text.Substring(buffer.Length)
			});

			if (command != null && buffer.Length < command.Command.Length + 1) { // don't want to display the hin
				_Command.Inlines.Add(new TextBlock() {
					Style = (Style)FindResource("CommandHint"),
					Text = " " + hint
				});
			}
		}

		/// <summary>
		/// Used to determine the distance between strings. Lower distance, better match.
		/// </summary>
		/// <remarks>http://sadgeeksinsnow.blogspot.com/2010/10/optimizing-levenshtein-algorithm-in-c.html</remarks>
		private int LevenshteinDistance(String s, String t) {
			int n = s.Length; //length of s
			int m = t.Length; //length of t
			int[,] d = new int[n + 1, m + 1]; // matrix
			int cost; // cost
			// Step 1
			if (n == 0) return m;
			if (m == 0) return n;
			// Step 2
			for (int i = 0; i <= n; d[i, 0] = i++) ;
			for (int j = 0; j <= m; d[0, j] = j++) ;
			// Step 3
			for (int i = 1; i <= n; i++) {
				//Step 4
				for (int j = 1; j <= m; j++) {
					// Step 5
					cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1); // *We'll be looking at this line*.
					// Step 6
					d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
				}
			}
			// Step 7
			return d[n, m];
		}

	}
}
