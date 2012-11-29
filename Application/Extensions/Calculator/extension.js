/*!
 * calculator @VERSION
 * http://shellscape.org
 *
 * Copyright 2012 <author> and other contributors
 * Released under the MIT license.
 * http://github.com/shellscape/Lumen/LICENSE
 *
 * http://shellscape.org
 *
 * Depends:
 *   <script>.js
 */
(function (lumen, undefined) {

	// TODO: get the require syntax working.
	// TODO: figure out how to load these scripts before the extension script. not doing so limits where the required libs can be used.
	//require("lib");
	require("lib/underscore-1.4.2.js");
	
	extension("calculator", {
		version: "0.0.1",
		author: "Shellscape, Andrew Powell <andrew@shellscape.org>",
		website: "http://shellscape.org",
		description: "A simple calcuator",
		contributors: [
			{ name: "Andrew Powell", email: "andrew@shellscape.org" }
		],

		commands: {
			"open calculator": function () {
				lumen.run("calc.exe");
			},
			"calc": function (params) {
				lumen.alert(eval(params));
			}
		},

		results: function (term) { // shows results for a plugin. eg. dictionary, calculator
			var result;
			try {
				result = eval(term);
			}
			catch(e){}

			return isNaN(result) ? null : [{ text: result, command: "calc.exe" }];
		}
		
	});
	
})(lumen);