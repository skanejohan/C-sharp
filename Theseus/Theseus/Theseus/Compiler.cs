using Sprache;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Theseus.Elements.JavaScriptUtils;
using Theseus.Extensions;
using Theseus.Interfaces;
using Theseus.Parser;
using Theseus.Semantics;

namespace Theseus
{
    public class Compiler
    {
        private class Document
        {
            public string SourceFile { get; }
            public IElement Element { get; }
            public Document(string sourceFile, IElement element)
            {
                SourceFile = sourceFile;
                Element = element;
            }
        }

        public Compiler(IEnumerable<string> sourceFiles)
        {
            Documents = new List<Document>();
            Semantics = new SemanticsManager();

            foreach (var sourceFile in sourceFiles)
            {
                Parse(sourceFile);
            }

            foreach (var i in Semantics.Items)
            {
                i.CheckSemantics(Semantics);
            }
        }

        public void Output(string resourcesDir, string targetDir, string gameName, string startLocation)
        {
            var gameDir = Path.Combine(targetDir, "Game");
            var frameworkDir = Path.Combine(targetDir, "Framework");
            var frameworkSourceDir = Path.Combine(resourcesDir, "Framework");

            new DirectoryInfo(targetDir).CreateOrClear(targetDir);
            Directory.CreateDirectory(frameworkDir);
            Directory.CreateDirectory(gameDir);

            File.Copy(Path.Combine(resourcesDir, "index.css"), Path.Combine(targetDir, "index.css"));
            File.Copy(Path.Combine(frameworkSourceDir, "conv.js"), Path.Combine(frameworkDir, "conv.js"));
            File.Copy(Path.Combine(frameworkSourceDir, "game.js"), Path.Combine(frameworkDir, "game.js"));
            File.Copy(Path.Combine(frameworkSourceDir, "keypad.js"), Path.Combine(frameworkDir, "keypad.js"));
            File.Copy(Path.Combine(frameworkSourceDir, "state.js"), Path.Combine(frameworkDir, "state.js"));
            File.Copy(Path.Combine(frameworkSourceDir, "view.js"), Path.Combine(frameworkDir, "view.js"));
            File.Copy(Path.Combine(frameworkSourceDir, "collection.js"), Path.Combine(frameworkDir, "collection.js"));
            File.Copy(Path.Combine(frameworkSourceDir, "items.js"), Path.Combine(frameworkDir, "items.js"));

            // TODO check no errors!

            // Make sure that we write stuff in the correct order
            var orderer = new Orderer();
            foreach (var i in Semantics.Items)
            {
                orderer.AddOrderable(i);
            }
            orderer.Order();

            // Create the JavaScript file
            var cb = new CodeBuilder(0, 4);

            var flagsCounter = 1;
            cb.Add("var Flag = {").In();
            foreach (var flag in Semantics.Flags)
            {
                cb.Add($"{flag.Name} : {flagsCounter},");
                flagsCounter++;
            }
            cb.Out();
            cb.Add("}");
            cb.Add();

            foreach (var i in Semantics.Items.ToList().OrderBy(i => i.Order).Reverse())
            {
                i.EmitJavaScriptCode(Semantics, cb);
            }

            foreach (var c in Semantics.Characters)
            {
                c.EmitJavaScriptCode(Semantics, cb);
            }

            foreach (var d in Semantics.Doors)
            {
                d.EmitJavaScriptCode(Semantics, cb);
            }

            foreach (var l in Semantics.Locations)
            {
                l.EmitJavaScriptCode(Semantics, cb);
            }

            File.WriteAllText($"{gameDir}\\{gameName}.js", cb.ToString());

            // Create index.html
            var indexHtml = File.ReadAllText(Path.Combine(resourcesDir, "index.html"))
                .Replace("{startposition}", "travelSection")
                .Replace("{gameName}", gameName);
            File.WriteAllText(Path.Combine(targetDir, "index.html"), indexHtml);

            //TODO ???
            File.Copy(Path.Combine(frameworkSourceDir, "test.js"), Path.Combine(frameworkDir, "test.js"));
        }

        private List<Document> Documents;
        private SemanticsManager Semantics;

        private void Parse(string sourceFile)
        {
            var code = File.ReadAllText(sourceFile);
            var element = TheseusParser.DocumentParser.Parse(code);
            (element as ISemanticsValidator)?.BuildSemantics(Semantics);
            Documents.Add(new Document(sourceFile, element));
        }
    }
}
