var state = (function() {
	this.actions = [];
	this.applying = false;
	that = this;

	return {
		add : add,
		clear : clear,
		apply : apply,
		toString : toString,
		fromString : fromString,
	}

	function add(func) {
		if (!that.applying) {
			that.actions.push(func);
		}
	}

	function clear() {
		that.actions = [];
	}

	function apply() {
		that.applying = true;
		that.actions.forEach(elem => new Function(elem)());
		that.applying = false;
	}

	function toString() {
		return that.actions.join(";");
	}

	function fromString(s) {
		clear();
		s.split(";").forEach(add);
		apply();
	}

})();