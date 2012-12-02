using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Lumen.Controls {

	public class ExtensionSearchBlock : BaseBlock {

		private String _category = String.Empty;
		private TextBlock _categoryBlock = null;

		public ExtensionSearchBlock(Extensions.ExtensionResult extensionResult, String highlight) : base(extensionResult.Text, highlight, RowType.WindowsSearch){
			this._category = extensionResult.Extension.Name;
			if (this._part != null) {
				this._part.Style = Styles.ResultHighlight;
			}
			
			this.Style = Styles.Result;
			this.ExtensionResult = extensionResult;
		}

		public override void Render() {
			this.Inlines.Add(_categoryBlock = new TextBlock() {
				Style = Styles.Category,
				Text = _category,
				FontWeight = FontWeights.Normal
			});

			base.Render();
		}

		public Extensions.ExtensionResult ExtensionResult { get; private set; }

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