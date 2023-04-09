using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AutoBattle
{
    public class Utilities
    {
        public static Random rng = new Random();

        // Returns a random integer in a range
        public static int GetRandomInt(int min, int max)
        {
            int index = rng.Next(min, max);
            return index;
        }

        public static float GetRandomFloat(float min, float max)
        {
            return (float)(min + (max - min) * rng.NextDouble());
        }

        // Validates a string given a pattern
        public Match ValidateString(string toValidate, string pattern)
        {
            return Regex.Match(toValidate, pattern);
        }

    }
}
