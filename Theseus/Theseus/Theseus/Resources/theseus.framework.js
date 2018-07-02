"use strict";

var THESEUS = THESEUS || {};

// A simple list implementation, building upon the array construct.
THESEUS.List = function() {
    var items = []

     return {
        add: add,
        has: has,
        remove: remove,
        forEach: forEach,
        by: by,
    }

   function add(item){
        if (!has(item)) { 
            items.push(item) 
        }
    }

    function has(item) {
        return items.indexOf(item) > -1;
    }

    function remove(item) {
        var idx = items.indexOf(item);
        if (idx > -1) {
            items.splice(idx, 1);
        }
    }

    function by(prop, value) {
        for (let i = 0; i < items.length; i++) {
            if (items[i][prop] === value) {
                return items[i];
            }
        }
    }

    function forEach(fn) {
        for (let i = 0; i < items.length; i++) {
            fn(items[i]);
        }
    }
};

// A simple key-value store, building upon the object construct.
THESEUS.Collection = function() {
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
    	for (let key in data) {
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

// A collection of THESEUS.Item objects, supporting container/contained
// in e.g. traversal methods. 
THESEUS.Items = function () {
    var items = []

    var add = function(item){
        if (!has(item)) { 
            items.push(item) 
        }
    }

    var has = function (item) {
        return items.indexOf(item) > -1;
    }

    var byName = function (name) {
        var item = null;
        forEach(
            (i, c) => {
                if (i.caption == name) {
                    item = i;
                }
            });
        return item;
    }

    var remove = function (item) {
        var idx = items.indexOf(item);
        if (idx > -1) {
            items.splice(idx, 1);
        }
        else {
            forEach(i => { i.containedItems.remove(item); });
        }
    }

    var forEach = function (fn) {
        for (let i = 0; i < items.length; i++) {
            fn(items[i]);
            items[i].containedItems.forEach(fn, items[i]);
        }
    }

    var forEachOpen = function (fn, container) {
        if (container == undefined) {
            container = null;
        }
        for (let i = 0; i < items.length; i++) {
            if (items[i].isVisible()) {
                fn(items[i], container);
                if (items[i].isOpen() || !(items[i].isPickable() || items[i].isOpenable() || items[i].isLockable())) {
                    items[i].containedItems.forEachOpen(fn, items[i]);
                }
            }
        }
    }

    return {
        add : add,
        has: has,
        byName: byName,
        remove: remove,
        length: () => items.length,
        forEach: forEach,
        forEachOpen: forEachOpen,
        toArray: () => items,
    }
};

// Constructs an item.
THESEUS.Item = function(info) {
    var isVisible = withDefaultValue(info.isVisible, true);
    
    var isOpenable = withDefaultValue(info.isOpenable, false);
    var isOpen = withDefaultValue(info.isOpen, false);
    var hasBeenOpen = isOpen;
    var isClosed = withDefaultValue(info.isClosed, false);
    var hasBeenClosed = isClosed;

    var isLockable = withDefaultValue(info.isLockable, false);
    var isLocked = withDefaultValue(info.isLocked, false);
    var hasBeenLocked = isLocked;
    var isUnlocked = withDefaultValue(info.isUnlocked, false);
    var hasBeenUnlocked = isUnlocked;

    var isPickable = withDefaultValue(info.isPickable, false);
    var isPicked = false;
    var hasBeenPicked = false;

    var isFixed = withDefaultValue(info.isFixed, false);

    var visibleFunctions = {};

    var containedItems = new THESEUS.Items();
    if (info.containedItems != undefined){
        for (let i = 0; i < info.containedItems.length; i++) {
            containedItems.add(info.containedItems[i]);
        }
    }

    function withDefaultValue(prop, def){
        return prop === undefined ? def : prop;
    }

    var result = {
        caption: info.caption,
        isVisible: () => isVisible,
        setVisible: value => {
            isVisible = value;
        },
        isOpenable: () => isOpenable,
        isOpen: () => isOpen,
        hasBeenOpen: () => hasBeenOpen,
        isClosed: () => isClosed,
        hasBeenClosed: () => hasBeenClosed,
        isLockable: () => isLockable,
        isLocked: () => isLocked,
        hasBeenLocked: () => hasBeenLocked,
        isUnlocked: () => isUnlocked,
        hasBeenUnlocked: () => hasBeenUnlocked,
        isPickable: () => isPickable,
        isPicked: () => isPicked,
        hasBeenPicked: () => hasBeenPicked,
        functionIsVisible: f => visibleFunctions[f],
        setFunctionVisible: (f,v) => visibleFunctions[f] = v,
        containedItems: containedItems,
        getVerbs: getVerbs,
    }

    function getVerbs(context) {
        var verbs = new THESEUS.Collection();
        if (info.examine != undefined) {
            verbs.add("Examine", info.examine);
        }
        if (!isFixed){
            if (context.inventory().has(result)) {
                verbs.add("Drop", drop);
            }
            else {
                verbs.add("Take", take);
            }
        }
        if (isOpenable || isLockable || isPickable) {
            if (isOpen) {
                verbs.add("Close", close);
            }
            if (isClosed && !isLocked) {
                verbs.add("Open", open);
            }
        }
        if (isLockable && info.requiredCombination != undefined) {
            if (isClosed && !isLocked) {
                verbs.add("Lock", lock);
            }
            if (isLocked) {
                verbs.add("Enter combination", enterCombination);
            }
        }
        else if (isLockable && info.requiredKey != undefined) {
            if (isClosed && !isLocked && context.inventory().has(info.requiredKey)){
                verbs.add("Lock", lock);
            }
            if (isLocked && context.inventory().has(info.requiredKey)){
                verbs.add("Unlock", unlock);
            }
        }
        else if (isPickable && info.requiredKey != undefined) {
            if (isLocked && context.inventory().has(info.requiredKey)) {
                verbs.add("Pick", pick);
            }
        }

        if (info.getAdditionalVerbs != undefined) {
            info.getAdditionalVerbs(verbs);
        }
        return verbs;
    }

    function drop(context) {
        context.inventory().remove(result);
        context.location().items.add(result);
        context.setMessage("You drop the " + info.caption + ".");
        processAfter(context, "Drop");
    }

    function take(context) {
        context.inventory().remove(result);
        context.inventory().add(result);
        context.historicInventory().add(result);
        context.location().items.remove(result);
        context.setMessage("You take the " + info.caption + ".");
        processAfter(context, "Take");
    }

    function close(context) {
        isOpen = false;
        isClosed = true;
        hasBeenClosed = true;
        context.setMessage("You close the " + info.caption + ".");
        processAfter(context, "Close");
    }

    function open(context) {
        isOpen = true;
        isClosed = false;
        hasBeenOpen = true;
        context.setMessage("You open the " + info.caption + ".");
        processAfter(context, "Open");
    }

    function lock(context) {
        isLocked = true;
        hasBeenLocked = true;
        context.setMessage("You lock the " + info.caption + ".");
        processAfter(context, "Lock");
    }

    function enterCombination(context, combination) {
        var correctFunction = function() {
            isLocked = false;
            hasBeenUnlocked = true;
            context.setMessage("You enter the correct combination and unlock the " + info.caption + ".");
            processAfter(context, "EnterCombination");
        } 


        var incorrectFunction = function() {
            context.setMessage("For a moment there, you thought you remembered the code. A futile attempt.");
            processAfter(context, "EnterCombination");
        }

        if (THESEUS.view == undefined) {
            if (info.requiredCombination == combination) {
                correctFunction();
            }
            else {
                incorrectFunction();
            }
        }
        else {
            THESEUS.keypad.enterCombination(info.requiredCombination, combination,
                () => {
                    correctFunction();
                    THESEUS.view.update(context);
                },
                () => {
                    incorrectFunction();
                    THESEUS.view.update(context);
                });
        }
    }

    function unlock(context) {
        context.setMessage("You unlock the " + info.caption + ".");
        isLocked = false;
        hasBeenUnlocked = true;
        processAfter(context, "Unlock");
    }

    function pick(context) {
        isLocked = false;
        hasBeenUnlocked = true;
        isPicked = true;
        hasBeenPicked = true;
        context.setMessage("Using the " + info.requiredKey.caption + ", you manage to pick the " + info.caption + ".");
        processAfter(context, "Pick");
    }

    function processAfter(context, verb){
        var v = "after" + verb + "Once";
        if (info[v] != undefined) {
            info[v](context);
            delete info[v];
        }

        v = "after" + verb;
        if (info[v] != undefined) {
            info[v](context);
        }

    }

    return result;
};

// Constructs a "mood sentences" object.
THESEUS.MoodSentences = function(info) {
    var sentences = info.sentences;
    var random = info.random === undefined ? false : info.random;
    var probability = info.probability === undefined ? 100 : info.probability;
    var availableSentences = [];

    return {
        get: function () {
            if (availableSentences.length == 0) {
                availableSentences = sentences.slice();
            }
            if (probability == 100 || Math.random() * 100 < probability) {
                var index = 0;
                if (random) {
                    index = Math.floor(Math.random() * availableSentences.length);
                }
                var sentence = availableSentences[index].slice(0);
                availableSentences.splice(index, 1);
                return "<br><br>" + sentence;
            }
            return "";
        }
    }
};

// Constructs a location.
THESEUS.Location = function(info) {
    var containedItems = new THESEUS.Items();
    var characters = new THESEUS.List();

    if (info.containedItems != undefined){
        for (let i = 0; i < info.containedItems.length; i++) {
            containedItems.add(info.containedItems[i]);
        }
    }

    if (info.characters != undefined){
        for (let i = 0; i < info.characters.length; i++) {
            characters.add(info.characters[i]);
        }
    }

    var result = {
    	caption: info.caption,
        items: containedItems,
        characters: characters,
        getExits: getExits,
        look: look,
    }

    function getExits() {
        var exits = { };
        if (info.getExits != undefined) {
            info.getExits(exits);
        }
        return exits;
    }

    function look(context) {
        var s;
        if (info.look === undefined) {
	    	s = "You see nothing special here";
        }
        else{
            s = info.look(context);
            if (info.moodSentences != undefined) {
                s += info.moodSentences.get();
            }
        }
	    return s;
	}

    return result;
}

// Constructs a character object.
THESEUS.Character = function(info) {
    var caption = info.caption;
    var isVisible = withDefaultValue(info.isVisible, true);
    var location = withDefaultValue(info.location, null);
    var conversation = withDefaultValue(info.conversation, null);

    var character = {
        caption: caption,
        getVerbs: getVerbs,
        update: update,
        examine: info.examine,
        isVisible: () => isVisible,
        setVisible: value => {
            isVisible = value;
        },
    }

    return character;

    function getVerbs(context) {
        var verbs = new THESEUS.Collection();
        verbs.add("Examine", info.examine);
        verbs.add("Talk", conversation);
        return verbs;
    }

    function withDefaultValue(prop, def){
        return prop === undefined ? def : prop;
    }

    function update(context) {
        if (isVisible) {
            if (location != null) {
                location.characters.remove(character)
            }
            location = context.historicLocation(1);
            if (location != null) {
                location.characters.add(character);
            }
        }
    }
}

// Constructs a conversation object.
THESEUS.Conversation = function() {
	var _statements = {};
	var _responses = {};
	var _showConversation = null;
	var _addStatementToView = null;
	var _addResponseToView = null;
	var _clearView = null;
	var _hideConversation = null;

	return {
		initialize : initialize,
		addStatement : addStatement,
		setResponses : setResponses,
		startConversation : startConversation,
		respond : respond,
        end : end,
        clear : clear,
	}	

	function initialize(showConversation, addStatement, addResponse, clear, hideConversation) {
		_showConversation = showConversation;
		_addStatementToView = addStatement;
		_addResponseToView = addResponse;
		_clearView = clear;
		_hideConversation = hideConversation;
	}

	function addStatement(id, func) {
		_statements[id] = func;
	}

	function setResponses(id, ids) {
		_responses[id] = ids;
	}

	function startConversation(id) {
		_showConversation();
		respond(0, id);
	}

	function respond(responseId, newStatementId) {
		if (newStatementId == 0) {
		    THESEUS.conversation.end();
            if (THESEUS.view != undefined) { THESEUS.view.update(THESEUS.context); }
			return;
		}

		_clearView();
		var func = _statements[newStatementId];
 		_addStatementToView(func());
 		_responses[newStatementId].forEach(
  			r_id => {
  				var func = _statements[r_id];
                var id = _responses[r_id][0];
                _addResponseToView(func(), r_id, id);
  			});
	}

	function end() {
		_hideConversation();
	}

    function clear() {
        _statements = {};
        _responses = {};
    }
};

// Constructs an object that holds the game context, 
// i.e. the current state of the game. 
THESEUS.Context = function () {
    var location;
    var historicLocations;
    var inventory;
    var historicInventory;
    var message;
    var flags;
    var historicFlags;
    var characters;

    function initialize(loc, msg) {
        location = loc,
        inventory = new THESEUS.Items();
        historicInventory = THESEUS.Items(); // Everything that has ever been carried
        flags = new Set();
        message = msg;
        historicLocations = [];
        characters = [];
    }

    var obj = {
        initialize: initialize,
        location: () => location,
        setLocation: loc => {
            location = loc;
            message = "You move to the " + loc.caption;
        },
        updateLocation: () => historicLocations.push(location),
        updateCharacters: () => {
            for (let c in characters)
            {
                characters[c].update(obj);
            }
        },
        characters: () => characters,
        inventory: () => inventory,
        historicInventory: () => historicInventory,
        historicLocation: stepsBack => historicLocations[historicLocations.length - 1 - stepsBack],
        message: () => message,
        setMessage: m => message = m,
        flags: () => flags,
        historicFlags: () => historicFlags,
    }

    return obj;
};

// Constructs a state object.
THESEUS.State = function() {
    // Possible actions:
    //   N, E, S, W - move in the correct direction
    //   "office door"-"Unlock" - apply verb to noun
    //   K1, K5 - click the appropriate keylock digit
    //   R2,3 - respond 2, leading to statement 3
    var actions = [];
    var applying = false;

    var state = {
        add: add,
        clear: clear,
        apply: apply,
        toString: toString,
        fromString: fromString,
    }

    return state;

    function add(s) {
        if (!applying) {
            actions.push(s);
        }
	}

    function clear() {
		actions = [];
	}

    function apply() {
        applying = true;
        for (var i = 0; i < actions.length; i++) {
            var action = actions[i];
        
            // Is it a noun - verb construct?
            var nv = action.match(new RegExp(/^"([^"]*)"-"([^"]*)"$/));
            if (nv != null) {
                console.log("applying " + nv[2] + " to " + nv[1]);
                applyVerb(nv[1], nv[2]);
            }
            
            // Is it a direction?
            if (action == "N" || action == "E" || action == "S" || action == "W") {
                console.log("moving " + action);
                THESEUS.context.setLocation(THESEUS.context.location().getExits()[action]);
            }

            // Is it a keylock click?
            var kc = action.match(new RegExp(/^K\d$/));
            if (kc != null) {
                console.log("clicking digit " + kc[0][1]);
                THESEUS.keypad.enterDigit(kc[0][1]);
            }

            // Is it a response?
            var r = action.match(new RegExp(/^R(\d*),(\d*)$/));
            if (r != null) {
                console.log("responding " + r[1] + ", leading to statement " + r[2]);
                THESEUS.conversation.respond(r[1], r[2])
            }

            if (THESEUS.view != undefined) { THESEUS.view.update(THESEUS.context); }
        }
        applying = false;
    }

    function toString() {
        // TODO
    }

    function fromString() {
        // TODO
    }

    function applyVerb(itemName, verb) {
        var target = THESEUS.context.location().items.byName(itemName);
        if (target == null) {
            target = THESEUS.context.inventory().byName(itemName);
        }
        if (target == null) {
            target = THESEUS.context.location().characters.by("caption", itemName);
        }
        if (target == null) {
            return;
        }
        target.getVerbs(THESEUS.context).forEach((v, f) => {
            if (v == verb) {
                f(THESEUS.context);
            }
        });
    }
}

THESEUS.context = new THESEUS.Context();
THESEUS.conversation = new THESEUS.Conversation();
