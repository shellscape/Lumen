using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Data.OleDb;
using System.Linq;

namespace Lumen.Search {

	public static class WindowsSearchProvider {
		// Shared connection used for any search index queries

		private static OleDbConnection _connection;

		static WindowsSearchProvider() {
			try {
				_connection = new OleDbConnection("Provider=Search.CollatorDSO;Extended Properties='Application=Windows';");

				_connection.Open();
			}
			catch { // fails if desktop search is disabled or not installed
				_connection = null;
			}
		}

		/// <summary>
		/// Indicates if Winders Desktop Search is installed and enabled.
		/// </summary>
		public static bool IsAvailable {
			get { return ((_connection != null)); }
		}

		public class thing {
			public object reader { get; set; }
		}

		public static List<WindowsSearchResult> Search(string term) {

			var list = new List<WindowsSearchResult>();

			// prevent hidden (0x2) and system (0x4) files
			var sql = @"select top 1000 
		  System.ItemNameDisplay, System.ItemPathDisplay, System.Kind, System.Search.Rank, System.FileAttributes 
		  from systemindex 
		  where CONTAINS(""System.ItemNameDisplay"", '""*{0}*""')
		  and System.FileAttributes <> ALL BITWISE 0x4 and System.FileAttributes <> ALL BITWISE 0x2 order by System.Search.Rank";

			using (OleDbCommand command = new OleDbCommand()) {
				command.Connection = _connection;
				command.CommandText = string.Format(sql, term.Trim());

				using (OleDbDataReader reader = command.ExecuteReader()) {

					while (reader.Read()) {

						var result = new WindowsSearchResult() {
							FileName = reader["System.ItemNameDisplay"].ToString(),
							FilePath = reader["System.ItemPathDisplay"].ToString(),
							Rank = (int)reader["System.Search.Rank"]
						};

						String[] kinds = reader["System.Kind"] as String[];
						if (kinds != null && kinds.Length >= 1) {
							foreach (var k in reader["System.Kind"] as String[]) {
								result.Kind |= (WindowsSearchKind)Enum.Parse(typeof(WindowsSearchKind), k);
							}
						}
						else {
							result.Kind = WindowsSearchKind.file;
						}

						list.Add(result);
					}
				}
			}

			var results = list.Distinct(p => p.FileName).ToList();

			return results;
		}

		/// <summary>
		/// Returns values from the search index for the given filename based on the supplied list of columns/properties
		/// </summary>
		/// <param name="columns">Comma-separated list of columns/properties (http://msdn2.microsoft.com/en-us/library/ms788673.aspx)</param>
		/// <param name="filename">Filename about which to retrieve data</param>
		/// <returns>Key/value pairs for all requested columns/properties</returns>
		/// <remarks></remarks>
		public static Dictionary<string, string> GetProperties(string columns, string filename) {
			Dictionary<string, string> items = new Dictionary<string, string>();

			// The search provider does not support SQL parameters
			filename = filename.Replace("'", "''");

			OleDbCommand cmd = new OleDbCommand();
			cmd.Connection = _connection;
			cmd.CommandText = string.Format("SELECT {0} from systemindex WHERE System.ItemNameDisplay = '{1}'", columns, filename);

			OleDbDataReader results = cmd.ExecuteReader();
			string value = null;
			string key = null;

			if (results.Read()) {
				int i = 0;
				while ((i < results.FieldCount)) {
					if (!results.IsDBNull(i)) {
						key = results.GetName(i);
						if (((results.GetValue(i)) is string[])) {
							string[] arrayValue = results.GetValue(i) as String[];
							if ((arrayValue.GetLength(0) == 1)) {
								value = arrayValue[0];
							}
							else {
								value = string.Join(", ", arrayValue);
							}
						}
						else {
							value = results.GetValue(i).ToString();
						}
						if (key.StartsWith("System.")) {
							key = key.Substring(7);
						}
						items.Add(key, value);
					}
					i = (i + 1);
				}
			}

			results.Close();

			return items;
		}
	}
}