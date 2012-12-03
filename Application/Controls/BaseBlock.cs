using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lumen.Controls {

	public abstract class BaseBlock : TextBlock {

		protected TextBlock _part = null;
		protected String _text = null;
		protected String _highlight = null;
		private Boolean _selected = false;

		public BaseBlock(String text, String highlight, RowType type) : base() {
			this.Type = type;
			this.Style = Styles.Command;

			this._text = text;
			this._highlight = highlight;

			Render();
		}

		public virtual void Render() {
			int pos = _text.IndexOf(_highlight, StringComparison.CurrentCultureIgnoreCase);
			if (pos >= 0) {

				var start = _text.Substring(0, pos);
				var end = _text.Substring(pos + _highlight.Length);
				var highlighted = _text;

				if (!String.IsNullOrEmpty(end)) {
					highlighted = highlighted.Replace(end, string.Empty);
				}

				if (!String.IsNullOrEmpty(start)) {
					highlighted = highlighted.Replace(start, string.Empty);
				}

				this.Inlines.Add(start);
				this.Inlines.Add(_part = new TextBlock() {
					Text = highlighted,
					FontWeight = FontWeights.Normal
				});
				this.Inlines.Add(end);
			}
			else {
				this.Inlines.Add(_text);
			}
		}

		protected virtual void OnSelectedChanged() {
			// set the selected styles
		}

		public Boolean Selected {
			get { return _selected; }
			set {
				Boolean trigger = _selected != value;
				_selected = value;
				if(trigger){
					OnSelectedChanged();
				}
			}
		}

		public RowType Type { get; set; }
	}
}