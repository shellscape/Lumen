/*!
 * <extensionName> @VERSION
 * <extensionUrl>
 *
 * Copyright 2012 <author> and other contributors
 * Released under the MIT license.
 * <licenseUrl>
 *
 * <docsUrl>
 *
 * Depends:
 *   <script>.js
 */
(function( lumen, undefined ) {

// TODO: get the require syntax working.
//require("lib");
//require("lib/jquery-1.8.2.min.js");

lumen.extension( "<extensionName>", {
	version: "@VERSION",
	
	//optional info
	author: "<author>",
	website: "<extensionUrl>",
	description: "<extensionDesc>",
	contributors: [
    //{ name: "Andrew Powell", email: "andrew@shellscape.org" }
	],
	// end - optional info
	
	commands: {
		//"calc": function(params){
		//	alert(eval(params));
		//}
	},

	results: function(term){ // shows results for a plugin. eg. dictionary, calculator
		//return []; // results
		//return null; // no results
	}, 


});

})( lumen );