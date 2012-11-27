var extension = {
	name: "calculator",
	version: "0.0.1",
	author: "Shellscape, Andrew Powell <andrew@shellscape.org>",
	website: "http://shellscape.org",
	description: "A simple calcuator",
	contributors: [
    { name: "Andrew Powell", email: "andrew@shellscape.org" }
	],
	require: [
		"lib/jquery-1.8.2.min.js"
	],
	

	// extension entry point

	register: function(){

	},

	results: function(term){ // shows results for a plugin. eg. dictionary, calculator
		var foo = jquery;
		return []; // results
		return null; // no results
	}, 

	execute: function(name, params){
		
	}// executes a commandv
};
