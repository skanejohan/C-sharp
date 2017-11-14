var take = () => "You pick it up";
var drop = () => "You drop it";
var lookat = () => "A shiny object";
var lookatanother = () => "Another shiny object";
var lock = () => "You lock it";
var unlock = () => "You unlock it";

function testCollection(){
	beginTest("Collection");

	var coll = new collection();

	assert(!coll.has("Take"));
	assert(!coll.has("Drop"));
	assert(!coll.has("Look at"));

	coll.add("Take", take);
	assert(coll.has("Take"));
	assert(coll.value("Take") === take);
	assert(coll.value("Take")() == "You pick it up");
	assert(!coll.has("Drop"));
	assert(!coll.has("Look at"));

	coll.add("Drop", drop);
	assert(coll.has("Take"));
	assert(coll.has("Drop"));
	assert(coll.value("Drop") === drop);
	assert(coll.value("Drop")() == "You drop it");
	assert(!coll.has("Look at"));
	
	coll.add("Look at", lookat);
	assert(coll.has("Take"));
	assert(coll.has("Drop"));
	assert(coll.has("Look at"));
	assert(coll.value("Look at") === lookat);
	assert(coll.value("Look at")() == "A shiny object");

	coll.add("Look at", lookatanother);
	assert(coll.has("Take"));
	assert(coll.has("Drop"));
	assert(coll.has("Look at"));
	assert(coll.value("Look at") === lookatanother);
	assert(coll.value("Look at")() == "Another shiny object");

	coll.forEach(
		function(k, v) {
			if(k == "Take"){
				assert(v() == "You pick it up");
			}
			else if (k == "Drop"){
				assert(v() == "You drop it");
			}
			else if (k == "Look at"){
				assert(v() == "Another shiny object");
			}
		});

	coll.remove("Take");
	assert(!coll.has("Take"));
	assert(coll.has("Drop"));
	assert(coll.has("Look at"));

	coll.remove("Drop");
	assert(!coll.has("Take"));
	assert(!coll.has("Drop"));
	assert(coll.has("Look at"));

	coll.remove("Look at");
	assert(!coll.has("Take"));
	assert(!coll.has("Drop"));
	assert(!coll.has("Look at"));

	coll.add("Take", take);
	coll.add("Drop", drop);
	coll.add("Look at", lookat);

	var coll2 = new collection();
	coll2.add("Lock", lock);
	coll2.add("Unlock", unlock);

	coll.concat(coll2);
	assert(coll.value("Take")() == "You pick it up");
	assert(coll.value("Drop")() == "You drop it");
	assert(coll.value("Look at")() == "A shiny object");
	assert(coll.value("Lock")() == "You lock it");
	assert(coll.value("Unlock")() == "You unlock it");

	endTest();
}