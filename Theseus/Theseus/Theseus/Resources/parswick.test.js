function test() {
    var state = new THESEUS.State();
    state.add('E');
    state.add('S');
    state.add('"cupboard"-"Open"');
    state.add('"cup"-"Take"');
    state.add('"office door key"-"Take"');
    state.add('N');
    state.add('E');
    state.add('S');
    state.add('"office door"-"Unlock"');
    state.add('"office door"-"Open"');
    state.add('S');
    state.add('"safe"-"Enter combination"');
    state.add('K1');
    state.add('K9');
    state.add('K7');
    state.add('K9');
    state.add('"safe"-"Open"');
    state.add('"old book"-"Take"');
    state.add('"desk"-"Examine"');
    state.add('"drawer"-"Open"');
    state.add('"paper clip"-"Take"');
    state.add('"cabinet"-"Open"');
    state.add('"metal box"-"Pick"');
    state.add('"metal box"-"Open"');
    state.add('"rock pick"-"Take"');
    state.add('N');
    state.add('N');
    state.add('W');
    state.add('W');
    state.add('"language shelf"-"Examine"');
    state.add('"latin dictionary"-"Take"');
    state.add('"old book"-"Examine"');
    state.add('E');
    state.add('E');
    state.add('S');
    state.add('"history bookshelf"-"Examine"');
    state.add('"history bookshelf"-"Empty"');
    state.add('"history bookshelf"-"Pull"');
    state.add('"wall"-"Examine"');
    state.add('"wall"-"Hit"');
    state.add('S');
    state.add('"sink"-"Examine"');
    state.add('"Uncle Ailbert"-"Talk"');
    state.add('R2,3');
    state.add('R2,3');
    state.add('R5,9');
    state.add('R11,16');
    state.add('R17,0');
    state.add('"water cooker"-"Make tea"');
    state.add('N');
    state.add('E');
    state.add('S');
    state.add('"wall"-"Hit"');
    state.apply();
}