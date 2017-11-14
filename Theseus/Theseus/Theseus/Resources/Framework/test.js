function applyVerb(context, itemName, verb) {
    var target = context.location().items.byName(itemName);
    if (target == null) {
        target = context.inventory().byName(itemName);
    }
    if (target == null) {
        target = context.location().characters.byName(itemName);
    }
    if (target == null) {
        return;
    }
    target.getVerbs(context).forEach((v, f) => {
        if (v == verb) {
            f(context);
        }
    }), i=> true, i => true;
}

function applyMovement(context, direction) {
    context.setLocation(context.location().getExits()[direction]);
}

function doEval(s) {
    eval(s);
    view.update(context);
}

function test() {
    //M:E,M:S,V:"cupboard"-"Open",V:"cup"-"Take",V:"office door key"-"Take",M:N,M:E,M:S,V:"office door"-"Unlock",
    //V:"office door"-"Open",M:S,V:"safe"-"Enter combination",K:1,K:9,K:7,K:9,

    doEval('applyMovement(context, "E")');
    doEval('applyMovement(context, "S")');
    doEval('applyVerb(context, "cupboard", "Open")');
    doEval('applyVerb(context, "cup", "Take")');
    doEval('applyVerb(context, "office door key", "Take")');
    doEval('applyMovement(context, "N")');
    doEval('applyMovement(context, "E")');
    doEval('applyMovement(context, "S")');
    doEval('applyVerb(context, "office door", "Unlock")');
    doEval('applyVerb(context, "office door", "Open")');
    doEval('applyMovement(context, "S")');
    doEval('applyVerb(context, "safe", "Enter combination")');
    doEval('keypad.enterDigit(1)');
    doEval('keypad.enterDigit(9)');
    doEval('keypad.enterDigit(7)');
    doEval('keypad.enterDigit(9)');
    doEval('applyVerb(context, "safe", "Open")');
    doEval('applyVerb(context, "old book", "Take")');
    doEval('applyVerb(context, "desk", "Examine")');
    doEval('applyVerb(context, "drawer", "Open")');
    doEval('applyVerb(context, "paper clip", "Take")');
    doEval('applyVerb(context, "cabinet", "Open")');
    doEval('applyVerb(context, "metal box", "Pick")');
    doEval('applyVerb(context, "metal box", "Open")');
    doEval('applyVerb(context, "rock pick", "Take")');
    doEval('applyMovement(context, "N")');
    doEval('applyMovement(context, "N")');
    doEval('applyMovement(context, "W")');
    doEval('applyMovement(context, "W")');
    doEval('applyVerb(context, "language shelf", "Examine")');
    doEval('applyVerb(context, "latin dictionary", "Take")');
    doEval('applyVerb(context, "old book", "Examine")');
    doEval('applyMovement(context, "E")');
    doEval('applyMovement(context, "E")');
    doEval('applyMovement(context, "S")');
    doEval('applyVerb(context, "history bookshelf", "Examine")');
    doEval('applyVerb(context, "history bookshelf", "Empty")');
    doEval('applyVerb(context, "history bookshelf", "Pull")');
    doEval('applyVerb(context, "wall", "Examine")');
    doEval('applyVerb(context, "wall", "Hit")');
    doEval('applyMovement(context, "S")');
    doEval('applyVerb(context, "sink", "Examine")');
    doEval('applyVerb(context, "Uncle Ailbert", "Talk")');
    doEval('conversation.respond(2, 3)');
    doEval('conversation.respond(2, 3)');
    doEval('conversation.respond(5, 9)');
    doEval('conversation.respond(11, 16)');
    doEval('conversation.respond(17, 0)');
    doEval('applyVerb(context, "water cooker", "Make tea")');
    doEval('applyMovement(context, "N")');
    doEval('applyMovement(context, "E")');
    doEval('applyMovement(context, "S")');
    doEval('applyVerb(context, "wall", "Hit")');
}
