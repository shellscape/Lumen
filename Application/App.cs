﻿using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Lumen {

	/// <summary>
	/// App. Copied from the file generated by App.xaml
	/// </summary>
	public partial class App : System.Windows.Application {

		/// <summary>
		/// InitializeComponent
		/// </summary>
		[DebuggerNonUserCodeAttribute()]
		[GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent() {

			#line 4 "..\..\App.xaml"
			this.StartupUri = new System.Uri("Windows/Main.xaml", System.UriKind.Relative);

			#line default
			#line hidden
		}

		/// <summary>
		/// Application Entry Point.
		/// </summary>
		[STAThreadAttribute()]
		public static void Main() {
			Lumen.App app = new Lumen.App();
			app.InitializeComponent();


			String exePath = Directory.GetParent(System.Reflection.Assembly.GetExecutingAssembly().Location).FullName;
			String extensionsPath = System.IO.Path.Combine(exePath, "Extensions");
			String calcPath = System.IO.Path.Combine(extensionsPath, "Calculator");

			var extension = new Extension(calcPath);
			
			app.Run();
		}
	}
}
