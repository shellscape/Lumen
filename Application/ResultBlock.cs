using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Lumen {
	public class ResultBlock : TextBlock {

		private String _category = String.Empty;
		private TextBlock _categoryBlock = null;

		public ResultBlock(String text, String highlight, String category) : base(){

			_category = category;

			this.Style = Styles.Result;

			this.Inlines.Add(_categoryBlock = new TextBlock() {
				Style = Styles.Category,
				Text = category,
				FontWeight = FontWeights.Normal
			});

			int pos = text.IndexOf(highlight, StringComparison.CurrentCultureIgnoreCase);
			if (pos >= 0) {

				var start = text.Substring(0, pos);
				var end = text.Substring(pos + highlight.Length);
				var highlighted = text;

				if (!String.IsNullOrEmpty(end)) {
					highlighted = highlighted.Replace(end, string.Empty);
				}

				if (!String.IsNullOrEmpty(start)) {
					highlighted = highlighted.Replace(start, string.Empty);
				}

				this.Inlines.Add(start);
				this.Inlines.Add(new TextBlock() {
					Style = Styles.ResultHighlight,
					Text = highlighted,
					FontWeight = FontWeights.Normal
				});
				this.Inlines.Add(end);
			}
			else {
				this.Inlines.Add(text);
			}

		}

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
