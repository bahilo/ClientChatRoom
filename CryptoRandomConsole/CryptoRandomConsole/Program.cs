using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoRandomConsole
{
    class Program
    {
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        // Main method.
        public static void Main()
        {
            

            const int totalRolls = 25;
            int[] results = new int[10];
            int[] output = new int[totalRolls];
            // Roll the dice 25000 times and display
            // the results to the console.
            for (int x = 0; x < totalRolls; x++)
            {
                byte roll = RollDice(5,7);
                output[x] = roll;
            }
            for (int i = 0; i < output.Length; ++i)
            {
                Console.WriteLine("{0}: {1}", i + 1, output[i]);
            }
            rngCsp.Dispose();
            Console.ReadLine();/**/
        }

        // This method simulates a roll of the dice. The input parameter is the
        // number of sides of the dice.

        public static byte RollDice(int min, int max)
        {

            if (min > max || min <= 0)
                throw new ArgumentOutOfRangeException("Range min and max for random numbers is not correct");

            byte[] randomNumber = new byte[1];
            bool IsminRandom;
            bool IsMaxRandom;
            do
            {
                rngCsp.GetBytes(randomNumber);
                byte Val = (byte)((randomNumber[0] % (byte)max) + 1);
                IsminRandom = (byte)min <= Val;
                IsMaxRandom = Val <= (byte)max;
            } while (!(IsminRandom && IsMaxRandom));
            

            return (byte)((randomNumber[0] % max) + 1);
        }

        private static bool IsFairRoll(byte roll, byte numSides)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a 6 sided die, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = Byte.MaxValue / numSides;

            // If the roll is within this range of fair values, then we let it continue.
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            return roll < Byte.MaxValue;// numSides * fullSetsOfValues;
        }
    }
}
