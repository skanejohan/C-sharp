var THESEUS = THESEUS || {};

// Initialized with the proper UI elements, implements a keypad.
THESEUS.Keypad = function() {
	var _code = "";
	var _expectedCode = "";
	var _onSuccess = code => {};
	var _onFailure = code => {};
	var _keypadDiv = null;
	var _codeDiv = null;

	return {
		initialize : initialize,
		enterCombination: enterCombination,
        enterDigit : enterDigit,
	}
    
	function initialize(keypadDiv, codeDiv, btn0, btn1, btn2, btn3, btn4, btn5, btn6, btn7, btn8, btn9) {
		_keypadDiv = keypadDiv;
		_codeDiv = codeDiv;
		btn0.onclick = () => { enterDigit("0"); } 
		btn1.onclick = () => { enterDigit("1"); } 
		btn2.onclick = () => { enterDigit("2"); } 
		btn3.onclick = () => { enterDigit("3"); } 
		btn4.onclick = () => { enterDigit("4"); } 
		btn5.onclick = () => { enterDigit("5"); } 
		btn6.onclick = () => { enterDigit("6"); } 
		btn7.onclick = () => { enterDigit("7"); } 
		btn8.onclick = () => { enterDigit("8"); } 
		btn9.onclick = () => { enterDigit("9"); } 
	}

	function enterDigit(d) { 
	    _code += d;
	    update();
	}

	function enterCombination(expectedCode, enteredCode, onSuccess, onFailure) {
		_code = enteredCode != null ? enteredCode : "";
		_expectedCode = expectedCode;
		_onSuccess = onSuccess;
		_onFailure = onFailure;
		_keypadDiv.classList.add("target");
		update();
	}

    function update() {
    	_codeDiv.innerHTML = "Code: " + _code;
    	if (_code.length == _expectedCode.length) {
    		_keypadDiv.classList.remove("target");
	    	//TODO state.add("game.apply(" + item.name + ", 'enterCombination', '" + code + "')");
    		if (_code === _expectedCode) {
    			_onSuccess();
    		}
    		else {
    			_onFailure();
    		}
    	} 
	}
};

