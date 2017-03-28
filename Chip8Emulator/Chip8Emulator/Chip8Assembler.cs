using System;
using System.Collections.Generic;
using System.IO;

namespace ads.chip8
{
    /// <summary>
    /// Call fromFile to read a .c8, parse it and generate a byte array containing the opCodes.
    /// Has support for labels, commands and comments.
    /// 
    /// The following commands are supported:
    /// CLS                ; Clear the screen.
    /// RTS                ; Return from a subroutine.
    /// JMP NNN            ; Jump to address NNN.
    /// JSR NNN            ; Call subroutine at NNN.
    /// SKIPEQABS X NN     ; Skip the next instruction if VX equals NN.
    /// SKIPNEQABS X NN    ; Skip the next instruction if VX doesn't equal NN.
    /// SKIPEQ X Y         ; Skip the next instruction if VX equals VY.
    /// SETABS X NN        ; Set VX to NN.
    /// ADDABS X NN        ; Add NN to VX.
    /// SET X Y            ; Set VX to the value of VY.
    /// OR X Y             ; Set VX to VX or VY.
    /// AND X Y            ; Set VX to VX and VY.
    /// XOR X Y            ; Set VX to VX xor VY.
    /// ADD X Y            ; Add VY to VX.
    /// SUB X Y            ; VY is subtracted from VX.
    /// SHR X              ; Shift VX right by one.
    /// SUBREV X Y         ; Set VX to VY minus VX.
    /// SHL X              ; Shift VX left by one.
    /// SKIPNEQ X Y        ; Skip the next instruction if VX doesn't equal VY.
    /// SETI NNN           ; Set I to the address NNN.
    /// JMPREL NNN         ; Jump to the address NNN plus V0.
    /// RAND X NN          ; Set VX to the result of a bitwise and operation on a random number and NN.
    /// DRAW X Y N         ; Draw a sprite at coordinate (VX, VY) that has a width of 8 pixels and a height of N pixels.
    /// SKIPKEY X          ; Skip the next instruction if the key stored in VX is pressed.
    /// SKIPNKEY X         ; Skip the next instruction if the key stored in VX isn't pressed.
    /// GETDELAY X         ; Set VX to the value of the delay timer.
    /// WAITKEY X          ; A key press is awaited, and then stored in VX.
    /// SETDELAY X         ; Sets the delay timer to VX.
    /// SETSOUND X         ; Sets the sound timer to VX.
    /// ADDI X             ; Add VX to I
    /// SPRLOC X           ; Set I to the location of the sprite for the character in VX.
    /// BCD X              ; Stores the binary-coded decimal representation of VX at I, I+1, I+2
    /// WREGS X            ; Store V0 to VX (including VX) in memory starting at address I.
    /// RREGS X            ; Fill V0 to VX (including VX) with values from memory starting at address I.
    /// 
    /// Example code:
    /// 
    ///                 SETABS 0 00     ; V[0] = 0 indicates movement to the right
    ///                 SETABS 1 00     ; V[1] = 0 (x coordinate of sprite)
    ///                 SETABS 2 00     ; V[2] = 0 (y coordinate of sprite)
    ///                 SETI 000        ; I = 0 (mem of sprite to draw - "0")
    ///                 SETABS 3 01     ; V[3] = 1 (used for subtraction later)
    /// DRAW:           CLS
    ///                 DRAW 1 2 5      ; Draw sprite(5 rows high, given x, y and i as above)
    /// 
    ///                 ; Main loop starts here
    /// MAINLOOP:       SKIPEQABS 0 00  ; Skip the next instruction if V[0] == 0 (indicates we are moving right)
    ///                 JMP MOVINGLEFT
    /// 
    ///                 ; We are currently moving right
    ///                 SKIPNEQABS 1 3C ; Skip the next instruction if V[1] <> 60 (we have not reached the right edge)
    ///                 JMP SWITCHLEFT
    ///                 ADDABS 1 01     ; Increase x coordinate by 1
    ///                 JMP DRAW
    /// SWITCHLEFT:     SETABS 0 01 ; Set V[0] to 1 (indicates movement to the left)
    /// 
    ///                 ; We are currently moving left
    /// MOVINGLEFT:     SKIPNEQABS 1 00 ; Skip the next instruction if V[1] <> 0 (we have not reached the left edge)
    ///                 JMP SWITCHRIGHT
    ///                 SUB 1 3         ; Decrease x coordinate by 1 (subtract V[3] from V[1])
    ///                 JMP DRAW
    /// SWITCHRIGHT:    SETABS 0 00     ; Set V[0] to 0 (indicates movement to the right)
    ///     JMP MAINLOOP
    /// </summary>
    static class Assembler
    {
        public class Result
        {
            public bool Success { get; set; }
            public string Error { get; set; }
            public byte[] Bytes { get; set; }
        }

