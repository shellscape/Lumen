
var __extension,
	require = function (what) {
		lumen.require(what); // the way IActiveScript adds objects is messed up.
	},
	extension = function (name, extension) {
		lumen.extensionName = name;
		__extension = extension;
	};
