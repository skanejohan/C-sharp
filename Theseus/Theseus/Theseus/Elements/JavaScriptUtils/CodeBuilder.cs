using System;
using System.Collections.Generic;
using Theseus.Extensions;
using Theseus.Interfaces;

namespace Theseus.Elements.JavaScriptUtils
{
    public class CodeBuilder : ICodeBuilder
    {
        private int indent;
        private int indentSize;
        private List<string> lines;

        public CodeBuilder(int indent, int indentSize)
        {
            lines = new List<string>();
            this.indentSize = indentSize;
            this.indent = indent;
        }

        public void In()
        {
            indent += indentSize;
        }

        public void Out()
        {
            indent -= indentSize;
        }

        public ICodeBuilder Add()
        {
            lines.Add("");
            return this;
        }

        public ICodeBuilder Add(string s)
        {
            lines.Add(s.Indent(indent));
            return this;
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, lines);
        }

        public IEnumerable<string> Lines()
        {
            foreach (var line in lines)
            {
                yield return line;
            }
        }
    }

}
