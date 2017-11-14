var view = (function() {
	this.locationDiv = null;
	this.messageDiv = null;
	this.locationItemsDiv = null;
	this.carriedItemsDiv = null; 
	this.movementDiv = null;
	this.northBtn = null;
	this.eastBtn = null;
	this.southBtn = null;
	this.westBtn = null;
	this.conversationContainer = null;
	this.conversationDiv = null;
	this.settingsContainer = null;
	this.settingsDiv = null;
	that = this;

	return {
		conversation : {
			show : showConversation,
			addStatement : addConversationStatement,
			addResponse : addConversationResponse,
			clear : clearConversation,
			hide : hideConversation,
		},
		settings : {
			show : showSettings,
			//hide : hideSettings
		},
		initialize : initialize,
		update : update,
	}

	function initialize(locationDiv, messageDiv, locationItemsDiv, carriedItemsDiv, movementDiv, northBtn, eastBtn, 
		southBtn, westBtn, conversationContainer, conversationDiv, settingsContainer, settingsDiv) {
		that.locationDiv = locationDiv;
		that.messageDiv = messageDiv;
		that.locationItemsDiv = locationItemsDiv;
		that.carriedItemsDiv = carriedItemsDiv; 
		that.movementDiv = movementDiv;
		that.northBtn = northBtn;
		that.eastBtn = eastBtn;
		that.southBtn = southBtn;
		that.westBtn = westBtn;
		that.conversationContainer = conversationContainer;
		that.conversationDiv = conversationDiv;
		that.settingsContainer = settingsContainer;
		that.settingsDiv = settingsDiv;

		//TODO populateSettingsDiv();
	}

	function update(context) {
	    context.updateLocation();
	    context.updateCharacters();

	    locationDiv.innerHTML = context.location().look(); // TODO + "<br><br>" + game.npcs.descriptions.join("<br><br>");
        
	    console.clear();

    	locationItemsDiv.innerHTML = "";
    	locationItemsDiv.appendChild(document.createTextNode("You see:"));
    	locationItemsDiv.appendChild(document.createElement("br"));
    	locationItemsDiv.appendChild(document.createElement("br"));
    	addItems(context, context.location().items, locationItemsDiv);
    	addItems(context, context.location().characters, locationItemsDiv);

    	carriedItemsDiv.innerHTML = "";
    	carriedItemsDiv.appendChild(document.createTextNode("You are carrying:"));
    	carriedItemsDiv.appendChild(document.createElement("br"));
    	carriedItemsDiv.appendChild(document.createElement("br"));
    	addItems(context, context.inventory(), carriedItemsDiv);

    	movementDiv.innerHTML = "";
    	var exits = context.location().getExits();
	    if (exits.N) {
	        AddMovementButton("N", exits.N)
	    }
	    if (exits.E) {
	        AddMovementButton("E", exits.E)
	    }
	    if (exits.S) {
	        AddMovementButton("S", exits.S)
	    }
	    if (exits.W) {
	        AddMovementButton("W", exits.W)
	    }

	    messageDiv.innerHTML = context.message();

	    function addItems(context, items, div) {
	        items.forEachOpen(
                function (item, container) {
                    var containerText = container == null ? "" : " (in the " + container.caption + ")";
                    var text = document.createTextNode(item.caption + containerText);
                    div.appendChild(text);
                    item.getVerbs(context).forEach(
                        function (caption, func) {
                            var button = document.createElement('button');
                            button.innerHTML = caption;
                            button.onclick = function () { func(context); view.update(context); }
                            div.appendChild(button);
                        });
                    div.appendChild(document.createElement("br"));
                });
	    }

	    function AddMovementButton(direction, targetLocation) {
    	    var button = document.createElement('button');
    	    button.innerHTML = direction;
    	    button.onclick = function () {
    	        context.setLocation(targetLocation);
    	        //context.setMessage("You move to the " + targetLocation.caption);
    	        view.update(context);
    	    }
    	    movementDiv.appendChild(button);
    	}
	}

	// ---------- The following functions generate Javascript code.
	
    /*
	function jsVerbName(verb) {
		return verb.name == null ? verb : verb.name;
	}

	function jsUpdateFunction() {
		return "view.update()";
	}

	function jsVerbFunction(item, verb) {
		return "game.apply(" + item.name + ", \"" + jsVerbName(verb) + "\")";
	}
    */

	function showConversation() {
		that.conversationContainer.classList.add("target");
		that.conversationDiv.classList.add("keypad"); // TODO not keypad
	}

	function addConversationStatement(msg) {
		that.conversationDiv.innerHTML += msg;
		that.conversationDiv.innerHTML += "<br>";
	}

	function addConversationResponse(msg, target) {
		that.conversationDiv.innerHTML += "<a href='#' onclick='" + target + "'>" + msg + "</a>";
		that.conversationDiv.innerHTML += "<br>";
	}

	function clearConversation() {
		that.conversationDiv.innerHTML = "";
	}

	function hideConversation() {
		that.conversationContainer.classList.remove("target");
	}

	// ---------- Settings

	function showSettings() {
		that.settingsContainer.classList.add("target");
		that.settingsDiv.classList.add("keypad"); // TODO not keypad
	}

	function createButton(caption, clickFunction) {
		var btn = document.createElement("button");
		btn.appendChild(document.createTextNode(caption));
		btn.addEventListener('click', clickFunction);
		return btn;
	}

	function createSelect(items, selectFunction){
		var sel = document.createElement("select");
		items.forEach(item => {
			var option = document.createElement("option");
			option.text = item;
			sel.add(option)
		});
		sel.addEventListener('click', selectFunction)
		return sel;
	}

	function createEdit(text){
		var ed = document.createElement("input");
		ed.setAttribute("type", "text");
		ed.setAttribute("value", text);
		return ed;
	}

	function createLineBreak() {
		return document.createElement("br");
	}

	function populateSettingsDiv() {
		settingsDiv.innerHTML = "";
		settingsDiv.appendChild(document.createTextNode("Games: "));
		settingsDiv.appendChild(createSelect(game.listGames(), () => console.log("hi"))); // TODO
		settingsDiv.appendChild(createEdit(""));
		settingsDiv.appendChild(createLineBreak());
		settingsDiv.appendChild(createButton("Load", loadGame));
		settingsDiv.appendChild(createButton("Save", saveGame));
		settingsDiv.appendChild(createButton("Cancel", hideSettings));
	}

	function loadGame() {
		// todo
		hideSettings();
	}	

	function saveGame() {
		// todo
		hideSettings();
	}	

	function hideSettings() {
		that.settingsContainer.classList.remove("target");
	}

	/*
	function htmlSettings() {
		var html = "Games: <select>";
		game.listGames().forEach(g => html += "<option value='" + g + "'>" + g + "</option>");
		html += "</select><input type='text' name='FirstName' value='Mickey'>";
		html += htmlButton("Load", "game.loadGame(\"2\");view.settings.hide()");
		html += htmlButton("Save", "view.settings.hide()");
		html += htmlButton("Cancel", "that.hideSettings()");
		return html;
	}
	*/

})();
