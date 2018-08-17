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
        context.state().add("A-" + info.name + "-drop");
        context.inventory().remove(result);
        context.location().items.add(result);
        context.setMessage("You drop the " + info.caption + ".");
        processAfter(context, "Drop");
        context.update();
    }

    function take(context) {
        context.state().add("A-" + info.name + "-take");
        context.inventory().remove(result);
        context.inventory().add(result);
        context.historicInventory().add(result);
        context.location().items.remove(result);
        context.setMessage("You take the " + info.caption + ".");
        processAfter(context, "Take");
        context.update();
    }

    function close(context) {
        context.state().add("A-" + info.name + "-close");
        isOpen = false;
        isClosed = true;
        hasBeenClosed = true;
        context.setMessage("You close the " + info.caption + ".");
        processAfter(context, "Close");
        context.update();
    }

    function open(context) {
        context.state().add("A-" + info.name + "-open");
        isOpen = true;
        isClosed = false;
        hasBeenOpen = true;
        context.setMessage("You open the " + info.caption + ".");
        processAfter(context, "Open");
        context.update();
    }

    function lock(context) {
        context.state().add("A-" + info.name + "-lock");
        isLocked = true;
        hasBeenLocked = true;
        context.setMessage("You lock the " + info.caption + ".");
        processAfter(context, "Lock");
        context.update();
    }

    function enterCombination(context, combination) {
        var correctFunction = function() {
            isLocked = false;
            hasBeenUnlocked = true;
            context.setMessage("You enter the correct combination and unlock the " + info.caption + ".");
            processAfter(context, "EnterCombination");
            context.update();
        } 

        var incorrectFunction = function() {
            context.setMessage("For a moment there, you thought you remembered the code. A futile attempt.");
            processAfter(context, "EnterCombination");
            context.update();
        }

        context.state().add("A-" + info.name + "-enterCombination-" + combination);
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
        context.state().add("A-" + info.name + "-unlock");
        context.setMessage("You unlock the " + info.caption + ".");
        isLocked = false;
        hasBeenUnlocked = true;
        processAfter(context, "Unlock");
        context.update();
    }

    function pick(context) {
        context.state().add("A-" + info.name + "-pick");
        isLocked = false;
        hasBeenUnlocked = true;
        isPicked = true;
        hasBeenPicked = true;
        context.setMessage("Using the " + info.requiredKey.caption + ", you manage to pick the " + info.caption + ".");
        processAfter(context, "Pick");
        context.update();
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
        name: info.name,
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
        if (info.talk != undefined) {
            verbs.add("Talk", info.talk);
        }
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

    var _currentStatementId;
    var currentStatement;
    var currentResponses;

	return {
		addStatement : addStatement,
		setResponses : setResponses,
		startConversation : startConversation,
        currentStatement : () => currentStatement,
        currentResponses : () => currentResponses,
	}	

	function addStatement(id, func) {
		_statements[id] = func;
	}

	function setResponses(id, ids) {
		_responses[id] = ids;
	}

    function startConversation(id) { 
        setStatementId(id);
    }

    function setStatementId(i) {
        if (i == 0) {
            currentStatement = undefined;
            return;
        }

        _currentStatementId = i;
		currentStatement = _statements[_currentStatementId]();
        
        currentResponses = [];
        _responses[_currentStatementId].forEach(
            r_id => {
                var func = _statements[r_id];
                var id = _responses[r_id][0];
                // todo fix this! We must not evaluate the function (and have the side effects) unless the item is actually selected
                currentResponses.push({text : func(), responseId : r_id, fn : () => setStatementId(id)});
            });
    }
};

// Constructs an object that holds the game context, 
// i.e. the current state of the game. 
THESEUS.Context = function () {
    var initialLocation;
    var initialMessage;
    var location;
    var historicLocations;
    var inventory;
    var historicInventory;
    var message;
    var flags;
    var historicFlags;
    var characters;
    var state;
    var conversation;

    function initialize(loc, msg) {
        initialLocation = loc;
        initialMessage = msg;
        location = loc;
        inventory = new THESEUS.Items();
        historicInventory = new THESEUS.Items(); // Everything that has ever been carried
        flags = new Set();
        message = msg;
        historicLocations = [];
        characters = [];
        state = new THESEUS.State();
        conversation = undefined;
    }

    function reset() {
        var chars = characters.slice(0);
        initialize(initialLocation, initialMessage);
        characters = chars;
    }

    function restore(stateString) {
        reset();
        state.fromString(stateString);
        state.apply();
    }

    function getObjectByName(name) {
        var obj = window;
        name.split(".").forEach((v,i) => obj = obj[v]);
        return obj;
    }

    var obj = {
        initialize: initialize,
        reset: reset,
        restore: restore,
        location: () => location,
        setLocation: loc => {
            if (typeof loc == "string") {
                location = getObjectByName(loc);
            } 
            else {
                location = loc;
            }
            state.add('M-' + location.name);
            message = "You move to the " + location.caption;
            obj.update();
        },
        update: () => {
            historicLocations.push(location);
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
        state: () => state,
        getObjectByName: getObjectByName,
        conversation: conversation,
    }

    return obj;
};

// Constructs a state object.
THESEUS.State = function() {
    // Possible actions:
    //   "M-THESEUS.PARSWICK.historySection" - move to the specified location
    //   "A-THESEUS.PARSWICK.cupboard-open" - apply verb to noun
    //   TODO: R2,3 - respond 2, leading to statement 3
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
        
        actions.forEach((s,i) => {
            var type = s.substring(0, 2);
            var info = s.substring(2);
            switch(type) {
                case "M-":
                    THESEUS.context.setLocation(info);
                    break;
                case "A-":
                    var noun = info.split("-")[0]; 
                    var verb = info.split("-")[1]; 
                    var data = info.split("-")[2];
                    var item = THESEUS.context.getObjectByName(noun);
                    item.getVerbs(THESEUS.context).forEach((v, f) => {
                        if (f.name == verb) {
                            f(THESEUS.context, data);
                        }
                    });
                    break;
            }
            // // TODO Is it a response?
            // var r = action.match(new RegExp(/^R(\d*),(\d*)$/));
            // if (r != null) {
            //     console.log("responding " + r[1] + ", leading to statement " + r[2]);
            //     THESEUS.conversation.respond(r[1], r[2])
            // }

            if (THESEUS.view != undefined) { THESEUS.view.update(THESEUS.context); }
        });
        applying = false;
    }

    function toString() {
        return actions.join(",");
    }

    function fromString(s) {
        actions = s.split(",");
    }
}

THESEUS.context = new THESEUS.Context();
