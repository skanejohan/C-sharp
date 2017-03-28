using System;
using System.IO;
using System.Windows.Forms;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System.Collections.Generic;

namespace ads.chip8
{
    static class Program
    {
        private static bool loadAsm(string fileName, out byte[] bytes)
        {
            var loadRes = Assembler.fromFile(fileName);
            if (loadRes.Success)
            {
                bytes = loadRes.Bytes;
                return true;
            }
            Console.WriteLine(loadRes.Error);
            bytes = new byte[0];
            return false;
        }

        private static bool loadBytes(string fileName, out byte[] bytes)
        {
            try
            {
                bytes = System.IO.File.ReadAllBytes(fileName);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + " with message: " + e.Message);
                bytes = new byte[0];
                return false;

            }
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: Chip8Emulator <fileName> [/f=frequency]");
                Console.WriteLine("example: Chip8Emulator movingzeros.asm /f=1000");
                return;
            }

            string fileName = args[0];
            var keyMap = new Dictionary<int, Keys>
            {
                { 0, Keys.NumPad0 }, { 1, Keys.NumPad1 }, { 2, Keys.NumPad2 }, { 3, Keys.NumPad3 },
                { 4, Keys.NumPad4 }, { 5, Keys.NumPad5 }, { 6, Keys.NumPad6 }, { 7, Keys.NumPad7 },
                { 8, Keys.NumPad8 }, { 9, Keys.NumPad9 }, { 10, Keys.A }, { 11, Keys.B },
                { 12, Keys.C }, { 13, Keys.D }, { 14, Keys.E }, { 15, Keys.E }
            };

            int frequency = 1000;
            if (args.Length > 1)
            {
                if (!int.TryParse(args[1].Substring(args[1].IndexOf('=') + 1), out frequency))
                    frequency = 1000;
            }

            // Load the data
            byte[] memory;
            if (Path.GetExtension(fileName).ToLower() == ".c8")
            {
                if (!loadAsm(args[0], out memory))
                    return;
            }
            else if (!loadBytes(args[0], out memory))
                return;

            // Set up the emulator
            Chip8 chip8 = new Chip8();
            chip8.initialize();
            chip8.load(memory);

            // Set up the factory for the Direct2D window.
            var windowFactory = new ads.direct2d.Direct2dWindowFactory()
            {
                Caption = String.Format("Chip8 Emulator [{0} Hz]", frequency),
                WindowWidth = 640,
                WindowHeight = 320
            };

            long currentTicks;
            long previousTicks = DateTime.Now.Ticks;

            // Create the Direct2D window and run the application, repreatedly calling the supplied code.
            windowFactory.Run(
                (renderTarget, keys) =>
                {
                    foreach (var kv in keyMap)
                        chip8.Keys[kv.Key] = keys.Contains(kv.Value);

                    // Emulate the correct number of operations to match the desired clock frequency.
                    currentTicks = DateTime.Now.Ticks;
                    int noOfOperations = (int)((currentTicks - previousTicks) / 1e7 * frequency);
                    for (int i = 0; i < noOfOperations; i++)
                    {
                        chip8.emulateCycle();
                    }
                    previousTicks += (long)(noOfOperations * 1e7 / frequency);

                    // Render the screen
                    renderTarget.Clear(new RawColor4(0x00, 0x00, 0x00, 0xFF));
                    using (var brush = new SolidColorBrush(renderTarget, new RawColor4(0xFF, 0xFF, 0xFF, 0xFF)))
                    {
                        var w = renderTarget.Size.Width / 64;
                        var h = renderTarget.Size.Height / 32;
                        for (byte x = 0; x < 64; x++)
                            for (byte y = 0; y < 32; y++)
                                if (chip8.Display[y * 64 + x])
                                    renderTarget.FillRectangle(new RawRectangleF(x * w, y * h, (x + 1) * w, (y + 1) * h), brush);
                    }
                });
        }
    }
}
