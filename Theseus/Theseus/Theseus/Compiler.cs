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
            GameUtils.GameName = gameName;

            new DirectoryInfo(targetDir).CreateOrClear(targetDir);
            // TODO check no errors!
            File.Copy(Path.Combine(resourcesDir, "index.css"), Path.Combine(targetDir, $"{gameName}.css"));
            File.Copy(Path.Combine(resourcesDir, "theseus.framework.js"), Path.Combine(targetDir, $"{gameName}.framework.js"));

            // Make sure that we write stuff in the correct order
            var orderer = new Orderer();
            foreach (var i in Semantics.Items)
            {
                orderer.AddOrderable(i);
            }
            orderer.Order();

            // Create the JavaScript file
            var cb = new CodeBuilder(0, 4);
            cb.Add("\"use strict\";");
            cb.Add();

            cb.Add($"THESEUS.{gameName.ToUpper()} = {{}};");
            cb.Add();

            var flagsCounter = 1;
            cb.Add($"THESEUS.{gameName.ToUpper()}.Flag = {{").In();
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
                cb.Add(); 
            }

            foreach (var c in Semantics.Conversations)
            {
                c.EmitJavaScriptCode(Semantics, cb);
                cb.Add();
            }

            foreach (var c in Semantics.Characters)
            {
                c.EmitJavaScriptCode(Semantics, cb);
                cb.Add();
            }

            foreach (var d in Semantics.Doors)
            {
                d.EmitJavaScriptCode(Semantics, cb);
                cb.Add();
            }

            foreach (var l in Semantics.Locations)
            {
                l.EmitJavaScriptCode(Semantics, cb);
                cb.Add();
            }

            cb.Add($"THESEUS.{gameName.ToUpper()}.startGame = function(){{").In();
            cb.Add($"THESEUS.context.initialize(THESEUS.{gameName.ToUpper()}.{startLocation}, \"Welcome to the game\");");
            foreach (var c in Semantics.Characters)
            {
                cb.Add($"THESEUS.context.characters().push(THESEUS.{gameName.ToUpper()}.{c.Name});");
            }
            cb.Out();
            cb.Add("}");

            File.WriteAllText(Path.Combine(targetDir, $"{gameName}.js"), cb.ToString());

            // Create index.html
            var indexHtml = File.ReadAllText(Path.Combine(resourcesDir, "index.html"))
                .Replace("_gamename_", gameName)
                .Replace("_GAMENAME_", gameName.ToUpper());
            File.WriteAllText(Path.Combine(targetDir, $"{gameName}.html"), indexHtml);

            //TODO ???
            File.Copy(Path.Combine(resourcesDir, "parswick.test.js"), Path.Combine(targetDir, "parswick.test.js"));
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
