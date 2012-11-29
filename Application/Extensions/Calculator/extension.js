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
	//require("lib");
	//require("lib/jquery-1.8.2.min.js");

	lumen.extension("calculator", {
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
				alert(eval(params));
			}
		},

		results: function (term) { // shows results for a plugin. eg. dictionary, calculator
			var result = eval(term);
			return isNaN(result) ? null : [result];
		}


	});

})(lumen);