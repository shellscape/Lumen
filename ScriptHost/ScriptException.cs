using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;

namespace Lumen.Scripting {

  /// <summary>
  /// Defines a Windows Script Engine exception.
  /// </summary>
  [Serializable]
  public class ScriptException : Exception {
	/// <summary>
	/// Initializes a new instance of the <see cref="ScriptException"/> class.
	/// </summary>
	public ScriptException()
	  : base("Script Exception") {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ScriptException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	public ScriptException(string message)
	  : base(message) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ScriptException"/> class.
	/// </summary>
	/// <param name="innerException">The inner exception.</param>
	public ScriptException(Exception innerException)
	  : base(null, innerException) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ScriptException"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="innerException">The inner exception.</param>
	public ScriptException(string message, Exception innerException)
	  : base(message, innerException) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ScriptException"/> class.
	/// </summary>
	/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
	/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
	/// <exception cref="T:System.ArgumentNullException">
	/// The <paramref name="info"/> parameter is null.
	/// </exception>
	/// <exception cref="T:System.Runtime.Serialization.SerializationException">
	/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
	/// </exception>
	protected ScriptException(SerializationInfo info, StreamingContext context)
	  : base(info, context) {
	}

	/// <summary>
	/// Gets the error description intended for the customer.
	/// </summary>
	/// <value>The description text.</value>
	public string Description { get; internal set; }

	/// <summary>
	/// Gets the line number of error.
	/// </summary>
	/// <value>The line number.</value>
	public int Line { get; internal set; }

	/// <summary>
	/// Gets the character position of error.
	/// </summary>
	/// <value>The column number.</value>
	public int Column { get; internal set; }

	/// <summary>
	/// Gets a value describing the error.
	/// </summary>
	/// <value>The error number.</value>
	public int Number { get; internal set; }

	/// <summary>
	/// Gets the text line in the source file where an error occurred.
	/// </summary>
	/// <value>The text.</value>
	public string Text { get; internal set; }
  }

}