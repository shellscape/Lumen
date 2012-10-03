﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

namespace Lumen {

  public static class FlowDocumentExtensions {
	private static IEnumerable<TextElement> GetRunsAndParagraphs(FlowDocument doc) {
	  for(TextPointer position = doc.ContentStart;
		position != null && position.CompareTo(doc.ContentEnd) <= 0;
		position = position.GetNextContextPosition(LogicalDirection.Forward)) {
		if(position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd) {
		  Run run = position.Parent as Run;

		  if(run != null) {
			yield return run;
		  }
		  else {
			Paragraph para = position.Parent as Paragraph;

			if(para != null) {
			  yield return para;
			}
		  }
		}
	  }
	}

	public static FormattedText GetFormattedText(this FlowDocument doc) {
	  if(doc == null) {
		throw new ArgumentNullException("doc");
	  }

	  FormattedText output = new FormattedText(
		GetText(doc),
		CultureInfo.CurrentCulture,
		doc.FlowDirection,
		new Typeface(doc.FontFamily, doc.FontStyle, doc.FontWeight, doc.FontStretch),
		doc.FontSize,
		doc.Foreground);

	  int offset = 0;

	  foreach(TextElement el in GetRunsAndParagraphs(doc)) {
		Run run = el as Run;

		if(run != null) {
		  int count = run.Text.Length;

		  output.SetFontFamily(run.FontFamily, offset, count);
		  output.SetFontStyle(run.FontStyle, offset, count);
		  output.SetFontWeight(run.FontWeight, offset, count);
		  output.SetFontSize(run.FontSize, offset, count);
		  output.SetForegroundBrush(run.Foreground, offset, count);
		  output.SetFontStretch(run.FontStretch, offset, count);
		  output.SetTextDecorations(run.TextDecorations, offset, count);

		  offset += count;
		}
		else {
		  offset += Environment.NewLine.Length;
		}
	  }

	  return output;
	}

	public static string GetText(this FlowDocument doc) {
	  StringBuilder sb = new StringBuilder();

	  foreach(TextElement el in GetRunsAndParagraphs(doc)) {
		Run run = el as Run;
		sb.Append(run == null ? Environment.NewLine : run.Text);
	  }
	  return sb.ToString();
	}
  }
}
