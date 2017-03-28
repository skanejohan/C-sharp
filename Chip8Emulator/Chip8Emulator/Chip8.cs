using System;

namespace ads.chip8
{
    class Chip8
    {
        #region Private
        private const ushort DISPLAYSIZE = 64 * 32;
        private const ushort STACKSIZE = 16;
        private const ushort MEMORYSIZE = 4096;
        private const ushort KEYCOUNT = 16;
        private const ushort CHARCOUNT = 80;

        private readonly byte[] fontSet = new byte[CHARCOUNT]
        {
            0xF0, 0x90, 0x90, 0x90, 0xF0, //0
            0x20, 0x60, 0x20, 0x20, 0x70, //1
            0xF0, 0x10, 0xF0, 0x80, 0xF0, //2
            0xF0, 0x10, 0xF0, 0x10, 0xF0, //3
            0x90, 0x90, 0xF0, 0x10, 0x10, //4
            0xF0, 0x80, 0xF0, 0x10, 0xF0, //5
            0xF0, 0x80, 0xF0, 0x90, 0xF0, //6
            0xF0, 0x10, 0x20, 0x40, 0x40, //7
            0xF0, 0x90, 0xF0, 0x90, 0xF0, //8
            0xF0, 0x90, 0xF0, 0x10, 0xF0, //9
            0xF0, 0x90, 0xF0, 0x90, 0x90, //A
            0xE0, 0x90, 0xE0, 0x90, 0xE0, //B
            0xF0, 0x80, 0x80, 0x80, 0xF0, //C
            0xE0, 0x90, 0x90, 0x90, 0xE0, //D
            0xF0, 0x80, 0xF0, 0x80, 0xF0, //E
            0xF0, 0x80, 0xF0, 0x80, 0x80  //F
        };

        private ushort pc;     // The program counter
        private ushort opCode; // The current operation code
        private ushort iReg;   // The index register
        private ushort sp;     // The stack pointer
        private byte delayTimer;
        private byte soundTimer;

        private byte[] memory = new byte[MEMORYSIZE];  // Memory (size = 4k)		
        private bool[] display = new bool[DISPLAYSIZE]; // The display is 64*32 black/white pixels
        private byte[] vReg = new byte[16];			    // The data registers, V0-VF
        private ushort[] stack = new ushort[STACKSIZE]; // The stack has 16 levels
        private bool[] keys = new bool[KEYCOUNT];       // 16 keys, currently pressed or not
        private Random random = new Random();

        private enum Operation
        {
            op0NNN, op00E0, op00EE, op1NNN, op2NNN, op3XNN, op4XNN, op5XY0, op6XNN, op7XNN, op8XY0, op8XY1,
            op8XY2, op8XY3, op8XY4, op8XY5, op8XY6, op8XY7, op8XYE, op9XY0, opANNN, opBNNN, opCXNN, opDXYN,
            opEX9E, opEXA1, opFX07, opFX0A, opFX15, opFX18, opFX1E, opFX29, opFX33, opFX55, opFX65, opERROR
        };

        private string opCodeDescription;

