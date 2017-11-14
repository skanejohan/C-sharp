var conversation = (function() {
	var statements = {};
	var responses = {};
	var showConversation = null;
	var addStatementToView = null;
	var addResponseToView = null;
	var clearView = null;
	var hideConversation = null;
	that = this;

	return {
		initialize : initialize,
		addStatement : addStatement,
		setResponses : setResponses,
		startConversation : startConversation,
		respond : respond,
		end : end
	}	

	function initialize(showConversation, addStatement, addResponse, clear, hideConversation) {
		that.showConversation = showConversation;
		that.addStatementToView = addStatement;
		that.addResponseToView = addResponse;
		that.clearView = clear;
		that.hideConversation = hideConversation;
	}

	function addStatement(id, func) {
		statements[id] = func;
	}

	function setResponses(id, ids) {
		responses[id] = ids;
	}

	function startConversation(id) {
		that.showConversation();
		respond(0, id);
	}

	function respond(responseId, newStatementId) {
		if (newStatementId == 0) {
		    conversation.end();
		    view.update(context);
			return;
		}

		that.clearView();
		var func = statements[newStatementId];
 		that.addStatementToView(func());
 		responses[newStatementId].forEach(
  			r_id => {
  				var func = statements[r_id];
  				var id = responses[r_id][0];
			  	that.addResponseToView(func(), "conversation.respond(" + r_id + ", " + id + ")");
  			});
	}

	function end() {
		that.hideConversation();
	}

})();


