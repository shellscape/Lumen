using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Lumen.Controls {

	public class WindowsSearchBlock : BaseBlock {

		private String _category = String.Empty;
		private TextBlock _categoryBlock = null;

		public WindowsSearchBlock(Search.WindowsSearchResult searchResult, String highlight) : base(searchResult.FileName, highlight, RowType.WindowsSearch){
			this._category = searchResult.Kind.ToString();
			Console.WriteLine(_category);
			if (this._part != null) {
				this._part.Style = Styles.ResultHighlight;
			}
			
			this.Style = Styles.Result;
			this.SearchResult = searchResult;
		}

		public override void Render() {
			this.Inlines.Add(_categoryBlock = new TextBlock() {
				Style = Styles.Category,
				Text = _category,
				FontWeight = FontWeights.Normal
			});

			base.Render();
		}

		protected override void OnSelectedChanged() {
			Console.WriteLine("Selected - " + Selected);
			if (this.Selected) {
				//this.SetResourceReference(TextBlock.StyleProperty, "SelectedCommand");
				//this.Style = FindResource("SelectedCommand") as Style;
				this.Style = Styles.SelectedResult;
				//if (_part != null) {
				//	this._part.Style = this.Style = Styles.SelectedResultHighlight;
				//}
			}
			else {
			//	if (this._part != null) {
			//		this._part.Style = Styles.ResultHighlight;
			//	}
				this.Style = Styles.Result;
				//this.Style = FindResource("Result") as Style;
			}

		}

		public Search.WindowsSearchResult SearchResult { get; private set; }

		public Boolean ShowCategory {
			get { return _categoryBlock.Text == _category; }
			set {
				if (value) {
					_categoryBlock.Text = _category;
				}
				else {
					_categoryBlock.Text = String.Empty;
				}
			}
		}

	}
}