        private Operation getOperation(ushort opCode)
        {
            if (opCode == 0x00E0)
                return Operation.op00E0;
            if (opCode == 0x00EE)
                return Operation.op00EE;
            if ((opCode & 0xF000) == 0x1000)
                return Operation.op1NNN;
            if ((opCode & 0xF000) == 0x2000)
                return Operation.op2NNN;
            if ((opCode & 0xF000) == 0x3000)
                return Operation.op3XNN;
            if ((opCode & 0xF000) == 0x4000)
                return Operation.op4XNN;
            if ((opCode & 0xF000) == 0x5000)
                return Operation.op5XY0;
            if ((opCode & 0xF000) == 0x6000)
                return Operation.op6XNN;
            if ((opCode & 0xF000) == 0x7000)
                return Operation.op7XNN;
            if ((opCode & 0xF00F) == 0x8000)
                return Operation.op8XY0;
            if ((opCode & 0xF00F) == 0x8001)
                return Operation.op8XY1;
            if ((opCode & 0xF00F) == 0x8002)
                return Operation.op8XY2;
            if ((opCode & 0xF00F) == 0x8003)
                return Operation.op8XY3;
            if ((opCode & 0xF00F) == 0x8004)
                return Operation.op8XY4;
            if ((opCode & 0xF00F) == 0x8005)
                return Operation.op8XY5;
            if ((opCode & 0xF00F) == 0x8006)
                return Operation.op8XY6;
            if ((opCode & 0xF00F) == 0x8007)
                return Operation.op8XY7;
            if ((opCode & 0xF00F) == 0x800E)
                return Operation.op8XYE;
            if ((opCode & 0xF000) == 0x9000)
                return Operation.op9XY0;
            if ((opCode & 0xF000) == 0xA000)
                return Operation.opANNN;
            if ((opCode & 0xF000) == 0xB000)
                return Operation.opBNNN;
            if ((opCode & 0xF000) == 0xC000)
                return Operation.opCXNN;
            if ((opCode & 0xF000) == 0xD000)
                return Operation.opDXYN;
            if ((opCode & 0xF0FF) == 0xE09E)
                return Operation.opEX9E;
            if ((opCode & 0xF0FF) == 0xE0A1)
                return Operation.opEXA1;
            if ((opCode & 0xF0FF) == 0xF007)
                return Operation.opFX07;
            if ((opCode & 0xF0FF) == 0xF00A)
                return Operation.opFX0A;
            if ((opCode & 0xF0FF) == 0xF015)
                return Operation.opFX15;
            if ((opCode & 0xF0FF) == 0xF018)
                return Operation.opFX18;
            if ((opCode & 0xF0FF) == 0xF01E)
                return Operation.opFX1E;
            if ((opCode & 0xF0FF) == 0xF029)
                return Operation.opFX29;
            if ((opCode & 0xF0FF) == 0xF033)
                return Operation.opFX33;
            if ((opCode & 0xF0FF) == 0xF055)
                return Operation.opFX55;
            if ((opCode & 0xF0FF) == 0xF065)
                return Operation.opFX65;
            return Operation.opERROR;
        }

        private bool processOpCode(ushort opCode)
        {
            // Parts of the opcode, used in several places below
            byte x = (byte)((opCode & 0x0F00) >> 8); // part of opCode: _X__
            byte y = (byte)((opCode & 0x00F0) >> 4); // part of opCode: __Y_
            byte n = (byte)(opCode & 0x000F);        // part of opCode: ___N
            byte nn = (byte)(opCode & 0x00FF);       // part of opCode: __NN
            ushort nnn = (ushort)(opCode & 0x0FFF);  // part of opCode: _NNN

            switch (getOperation(opCode))
            {

                case Operation.op00E0: // Clear the display
                    opCodeDescription = "clear the display";
                    for (int i = 0; i < DISPLAYSIZE; i++)
                    {
                        display[i] = false;
                    }
                    drawFlag = true;
                    pc += 2;
                    return true;

                case Operation.op00EE: // Return from subroutine - decrease sp, return to the address pointed to, increase pc.
                    opCodeDescription = "return from subroutine";
                    sp--;
                    pc = stack[sp];
                    pc += 2;
                    return true;

                case Operation.op1NNN: // Jump to address NNN
                    opCodeDescription = string.Format("jump to address {0:X4}", nnn);
                    pc = nnn;
                    return true;

                case Operation.op2NNN: // Call subroutine at NNN - store current address in stack, increment sp, set pc to the subroutine address.
                    opCodeDescription = string.Format("call subroutine at address {0:X4}", nnn);
                    stack[sp] = pc;
                    sp++;
                    pc = nnn;
                    return true;

                case Operation.op3XNN: // Skip the next instruction if vReg[X] equals NN
                    opCodeDescription = string.Format("skip next instruction if V[{0:X2}] == {1:X2}", x, nn);
                    if (vReg[x] == nn)
                        pc += 4;
                    else
                        pc += 2;
                    return true;

                case Operation.op4XNN: // Skip the next instruction if vReg[X] doesn't equal NN
                    opCodeDescription = string.Format("skip next instruction if V[{0:X2}] != {1:X2}", x, nn);
                    if (vReg[x] != nn)
                        pc += 4;
                    else
                        pc += 2;
                    return true;

                case Operation.op5XY0: // Skip the next instruction if vReg[X] equals vReg[Y]
                    opCodeDescription = string.Format("skip next instruction if V[{0:X2}] == V[{1:X2}]", x, y);
                    if (vReg[x] == vReg[y])
                        pc += 4;
                    else
                        pc += 2;
                    return true;

                case Operation.op6XNN: // Set vReg[X] to NN
                    opCodeDescription = string.Format("set V[{0:X2}] = {1:X2}", x, nn);
                    vReg[x] = nn;
                    pc += 2;
                    return true;

                case Operation.op7XNN: // Add NN to vReg[X]
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{0:X2}] + {1:X2}", x, nn);
                    vReg[x] += nn;
                    pc += 2;
                    return true;

                case Operation.op8XY0: // Sets vReg[X] to the value of vReg[Y]
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{1:X2}]", x, y);
                    vReg[x] = vReg[y];
                    pc += 2;
                    return true;

