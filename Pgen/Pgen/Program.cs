using System;
using DerekSlager.IO;
using System.Windows.Forms;

namespace Pgen
{
    class Program
    {
        [STAThreadAttribute]
        static void Main(string[] args) 
        {
            if (MaskedConsoleReader.ReadLine() == "Ett&2tre")
            {
                string s = PgenImpl.convert(Console.ReadLine());
                Console.WriteLine(s);
                Clipboard.SetText(s);
            }
            else
            {
                Console.ReadLine();
            }
        }
    }
}
