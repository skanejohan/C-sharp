// Copyright (c) 2006 Derek Slager
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Security;
using System.Text;

namespace DerekSlager.IO {

    /// <summary>
    /// Reads masked input from the console.
    /// </summary>
    public static class MaskedConsoleReader {

        private const char MaskCharacter = '*';

        /// <summary>
        /// Reads a single line of text from the system console,
        /// masking the typed characters.
        /// </summary>
        public static string ReadLine() {
            StringBuilder buffer = new StringBuilder(64);
            ReadLine(new StringBuffer(buffer), MaskCharacter);
            return buffer.ToString();
        }

        /// <summary>
        /// Reads a single line of text from the system console as a
        /// <c>SecureString</c> instance, masking the typed
        /// characters.
        /// </summary>
        /// <remarks>
        /// The caller is responsible for freeing the returned string
        /// after usage by calling its <c>Dispose()</c> method.
        /// </remarks>
        public static SecureString ReadSecureLine() {

            SecureString secureString = null;
            try {
                secureString = new SecureString();
                ReadLine(new SecureStringBuffer(secureString),
                         MaskCharacter);
                secureString.MakeReadOnly();
                return secureString;
            } catch {
                if (secureString != null) {
                    secureString.Dispose();
                }
                throw;
            }
        }

        private static void ReadLine(IBuffer buffer, char maskChar) {

            int startPosition = Console.CursorLeft;

            int position = 0;
            int length = 0;

            ConsoleKeyInfo keyInfo;
            while ((keyInfo = Console.ReadKey(true)).Key != ConsoleKey.Enter) {
                if (keyInfo.Key == ConsoleKey.Backspace) {
                    if (position > 0) {
                        buffer.DeleteChar(--position);
                        Write(startPosition + --length, ' ');
                    }
                } else if (keyInfo.Key == ConsoleKey.UpArrow ||
                           keyInfo.Key == ConsoleKey.PageUp ||
                           keyInfo.Key == ConsoleKey.Escape) {
                    // Match buffer-empty 'doskey' scenario for up
                    // arrow & page up.
                    buffer.Clear();
                    for ( ; length >= 0; length--) {
                        Write(startPosition + length, ' ');
                    }
                    position = length = 0;
                } else if (keyInfo.Key == ConsoleKey.DownArrow ||
                           keyInfo.Key == ConsoleKey.PageDown) {
                    // No-op.
                } else if (keyInfo.Key == ConsoleKey.LeftArrow) {
                    position = Math.Max(position - 1, 0);
                } else if (keyInfo.Key == ConsoleKey.RightArrow) {
                    position = Math.Min(position + 1, length);
                } else if (keyInfo.Key == ConsoleKey.Delete) {
                    if (position < length) {
                        buffer.DeleteChar(position);
                        Write(startPosition + --length, ' ');
                    }
                } else if (keyInfo.Key == ConsoleKey.Home) {
                    position = 0;
                } else if (keyInfo.Key == ConsoleKey.End) {
                    position = length;
                } else {
                    buffer.InsertChar(position, keyInfo.KeyChar);
                    position++;
                    Write(startPosition + length, '*');
                    length++;
                }

                Console.CursorLeft = position + startPosition;
            }

            Console.WriteLine();
        }

        private static void Write(int index, char c) {
            Console.CursorLeft = index;
            Console.Write(c);
        }

        private interface IBuffer {
            void InsertChar(int index, char c);
            void DeleteChar(int index);
            void Clear();
        }

        private class StringBuffer : IBuffer {
            private StringBuilder buffer;
            public StringBuffer(StringBuilder buffer) {
                this.buffer = buffer;
            }
            public void InsertChar(int index, char c) {
                this.buffer.Insert(index, c);
            }
            public void DeleteChar(int index) {
                buffer.Remove(index, 1);
            }
            public void Clear() {
                this.buffer.Length = 0;
            }
        }

        private class SecureStringBuffer : IBuffer {
            private SecureString buffer;
            public SecureStringBuffer(SecureString buffer) {
                this.buffer = buffer;
            }
            public void InsertChar(int index, char c) {
                this.buffer.InsertAt(index, c);
            }
            public void DeleteChar(int index) {
                this.buffer.RemoveAt(index);
            }
            public void Clear() {
                this.buffer.Clear();
            }
        }

    }

}
