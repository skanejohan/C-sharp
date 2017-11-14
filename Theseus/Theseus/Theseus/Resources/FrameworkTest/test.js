function beginTest(testName){
	_tests = 0;
	_successful = 0;
	_testName = testName;
}

function assert(condition){
	_tests++;
	if (condition){
		_successful++;
	}
	console.assert(condition);
}

function endTest(){
	console.log(_testName + ": number of tests: " + _tests);
	console.log(_testName + ": successful tests: " + _successful);
	console.assert(_tests == _successful);

}

var _tests = 0;
var _successful = 0;
var _testName = "";