                case Operation.op8XY1: // Set vReg[X] to "vReg[X] OR vReg[Y]"
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{0:X2}] OR V[{1:X2}]", x, y);
                    vReg[x] = (byte)(vReg[x] | vReg[y]);
                    pc += 2;
                    return true;

                case Operation.op8XY2: // Set vReg[X] to "vReg[X] AND vReg[Y]"
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{0:X2}] AND V[{1:X2}]", x, y);
                    vReg[x] = (byte)(vReg[x] & vReg[y]);
                    pc += 2;
                    return true;

                case Operation.op8XY3: // Set vReg[X] to "vReg[X] XOR vReg[Y]"
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{0:X2}] XOR V[{1:X2}]", x, y);
                    vReg[x] = (byte)(vReg[x] ^ vReg[y]);
                    pc += 2;
                    return true;

                case Operation.op8XY4: // Add vReg[Y] to vReg[X]. vReg[F] is set to 1 when there's a carry, and to 0 when there isn't					
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{0:X2}] + V[{1:X2}], set V[0xF] = 1 if carry, 0 if not", x, y);
                    vReg[0xF] = (byte)((vReg[y] > (0xFF - vReg[x])) ? 1 : 0);
                    vReg[x] += vReg[y];
                    pc += 2;
                    return true;

                case Operation.op8XY5: // Subtract vReg[Y] from vReg[X]. vReg[F] is set to 0 when there's a borrow, and 1 when there isn't
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{0:X2}] - V[{1:X2}], set V[0xF] = 0 if borrow, 1 if not", x, y);
                    vReg[0xF] = (byte)(vReg[y] > vReg[x] ? 0 : 1);
                    vReg[x] -= vReg[y];
                    pc += 2;
                    return true;

                case Operation.op8XY6: // Shift vReg[X] right by one. vReg[F] is set to the value of the least significant bit of vReg[X] before the shift
                    opCodeDescription = string.Format("shift V[{0:X2}] right by one, set V[0xF] to LSB of V[{0:X2}] before operation", x);
                    vReg[0xF] = (byte)(vReg[x] & 0x01);
                    vReg[x] = (byte)(vReg[x] >> 1);
                    pc += 2;
                    return true;

                case Operation.op8XY7: // Set vReg[X] to vReg[Y] minus vReg[X]. vReg[F] is set to 0 when there's a borrow, and 1 when there isn't
                    opCodeDescription = string.Format("set V[{0:X2}] = V[{1:X2}] - V[{0:X2}], set V[0xF] = 0 if borrow, 1 if not", x, y);
                    vReg[0xF] = (byte)(vReg[x] > vReg[y] ? 0 : 1);
                    vReg[x] = (byte)(vReg[y] - vReg[x]);
                    pc += 2;
                    return true;

                case Operation.op8XYE: // Shift vReg[X] left by one. vReg[F] is set to the value of the most significant bit of vReg[X] before the shift
                    opCodeDescription = string.Format("shift V[{0:X2}] left by one, set V[0xF] to MSB of V[{0:X2}] before operation", x);
                    vReg[0xF] = (byte)(vReg[x] >> 7);
                    vReg[x] = (byte)(vReg[x] << 1);
                    pc += 2;
                    return true;

                case Operation.op9XY0: // Skip the next instruction if vReg[X] doesn't equal vReg[Y]
                    opCodeDescription = string.Format("skip next instruction if V[{0:X2}] != V[{1:X2}]", x, y);
                    pc += (ushort)((vReg[x] != vReg[y]) ? 4 : 2);
                    return true;

                case Operation.opANNN: // Set iReg to the address NNN
                    opCodeDescription = string.Format("set I to {0:X4}", nnn);
                    iReg = nnn;
                    pc += 2;
                    return true;

                case Operation.opBNNN: // Jump to the address NNN plus vReg[0]
                    opCodeDescription = string.Format("jump to address {0:X4} + V[0]", nnn);
                    pc = (ushort)(nnn + vReg[0]);
                    return true;

                case Operation.opCXNN: // Set vReg[X] to a random number and NN
                    opCodeDescription = string.Format("Set V[{0:X2}] to a random number and {1:X2}", x, nn);
                    vReg[x] = (byte)((random.Next() % 0xFF) & (opCode & 0x00FF));
                    pc += 2;
                    return true;

                // DXYN draws a sprite at coordinate (vReg[X], vReg[Y]) that has a width of 8 pixels and a height of N pixels. 
                // Each row of 8 pixels is read as bit-coded starting from memory location iReg. The value in iReg doesn't 
                // change after the execution of this instruction. vReg[F] is set to 1 if any screen pixels are flipped from 
                // set to unset when the sprite is drawn, and to 0 if that doesn't happen
                case Operation.opDXYN:
                    opCodeDescription = string.Format("draw 8*{0} sprite at ({1},{2})", n, vReg[x], vReg[y]);
                    byte pixel;
                    vReg[0xF] = 0;
                    for (int yline = 0; yline < n; yline++)
                    {
                        pixel = memory[iReg + yline];
                        for (int xline = 0; xline < 8; xline++)
                        {
                            if ((pixel & (0x80 >> xline)) != 0)
                            {
                                var pos = vReg[x] + xline + (vReg[y] + yline) * 64;
                                if (pos > -1 && pos < DISPLAYSIZE)
                                {
                                    if (display[pos])
                                        vReg[0xF] = 1;
                                    display[pos] = !display[pos]; //^= true;
                                }
                            }
                        }
                    }
                    drawFlag = true;
                    pc += 2;
                    return true;

                case Operation.opEX9E: // Skip the next instruction if the key stored in vReg[X] is pressed
                    opCodeDescription = "TODO";
                    pc += (byte)((keys[vReg[x]]) ? 4 : 2);
                    return true;

                case Operation.opEXA1: // Skip the next instruction if the key stored in vReg[X] isn't pressed
                    opCodeDescription = "TODO";
                    pc += (byte)((keys[vReg[x]]) ? 2 : 4);
                    return true;

                case Operation.opFX07: // Set vReg[X] to the value of the delay timer
                    opCodeDescription = "TODO";
                    vReg[x] = delayTimer;
                    pc += 2;
                    return true;

                case Operation.opFX0A: // A key press is awaited, and then stored in vReg[X]		
                    opCodeDescription = "TODO";
                    bool keyPress = false;
                    for (byte i = 0; i < 16; i++)
                    {
                        if (keys[i])
                        {
                            vReg[x] = i;
                            keyPress = true;
                        }
                    }
                    if (keyPress) // If we didn't receive a keypress, skip this cycle and try again (i.e. don't increase pc).
                        pc += 2;
                    return true;

                case Operation.opFX15: // Set the delay timer to vReg[X]
                    opCodeDescription = "TODO";
                    delayTimer = vReg[x];
                    pc += 2;
                    return true;

                case Operation.opFX18: // Sets the sound timer to vReg[X]
                    opCodeDescription = "TODO";
                    soundTimer = vReg[x];
                    pc += 2;
                    return true;

                case Operation.opFX1E: // Add vReg[X] to iReg, setting vReg[0xF] to 1 when range overflow (I+VX>0xFFF), and 0 when there isn't.
                    opCodeDescription = "TODO";
                    vReg[0xF] = (byte)((iReg + vReg[x] > 0xFFF) ? 1 : 0);
                    iReg += vReg[x];
                    pc += 2;
                    return true;

                case Operation.opFX29: // Set iReg to the location of the sprite for the character in vReg[x]. Characters 0-F (in hex) are represented by a 4x5 font
                    opCodeDescription = "TODO";
                    iReg = (ushort)(vReg[x] * 0x5);
                    pc += 2;
                    return true;

                case Operation.opFX33: // Store the BCD representation of vReg[x] at the addresses iReg, iReg + 1, and iReg + 2
                    opCodeDescription = "TODO";
                    memory[iReg] = (byte)(vReg[x] / 100);
                    memory[iReg + 1] = (byte)((vReg[x] / 10) % 10);
                    memory[iReg + 2] = (byte)((vReg[x] % 100) % 10);
                    pc += 2;
                    return true;

                case Operation.opFX55: // Store vReg[0] to vReg[x] in memory starting at address iReg					
                    opCodeDescription = "TODO";
                    for (int i = 0; i <= x; i++)
                        memory[iReg + i] = vReg[i];
                    iReg += (ushort)(x + 1); // On the original interpreter, when the operation is done, I = I + X + 1.
                    pc += 2;
                    return true;

                case Operation.opFX65: // Fill vReg[0] to vReg[x] with values from memory starting at address iReg					
                    opCodeDescription = "TODO";
                    for (int i = 0; i <= x; i++)
                        vReg[i] = memory[iReg + i];
                    iReg += (ushort)(x + 1); // On the original interpreter, when the operation is done, I = I + X + 1.
                    pc += 2;
                    return true;

                default:
                    opCodeDescription = "unknown opCode";
                    return false;
            }
        }
        #endregion


        public bool[] Keys { get { return keys; } }
        public bool[] Display { get { return display; } }
        public byte[] Memory {  get { return memory; } }

        public bool drawFlag { get; set; }

        public string debugGetState()
        {
            return string.Format(
                "PC:{0:X4} I:{1:X4} SP:{2:X4} V:{3:X2},{4:X2},{5:X2},{6:X2},{7:X2},{8:X2},{9:X2},{10:X2}" +
                "{11:X2},{12:X2},{13:X2},{14:X2},{15:X2},{16:X2},{17:X2},{18:X2} DT:{19:X2} ST:{20:X2} KEYS:{21}",
                pc, iReg, sp, vReg[0], vReg[1], vReg[2], vReg[3], vReg[4], vReg[5], vReg[6], vReg[7],
                vReg[8], vReg[9], vReg[10], vReg[11], vReg[12], vReg[13], vReg[14], vReg[15], delayTimer, soundTimer, "0000000000000000");
        }

        public string debugGetOperation()
        {
            return string.Format("{0:X4} - {1}", opCode, opCodeDescription);
        }

        public void initialize()
        {
            pc = 0x200; // Program counter starts at 0x200 (Start address of loaded program)
            opCode = 0;
            iReg = 0;
            sp = 0;

            // Clear the display
            for (int i = 0; i < DISPLAYSIZE; i++)
            {
                display[i] = false;
            }

            // Clear the stack
            for (int i = 0; i < STACKSIZE; i++)
            {
                stack[i] = 0;
            }

            // Reset all keys
            for (int i = 0; i < KEYCOUNT; i++)
            {
                keys[i] = false;
            }

            // Load the font set into the first 80 bytes of memory, then clear the rest of the memory.
            for (int i = 0; i < CHARCOUNT; i++)
            {
                memory[i] = fontSet[i];
            }
            for (int i = CHARCOUNT; i < MEMORYSIZE; i++)
            {
                memory[i] = 0;
            }

            // Reset timers
            delayTimer = 0;
            soundTimer = 0;

            drawFlag = true;
        }

        public void load(byte[] bytes)
        {
            var address = 0x200;
            foreach (var b in bytes)
            {
                memory[address] = b;
                address++;
            }
        }

        public bool emulateCycle()
        {
            byte b1 = memory[pc];
            byte b2 = memory[pc + 1];

            // Fetch opcode
            opCode = (ushort)(0x0100 * b1 + b2);

            // Process opcode
            bool result = processOpCode(opCode);

            // Update timers
            if (delayTimer > 0)
            {
                delayTimer--;
            }
            if (soundTimer > 0)
            {
                if (soundTimer == 1)
                {
                    Console.Beep();
                }
                soundTimer--;
            }

            return result;
        }
    }
}
