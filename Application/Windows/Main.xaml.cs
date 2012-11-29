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

namespace Lumen.Windows {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Main : Window {

		private List<LumenCommand> _commands = new List<LumenCommand>();
		private String _buffer = String.Empty;
		private bool _ignoreChange = false;

		private Search.WindowsSearch _windowsSearch = new Search.WindowsSearch();
		private List<ExtensionResult> _extensionResults = new List<ExtensionResult>();

		private int _initialWidth = 0;
		private int _initialHeight = 0;
		private int _resultHeight = 0;

		private Style _resultStyle = null;
		private Style _resultPartStyle = null;
		private Style _resultKindStyle = null;

		[ComVisible(true)]
		public class ScriptObject {
			public void alert(String what) {
				MessageBox.Show(what);
			}
		}

		public Main() {
			InitializeComponent();

			this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
			this.Top = this.Left = 0;

			var style = (Style)FindResource("CommandInput");
			var block = new RichTextBox() { Style = style };

			_initialWidth = block.Width == 0 ? 660 : (int)block.Width;

			style = (Style)FindResource("Result");
			var resultBlock = new TextBlock() { Style = style };

			_resultHeight = (int)resultBlock.Height;

			_TextCommand.TextChanged += _TextCommand_TextChanged;
			_BorderMain.SizeChanged += _Canvas_SizeChanged;

			//_commands.AddRange(new LumenCommand[]{
			//	new LumenCommand(){ Command = "open", ParameterHint = "target" },
			//	new LumenCommand(){ Command = "open run", ParameterHint = "" },
			//	new LumenCommand(){ Command = "open google", ParameterHint = "" },
			//	new LumenCommand(){ Command = "open git", ParameterHint = "" },
			//	new LumenCommand(){ Command = "github", ParameterHint = "" },
			//	new LumenCommand(){ Command = "github issues", ParameterHint = "" }
			//});

			ExtensionManager.Current.ForEach((e) => {
				var commands = e.GetCommands();
				commands.ForEach((c) => {
					_commands.Add(new LumenCommand() { Command = c, ParameterHint = "" });
				});
			});

			if (Search.WindowsSearchProvider.IsAvailable) {
				_windowsSearch.ResultsChanged += WindowsSearch_ResultsChanged;
			}

			_resultStyle = (Style)FindResource("Result");
			_resultPartStyle = (Style)FindResource("ResultPart");
			_resultKindStyle = (Style)FindResource("ResultKind");

		}

		void _Canvas_SizeChanged(object sender, SizeChangedEventArgs e) {
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
			ProcessSearch();
			ProcessCommand();
		}

		private void WindowsSearch_ResultsChanged(object sender, EventArgs e) {

			Action method = delegate() {
				_Progress.Visibility = System.Windows.Visibility.Hidden;
				DisplaySearchResults();
			};

			if (this.Dispatcher.Thread == System.Threading.Thread.CurrentThread) {
				method();
			}
			else {
				this.Dispatcher.BeginInvoke(method, null);
			}
		}

		private void ResizeCommand() {
			var text = _TextCommand.Document.GetFormattedText();
			var width = text.WidthIncludingTrailingWhitespace + 20;

			_buffer = _TextCommand.Document.GetText().Trim();
			_TextCommand.Width = _BorderMain.Width = Math.Max(width, _initialWidth);
		}

		private void ProcessSearch() {

			_GridResults.Children.Clear();
			_GridResults.RowDefinitions.Clear();

			_GridSearchResults.Children.Clear();
			_GridSearchResults.RowDefinitions.Clear();

			if (_buffer.Length > 2) {
				_Progress.Visibility = System.Windows.Visibility.Visible;

				_extensionResults.Clear();

				ExtensionManager.Current.ForEach((e) => {
					var extResults = e.GetResults(_buffer);
					if (extResults != null) {
						_extensionResults.AddRange(extResults);
					}
				});

				DisplayExtensionResults();

				_windowsSearch.Search(_buffer);
			}
		}

		// TODO: this is dumb. i should arrange the enum how I want it, but its quick and dirty for now.
		private List<Search.WindowsSearchKind> PrepareCategories() {
			var categories = new List<Search.WindowsSearchKind>();

			// setup the display order of the categories
			categories.Add(WindowsSearchKind.program);
			categories.Add(WindowsSearchKind.document);
			categories.Add(WindowsSearchKind.folder);
			categories.Add(WindowsSearchKind.picture);
			categories.Add(WindowsSearchKind.link);
			categories.Add(WindowsSearchKind.music);
			categories.Add(WindowsSearchKind.movie);
			categories.Add(WindowsSearchKind.video);
			categories.Add(WindowsSearchKind.email);
			categories.Add(WindowsSearchKind.file);

			// misc stuff that hardly ever makes the results
			categories.Add(WindowsSearchKind.contact);
			categories.Add(WindowsSearchKind.calendar);
			categories.Add(WindowsSearchKind.communication);
			categories.Add(WindowsSearchKind.feed);
			categories.Add(WindowsSearchKind.game);
			categories.Add(WindowsSearchKind.instantmessage);
			categories.Add(WindowsSearchKind.journal);
			categories.Add(WindowsSearchKind.note);
			categories.Add(WindowsSearchKind.recordedtv);
			categories.Add(WindowsSearchKind.searchfolder);
			categories.Add(WindowsSearchKind.task);
			categories.Add(WindowsSearchKind.webhistory);

			return categories;
		}

