var items = function () {
    var _items = []

    var _add = function(item){
        if (!_has(item)) { 
            _items.push(item) 
        }
    }

    var _has = function (item) {
        return _items.indexOf(item) > -1;
    }

    var _byName = function (name) {
        var _item = null;
        _forEach(
            (i, c) => {
                if (i.caption == name) {
                    _item = i;
                }
            }, i => true, i => true);
        return _item;
    }

    var _remove = function (item) {
        var idx = _items.indexOf(item);
        if (idx > -1) {
            _items.splice(idx, 1);
        }
        else {
            _forEach(
                i => {
                    if (i.hasOwnProperty('containedItems')) {
                        i.containedItems.remove(item);
                    }
                });
        }
    }

    var _forEach = function (fn) {
        for (var i = 0; i < _items.length; i++) {
            fn(_items[i]);
            if (_items[i].hasOwnProperty('containedItems')) {
                _items[i].containedItems.forEachOpen(fn, _items[i]);
            }
        }
    }

    var _forEachOpen = function (fn, container) {
        if (container == undefined) {
            container = null;
        }
        for (var i = 0; i < _items.length; i++) {
            if (_items[i].isVisible()) {
                fn(_items[i], container);
                if (_items[i].hasOwnProperty('containedItems') &&
                    (!_items[i].hasOwnProperty('isOpen') || _items[i].isOpen())) {
                    _items[i].containedItems.forEachOpen(fn, _items[i]);
                }
            }
        }
    }

    return {
        add : _add,
        has: _has,
        byName: _byName,
        remove: _remove,
        length: () => _items.length,
        forEach: _forEach,
        forEachOpen: _forEachOpen,
    }
};
