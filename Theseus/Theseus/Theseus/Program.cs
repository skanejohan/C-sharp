using System.Diagnostics;
using System.IO;

namespace Theseus
{
    class Program
    {
        private static string SourceDir = @"D:\Github\C-sharp\Theseus\Theseus\TheseusTest\TestData";
        private static string TargetDir = @"D:\Temp\Adv";
        private static string ResourcesDir;

        static void Main(string[] args)
        {
            ResourcesDir = Path.Combine(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), "Resources");
            var compiler = new Compiler(Directory.GetFiles(SourceDir, "*.txt"));
            compiler.Output(ResourcesDir, TargetDir, "parswick", "travelSection");
        }
    }
}
