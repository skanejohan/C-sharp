using System;

namespace Pgen
{
    class PgenImpl
    {
        private static char returnLastDigitOfAsciiCode(char c)
        {
            string s = string.Format("{0}", (int)c);
            return s[s.Length - 1];
        }

        public static string convert(string start)
        {
            /*
             * Calculate the sum of all ASCII values, and retrieve the least significant byte.
             * Multiply each character with X times this value, where X is the character's position 
             *   in the string, and retrieve the least significant byte.
             * Map all characters at positions 0, 4, 8 etc. to an upper-case letter.
             * Map all characters at positions 1, 3, 5, 7 etc. to a lower-case letter.
             * Map all characters at position 2, 6, 10 etc. to a digit.
             */
            char[] A = start.ToCharArray();

            int sum = 0;
            for (int i = 0; i < A.Length; i++)
            {
                sum += (int)A[i];
            }
            sum = sum % 256;

            for (int i = 0; i < A.Length; i++)
            {
                A[i] = (char)(((i + 1) * sum * (int)A[i]) % 256);
            }

            for (int i = 0; i < A.Length; i++)
            {
                if (i % 4 == 0) // 0, 4, 8...
                {
                    A[i] = (char)((int)'A' + ((int)A[i] % 26));
                }
                else if (i % 2 == 1) // 1, 3, 5, 7...
                {
                    A[i] = (char)((int)'a' + ((int)A[i] % 26));
                }
                else //2, 6, 10...
                {
                    A[i] = (char)((int)'0' + ((int)A[i] % 10));
                }
            }

            return new String(A);

            /*
             * 
             * 
             * // Step 1 - reverse the string and store it in an array
            char[] A = start.ToCharArray();
            Array.Reverse(A);
            
            // Step 2 - add (sum of all previous characters) to each character
            for (int i = 0; i < A.Length; i++)
            {
                //offset += (int)A[i];
                A[i] = (char) ((int)A[i] + 2);
            }

            // Step 3 - reverse case on every second (1, 3, 5...) character if it is within A-Z or a-z.
            for (int i = 0; i < A.Length; i+=2)
            {

                if (A[i] >= 'A' && A[i] <= 'Z')
                {
                    A[i] = char.ToLower(A[i]);
                }
                else if (A[i] >= 'a' && A[i] <= 'z')
                {
                    A[i] = char.ToUpper(A[i]);
                }
            }

            // Step 4 - replace all characters outside the (A-Z, a-z) range with the last digit of its ASCII code
            for (int i = 0; i < A.Length; i += 2)
            {
                if (!char.IsLetter(A[i]))
                {
                    A[i] = returnLastDigitOfAsciiCode(A[i]);
                }
            }

            // Step 5 - replace every third (1, 4, 7...) character with the last digit of its ASCII code 
            for (int i = 0; i < A.Length; i += 3)
            {
                A[i] = returnLastDigitOfAsciiCode(A[i]);
            }
            return new String(A);
             */
        }
    }
}