		private void DisplayExtensionResults() {

			_GridResults.Children.Clear();
			_GridResults.RowDefinitions.Clear();

			String previous = String.Empty;

			foreach (var result in _extensionResults) {
				RenderResult(result.Text, result.ExtensionName, previous != result.ExtensionName, _GridResults);
				previous = result.ExtensionName;
			}

		}

		private void DisplaySearchResults() {

			_GridSearchResults.Children.Clear();
			_GridSearchResults.RowDefinitions.Clear();

			var categories = PrepareCategories();

			// take the top three in each category (this is how spotlight does it)
			foreach (var resultKind in categories) {

				bool showCategory = true;

				var test = from result in _windowsSearch.Results
									 where (result.Kind & resultKind) == resultKind && result.Touched == false
									 orderby result.Rank descending
									 select result;

				foreach (var result in test.Take(3)) {

					var fileName = result.FileName;
					var buffer = _buffer.ToString();

					result.Touched = true;

					// window search: if the search term doesn't appear in the filename, skip it.
					int pos = fileName.IndexOf(buffer, StringComparison.CurrentCultureIgnoreCase);
					if (pos < 0) {
						continue;
					}

					RenderResult(fileName, resultKind.ToString() + "s", showCategory, _GridSearchResults);

					if (showCategory) {
						showCategory = false;
					}
				}

				showCategory = true;

			}
		}

		private void RenderResult(String text, String category, Boolean showCategory, Grid grid) {

			var buffer = _buffer.ToString();
			var textBlock = new TextBlock() { Style = _resultStyle };

			if (!showCategory) {
				category = String.Empty;
			}

			var kind = new TextBlock() {
				Style = _resultKindStyle,
				Text = category,
				FontWeight = FontWeights.Normal
			};

			textBlock.Inlines.Add(kind);

			int pos = text.IndexOf(buffer, StringComparison.CurrentCultureIgnoreCase);
			if (pos >= 0) {

				var start = text.Substring(0, pos);
				var end = text.Substring(pos + buffer.Length);
				var highlighted = text;

				if (!String.IsNullOrEmpty(end)) {
					highlighted = highlighted.Replace(end, string.Empty);
				}

				if (!String.IsNullOrEmpty(start)) {
					highlighted = highlighted.Replace(start, string.Empty);
				}

				var part = new TextBlock() {
					Style = _resultPartStyle,
					Text = highlighted,
					FontWeight = FontWeights.Normal
				};

				textBlock.Inlines.Add(start);
				textBlock.Inlines.Add(part);
				textBlock.Inlines.Add(end);
			}
			else {
				textBlock.Inlines.Add(text);
			}

			textBlock.Width = _BorderMain.Width;

			Grid.SetRow(textBlock, grid.Children.Count);

			grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
			grid.Children.Add(textBlock);

		}

		private void ProcessCommand() {
			var distances = new List<LumenCommandDistance>();
			LumenCommand topCommand = null;

			if (_buffer.Length > 0) {

				foreach (var command in _commands) {
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
			DisplayCommands(distances.Select(o => o.Command).Take(10).ToList());
		}

		private void DisplayCommands(List<LumenCommand> commands) {

			_GridCommands.Children.Clear();
			_GridCommands.RowDefinitions.Clear();

			foreach (var command in commands) {
				var buffer = _buffer.ToString();
				var commandText = command.Command;
				var textBlock = new TextBlock() { Style = (Style)FindResource("Command") };

				int pos = commandText.IndexOf(buffer);
				if (pos < 0) {
					continue;
				}

				var start = commandText.Substring(0, pos);
				var end = commandText.Substring(pos + buffer.Length);
				var part = new TextBlock() {
					Style = (Style)FindResource("CommandPart"),
					Text = buffer,
					FontWeight = FontWeights.Normal
				};

				textBlock.Inlines.Add(start);
				textBlock.Inlines.Add(part);
				textBlock.Inlines.Add(end);

				_GridCommands.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
				Grid.SetRow(textBlock, _GridCommands.Children.Count);

				_GridCommands.Children.Add(textBlock);
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

	public class LumenCommand {

		public String Command { get; set; }
		public String ParameterHint { get; set; }

	}

	public class LumenCommandDistance {

		public LumenCommand Command { get; set; }
		public int Distance { get; set; }

	}
}
