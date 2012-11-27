var calculator = {
	name: "calculator",
	version: "0.0.1",
	author: "Shellscape, Andrew Powell <andrew@shellscape.org>",
	website: "http://shellscape.org",
	description: "A simple calcuator",
	contributors: [
    { name: "Andrew Powell", email: "andrew@shellscape.org" },
		{ name: "Andrew Powell", email: "andrew@shellscape.org" }
	],
	scripts: {
		jquery: "jquery-1.9.2.min.js",
		"jquery-ui": "foobar.js"
	},
	

	// extension entry point

	register: function(){

	},

	results: function(term){ // shows results for a plugin. eg. dictionary, calculator
		return []; // results
		return null; // no results
	}, 

	execute: function(name, params){
		
	}// executes a commandv
};
