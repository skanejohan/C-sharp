using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Theseus
{
    class Program
    {
        private static List<string> files = new List<string>();
        private static string OutDir = "";
        private static string StartPosition = "";
        private static string GameName = "";

        private static bool GetValue(IEnumerable<string> lines, string key, string description, out string value)
        {
            var line = lines.FirstOrDefault(s => s.ToLower().StartsWith($"{key}="));
            if (line == null)
            {
                Console.WriteLine($"Theseus file doesn't contain a line defining {description} (starting with \"{key}=\").");
                value = "";
                return false;
            }
            value = line.Split('=')[1];
            return true;
        }

        static bool ParseTheseusFile(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"Theseus file {file} not found.");
                return false;
            }

            var lines = File.ReadAllLines(file);
            string sourceDirs;

            if (!GetValue(lines, "source", "source directories", out sourceDirs)) return false;
            if (!GetValue(lines, "out", "out directory", out OutDir)) return false;
            if (!GetValue(lines, "startposition", "start position", out StartPosition)) return false;
            if (!GetValue(lines, "gamename", "game name", out GameName)) return false;

            try
            {
                foreach (var dir in sourceDirs.Split(','))
                {
                    foreach (var f in Directory.GetFiles(dir, "*.tsu"))
                    {
                        files.Add(f);
                    }
                }
                return true;
            }
            catch
            {
                Console.WriteLine($"Unable to parse {file}");
                return false;
            }
        }

        static void Main(string[] args)
        {
            var ResourcesDir = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Resources");
            if (ParseTheseusFile(args[0]))
            {
                new Compiler(files).Output(ResourcesDir, OutDir, GameName, StartPosition);
            }
            if (Debugger.IsAttached)
            {
                Console.ReadLine();
            }
        }
    }
}
