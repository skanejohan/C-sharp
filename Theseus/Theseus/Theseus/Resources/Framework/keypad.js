var keypad = (function() {
	var code = "";
	var expectedCode = "";
	var onSuccess = code => {};
	var onFailure = code => {};
	var keypadDiv = null;
	var codeDiv = null;
	var item = null;

	that = this;

	return {
		initialize : initialize,
		enterCombination: enterCombination,
        enterDigit : enterDigit,
	}
    
	function initialize(keypadDiv, codeDiv, btn0, btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9) {
		that.keypadDiv = keypadDiv;
		that.codeDiv = codeDiv;
		btn0.onclick = () => { that.code += "0"; update(); } 
		btn1.onclick = () => { that.code += "1"; update(); } 
		btn2.onclick = () => { that.code += "2"; update(); } 
		btn3.onclick = () => { that.code += "3"; update(); } 
		btn4.onclick = () => { that.code += "4"; update(); } 
		btn5.onclick = () => { that.code += "5"; update(); } 
		btn6.onclick = () => { that.code += "6"; update(); } 
		btn7.onclick = () => { that.code += "7"; update(); } 
		btn8.onclick = () => { that.code += "8"; update(); } 
		btn9.onclick = () => { that.code += "9"; update(); } 
	}

	function enterDigit(d) {
	    that.code += d;
	    update();
	}

	function enterCombination(item, expectedCode, enteredCode, onSuccess, onFailure) {
		that.item = item;
		that.code = enteredCode != null ? enteredCode : "";
		that.expectedCode = expectedCode;
		that.onSuccess = onSuccess;
		that.onFailure = onFailure;
		that.keypadDiv.classList.add("target");
		update();
	}

    function update() {
    	that.codeDiv.innerHTML = "Code: " + that.code;
    	if (that.code.length == that.expectedCode.length) {
    		that.keypadDiv.classList.remove("target");
	    	state.add("game.apply(" + that.item.name + ", 'enterCombination', '" + that.code + "')");
    		if (that.code === that.expectedCode) {
    			that.onSuccess();
    		}
    		else {
    			that.onFailure();
    		}
    	} 
	}
})();