        public static Result fromFile(string fileName)
        {
            ushort commandIndex = 0x0200;
            var opCodes = new List<byte>();
            var commands = new List<string>();
            var labels = new Dictionary<string,ushort>();

            Result result = new Result() { Success = true };

            // Read the file and build up a list of commands (strings) and a dictionary of label positions
            foreach (var line in File.ReadLines(fileName))
            {
                var s = line;

                if (s.IndexOf(";") > 0)
                {
                    s = s.Substring(0, s.IndexOf(";"));
                }

                if (s.Trim() != "")
                {
                    if (s.IndexOf(":") > 0)
                    {
                        labels.Add(s.Substring(0, s.IndexOf(":")), commandIndex);
                        s = s.Substring(s.IndexOf(":") + 1);
                    }
                    commands.Add(s.Trim());
                    commandIndex += 2;
                }
            }

            // Using the commands (strings) and label positions, create the list of opCodes
            foreach (var command in commands)
            {
                string[] words = command.Split(' ');
                if (words[0] == "CLS")
                {
                    opCodes.Add(0x00);
                    opCodes.Add(0xE0);
                }
                else if (words[0] == "RTS")
                {
                    opCodes.Add(0x00);
                    opCodes.Add(0xEE);
                }
                else if (words[0] == "JMP")
                {
                    ushort val;
                    if (labels.TryGetValue(words[1], out val))
                    {
                        byte hi = (byte)(0x10 + ((val & 0x0F00) >> 8));
                        byte lo = (byte)(val & 0x00FF);
                        opCodes.Add(hi);
                        opCodes.Add(lo);
                    }
                    else
                    {
                        result.Success = false;
                        result.Error = string.Format("Invalid label in command \"{0}\"", command);
                    }
                }
                else if (words[0] == "JSR")
                {
                    ushort val;
                    if (labels.TryGetValue(words[1], out val))
                    {
                        byte hi = (byte)(0x20 + (val & 0x0F00) >> 8);
                        byte lo = (byte)(val & 0x00FF);
                        opCodes.Add(hi);
                        opCodes.Add(lo);
                    }
                    else
                    {
                        result.Success = false;
                        result.Error = string.Format("Invalid label in command \"{0}\"", command);
                    }
                }
                else if (words[0] == "SKIPEQABS")
                {
                    opCodes.Add((byte)(0x30 + byte.Parse(words[1])));
                    opCodes.Add(Convert.ToByte(words[2], 16));
                }
                else if (words[0] == "SKIPNEQABS")
                {
                    opCodes.Add((byte)(0x40 + byte.Parse(words[1])));
                    opCodes.Add(Convert.ToByte(words[2], 16));
                }
                else if (words[0] == "SKIPEQ")
                {
                    opCodes.Add((byte)(0x50 + byte.Parse(words[1])));
                    opCodes.Add((byte)(byte.Parse(words[2]) << 4));
                }
                else if (words[0] == "SETABS")
                {
                    opCodes.Add((byte)(0x60 + byte.Parse(words[1])));
                    opCodes.Add(byte.Parse(words[2]));
                }
                else if (words[0] == "ADDABS")
                {
                    opCodes.Add((byte)(0x70 + byte.Parse(words[1])));
                    opCodes.Add(byte.Parse(words[2]));
                }
                else if (words[0] == "SET")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add((byte)(byte.Parse(words[2]) << 4));
                }
                else if (words[0] == "OR")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add((byte)((byte.Parse(words[2]) << 4) + 1));
                }
                else if (words[0] == "AND")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add((byte)((byte.Parse(words[2]) << 4) + 2));
                }
                else if (words[0] == "XOR")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add((byte)((byte.Parse(words[2]) << 4) + 3));
                }
                else if (words[0] == "ADD")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add((byte)((byte.Parse(words[2]) << 4) + 4));
                }
                else if (words[0] == "SUB")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add((byte)((byte.Parse(words[2]) << 4) + 5));
                }
                else if (words[0] == "SHR")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add(0x06);
                }
                else if (words[0] == "SUBREV")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add((byte)((byte.Parse(words[2]) << 4) + 7));
                }
                else if (words[0] == "SHL")
                {
                    opCodes.Add((byte)(0x80 + byte.Parse(words[1])));
                    opCodes.Add(0x0E);
                }
                else if (words[0] == "SKIPNEQ")
                {
                    opCodes.Add((byte)(0x90 + byte.Parse(words[1])));
                    opCodes.Add((byte)(byte.Parse(words[2]) << 4));
                }
                else if (words[0] == "SETI")
                {
                    var val = ushort.Parse(words[1]);
                    byte hi = (byte)((val & 0x0F00) >> 8);
                    byte lo = (byte)(val & 0x00FF);
                    opCodes.Add((byte)(0xA0 + hi));
                    opCodes.Add(lo);
                }
                else if (words[0] == "JMPREL")
                {
                    opCodes.Add((byte)(0xB0 + byte.Parse(words[1])));
                    opCodes.Add(byte.Parse(words[2]));
                }
                else if (words[0] == "RAND")
                {
                    opCodes.Add((byte)(0xC0 + byte.Parse(words[1])));
                    opCodes.Add(byte.Parse(words[2]));
                }
                else if (words[0] == "DRAW")
                {
                    opCodes.Add((byte)(0xD0 + byte.Parse(words[1])));
                    opCodes.Add((byte)((byte.Parse(words[2]) << 4) + byte.Parse(words[3])));
                }
                else if (words[0] == "SKIPKEY")
                {
                    opCodes.Add((byte)(0xE0 + byte.Parse(words[1])));
                    opCodes.Add(0x9E);
                }
                else if (words[0] == "SKIPNKEY")
                {
                    opCodes.Add((byte)(0xE0 + byte.Parse(words[1])));
                    opCodes.Add(0xA1);
                }
                else if (words[0] == "GETDELAY")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x07);
                }
                else if (words[0] == "WAITKEY")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x0A);
                }
                else if (words[0] == "SETDELAY")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x15);
                }
                else if (words[0] == "SETSOUND")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x18);
                }
                else if (words[0] == "ADDI")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x1E);
                }
                else if (words[0] == "SPRLOC")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x29);
                }
                else if (words[0] == "BCD")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x33);
                }
                else if (words[0] == "WREGS")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x55);
                }
                else if (words[0] == "RREGS")
                {
                    opCodes.Add((byte)(0xF0 + byte.Parse(words[1])));
                    opCodes.Add(0x65);
                }
                else
                {
                    result.Success = false;
                    result.Error = string.Format("Unknown command in \"{0}\"", command);
                }
            }

            foreach (var opCode in opCodes)
            {
                Console.WriteLine(String.Format("{0:X2}", opCode));
            }

            result.Bytes = opCodes.ToArray();
            
            return result;
        }
    }
}
