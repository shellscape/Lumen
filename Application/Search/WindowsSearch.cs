using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;

namespace Lumen.Search {

  public class WindowsSearch {
	
	private String _currentTerm = String.Empty;
	private String _last = String.Empty;
	
	private List<String> _terms = new List<String>();
	private List<WindowsSearchResult> _results;
	
	private Thread _worker;
	private AutoResetEvent _flag = new AutoResetEvent(false);
	private object _lock = new object();

	public event EventHandler ResultsChanged;

	public WindowsSearch() {
	  CreateBackgroundThread();
	}

	public List<WindowsSearchResult> Results {
	  get { return _results; }
	}

	public String LastTerm { get { return _last; } }

	/// <summary>
	/// Creates a background thread to run UpdateStringsWorker
	/// </summary>
	private void CreateBackgroundThread() {
	  if(WindowsSearchProvider.IsAvailable) {
		_worker = new Thread(Worker);
		_worker.IsBackground = true;
		_worker.Start();
	  }
	}

	/// <summary>
	/// Sets the flag to invoke a search on the background thread and updates internal search term to prevent duplicate searches.
	/// </summary>
	public void Search(string term) {
	  lock(_lock) {
		
		if(String.IsNullOrEmpty(term)) { // reset
		  _currentTerm = String.Empty;
		  _terms.Clear();
		  OnResultsChanged();
		}
		else if(_terms.Contains(term)) { // do nothing
		  return;
		}
		else { // Set the search term and flag the search thread
		  _currentTerm = term;
		  _flag.Set();
		}
	  }
	}

	/// <summary>
	/// Method to be run on background thread.  Waits on AutoResetEvent until
	/// needed, then performs search and updates ListBox.  
	/// </summary>
	private void Worker(object state) {

	  String data = null;
	  long lastSearch = Environment.TickCount;

	  while(true) {

		_flag.WaitOne();

		lock(_lock) {
		  if(string.IsNullOrEmpty(_currentTerm) || _currentTerm == _last) {
			// Take no action with blank term or same as last term
			OnResultsChanged();
			continue;
		  }
		  else {
			// Copy the search term, then clear it
			data = _currentTerm;
			_currentTerm = String.Empty;
			_last = data;
		  }
		}

		// Enforce a delay to avoid constant searching
		if((Environment.TickCount - lastSearch) < 250) {
		  Thread.Sleep(250);
		}

		// Perform the search then update the list
		//_terms = WindowsSearchProvider.Search(data);
		//_results = _terms.ToArray();
		_results = WindowsSearchProvider.Search(data);

		OnResultsChanged();

		lastSearch = Environment.TickCount;
	  }
	}

	/// <summary>
	/// Indicates that the list of search items has been updated.
	/// </summary>
	private void OnResultsChanged() {
	  if(ResultsChanged != null) {
		ResultsChanged(this, null);
	  }
	}
  }
}
