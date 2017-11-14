var context = (function () {
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
        inventory = new items();
        historicInventory = new items(); // Everything that has ever been carried
        flags = new Set();
        message = msg;
        historicLocations = [];
        characters = [];
    }

    return {
        initialize: initialize,
        location: () => location,
        setLocation: loc => {
            location = loc;
            message = "You move to the " + loc.caption;
        },
        updateLocation: () => historicLocations.push(location),
        updateCharacters: () => {
            for (c in characters)
            {
                characters[c].update();
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
})();
