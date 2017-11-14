var collection = function(){
	var data = {};

	function add(k, v){
		data[k] = v;
	}

	function remove(k){
		delete data[k];
	}

	function value(k){
		return data[k];
	}

	function has(k){
		return k in data;
	}

    function forEach(func){
    	for (var key in data) {
    		if (data.hasOwnProperty(key)) {
    			func(key, data[key]);
    		}
	    }
	}

	function concat(coll){
		coll.forEach(function(k, v) { add (k, v); });
	}

	return {
		add : add,
		remove : remove,
		value : value,
		has : has,
		forEach : forEach,
		concat : concat
	}
}