// Constructs the Theseus view model.
THESEUS.ViewModel = function() {
    var message = new VMMessage(); 
    var location = new VMLocation();
    var items = new VMItems();
    var inventory = new VMItems();
    var navigation = new VMNavigation();
    var conversation = new VMConversation();
    var settings = new VMSettings();
    
    return {
        message: message, 
        location: location,
        items: items,
        inventory: inventory,
        navigation: navigation,
        conversation: conversation,
        settings: settings,
        initialize: function(locationDiv, messageDiv, locationItemsDiv, 
            carriedItemsDiv, movementDiv, conversationContainer, 
            conversationDiv, settingsContainer, settingsDiv) {
            message.initialize(messageDiv);
            location.initialize(locationDiv);
            items.initialize(locationItemsDiv);
            inventory.initialize(carriedItemsDiv);
            navigation.initialize(movementDiv);
            conversation.initialize(conversationContainer, conversationDiv);
            settings.initialize(settingsContainer, settingsDiv);
        },
		update: function(context) {
            context.updateLocation();
            context.updateCharacters();
            message.update(context);
            location.update(context);
            items.update(context, "You see:", context.location().items, context.location().characters);
            inventory.update(context, "You carry:", context.inventory());
            navigation.update(context);
            conversation.update(context);
            settings.update(context);
        },
    }

    function VMMessage() {
        var messageDiv;

        return {
            initialize: function(div) {
                messageDiv = div;
            },
            update(context) {
                messageDiv.innerHTML = context.message();
            },
        }
    }

    function VMLocation() {
        var locationDiv;

        return {
            initialize: function(div) {
                locationDiv = div;
            },
            update(context) {
                locationDiv.innerHTML = context.location().look(context);
                locationDiv.innerHTML += "<br><br>";
                context.location().characters.forEach(c => {
                    locationDiv.innerHTML += c.caption + " is also here.<br>";
                });
            },
        }
    }

    function VMItems() {
        var itemsDiv;

        return {
            initialize: function(div) {
                itemsDiv = div;
            },
            update(context, header, items, characters) {
                itemsDiv.innerHTML = "";
                itemsDiv.appendChild(document.createTextNode(header));
                itemsDiv.appendChild(document.createElement("br"));
                itemsDiv.appendChild(document.createElement("br"));
                addItems(context, items, itemsDiv);
                if (characters != null){
                    addCharacters(context, characters, itemsDiv);
                }
            },
        }

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
                            button.onclick = function () { 
                                func(context); 
                                THESEUS.view.update(context); 
                            }
                            div.appendChild(button);
                        });
                    div.appendChild(document.createElement("br"));
                });
	    }

	    function addCharacters(context, characters, div) {
	    	characters.forEach(
	    		function(character) {
	                var text = document.createTextNode(character.caption);
	                div.appendChild(text);
	                character.getVerbs(context).forEach(
	                    function (caption, func) {
	                        var button = document.createElement('button');
	                        button.innerHTML = caption;
	                        button.onclick = function () { 
                                func(context); 
                                THESEUS.view.update(context); 
                            }
	                        div.appendChild(button);
	                    });
	                div.appendChild(document.createElement("br"));
	    		});
	    }
    }

    function VMNavigation() {
        var navigationDiv;

        return {
            initialize: function(div) {
                navigationDiv = div;
            },
            update(context) {
                movementDiv.innerHTML = "";
                var exits = context.location().getExits();
                if (exits.N) {
                    AddMovementButton(context, "N", exits.N)
                }
                if (exits.E) {
                    AddMovementButton(context, "E", exits.E)
                }
                if (exits.S) {
                    AddMovementButton(context, "S", exits.S)
                }
                if (exits.W) {
                    AddMovementButton(context, "W", exits.W)
                }
            },
        }

        function AddMovementButton(context, direction, targetLocation) {
    	    var button = document.createElement('button');
    	    button.innerHTML = direction;
    	    button.onclick = function () {
    	        context.setLocation(targetLocation);
    	        THESEUS.view.update(context);
    	    }
    	    movementDiv.appendChild(button);
    	}
    }

    function VMConversation() {
        var conversationDiv;
        var conversationContainer;
        
        return {
            initialize: function(container, div) {
                conversationDiv = div;
                conversationContainer = container;
            },
            update: function(context) {
            },
            show: function() {
                conversationContainer.classList.add("target");
                conversationDiv.classList.add("conversation");
            },
			addStatement: function(msg) {
                conversationDiv.innerHTML += msg;
                conversationDiv.innerHTML += "<br>";
            },
            addResponse: function(msg, responseId, nextStatementId) {
                var target = "THESEUS.conversation.respond(" + responseId + ", " + nextStatementId + ")";
                conversationDiv.innerHTML += "<a href='#' onclick='" + target + "'>" + msg + "</a>";
                conversationDiv.innerHTML += "<br>";
            },
			clear: function() {
                conversationDiv.innerHTML = "";
            },
            hide: function() {
                conversationContainer.classList.remove("target");
            },
        }
    }
    
    function VMSettings() {
        var settingsDiv;

        return {
            initialize: function(div) {
                settingsDiv = div;
            },
            update(context) {
                /* TODO
                function showSettings() {
                    _settingsContainer.classList.add("target");
                    _settingsDiv.classList.add("keypad"); // TODO not keypad
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
                    _settingsContainer.classList.remove("target");
                }
            
                function htmlSettings() {
                    var html = "Games: <select>";
                    game.listGames().forEach(g => html += "<option value='" + g + "'>" + g + "</option>");
                    html += "</select><input type='text' name='FirstName' value='Mickey'>";
                    html += htmlButton("Load", "game.loadGame(\"2\");view.settings.hide()");
                    html += htmlButton("Save", "view.settings.hide()");
                    html += htmlButton("Cancel", "_hideSettings()");
                    return html;
                }
                */
            }
        }
    }
}

THESEUS.keypad = new THESEUS.Keypad();
THESEUS.view = new THESEUS.ViewModel();
