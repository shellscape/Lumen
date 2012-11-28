using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumen.Scripting {

	public enum ScriptErrorType : int {

		[Description("None")]
		E_NONE = 0,

		// Type Errors

		[Description("TypeError")]
		E_NOT_DATE = -2146823282,

		[Description("TypeError")]
		E_NOT_BOOL = -2146823278,

		[Description("TypeError")]
		E_ARG_NOT_OPT = -2146827839,

		[Description("TypeError")]
		E_NO_PROPERTY = -2146827850,

		[Description("TypeError")]
		E_NOT_NUM = -2146823287,

		[Description("TypeError")]
		E_INVALID_CALL_ARG = -2146828283,

		[Description("TypeError")]
		E_NOT_FUNC = -2146823286,

		[Description("TypeError")]
		E_OBJECT_EXPECTED = -2146823281,

		[Description("TypeError")]
		E_OBJECT_REQUIRED = -2146827864,

		[Description("TypeError")]
		E_UNSUPPORTED_ACTION = -2146827843,

		[Description("TypeError")]
		E_NOT_VBARRAY = -2146823275,

		[Description("TypeError")]
		E_INVALID_DELETE = -2146823276,

		[Description("TypeError")]
		E_JSCRIPT_EXPECTED = -2146823274,

		[Description("TypeError")]
		E_NOT_ARRAY = -2146823257,

		// Syntax Errors

		[Description("SyntaxError")]
		E_SYNTAX_ERROR = -2146827286,

		[Description("SyntaxError")]
		E_LBRACKET = -2146827283,

		[Description("SyntaxError")]
		E_RBRACKET = -2146827282,

		[Description("SyntaxError")]
		E_SEMICOLON = -2146827284,

		[Description("SyntaxError")]
		E_UNTERMINATED_STR = -2146827273,

		[Description("SyntaxError")]
		E_DISABLED_CC = -2146827258,

		[Description("SyntaxError")]
		E_INVALID_BREAK = -2146827269,

		[Description("SyntaxError")]
		E_INVALID_CONTINUE = -2146827268,

		[Description("SyntaxError")]
		E_LABEL_NOT_FOUND = -2146827262,

		[Description("SyntaxError")]
		E_LABEL_REDEFINED = -2146827263,

		[Description("SyntaxError")]
		E_SYNTAX_EXPECTED = -2146827279,

		E_SYNTAX_UNKNOWN2 = -2146827281,
		E_SYNTAX_UNKNOWN3 = -2146827280,


		// Reference Errors

		[Description("ReferenceError")]
		E_ILLEGAL_ASSIGN = -2146823280,

		[Description("ReferenceError")]
		E_UNDEFINED = -2146823279,

		// Range Errors

		[Description("RangeError")]
		E_INVALID_LENGTH = -2146823259,

		[Description("RangeError")]
		E_PRECISION_OUT_OF_RANGE = -2146823261,

		[Description("RangeError")]
		E_FRACTION_DIGITS_OUT_OF_RANGE = -2146823262,

		[Description("RangeError")]
		E_SUBSCRIPT_OUT_OF_RANGE = -2146828279,

		[Description("RegExpError")]
		E_REGEXP_SYNTAX_ERROR = -2146823271,


		[Description("URIError")]
		E_URI_INVALID_CHAR = -2146823264,

		[Description("URIError")]
		E_URI_INVALID_CODING = -2146823263

	}

}