using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Neuroevolution
{
    public static class CustomRandom
    {
        public static readonly Random Random = new Random(10);

        public static float GetUnsignedValue()
        {
            return (float)Random.NextDouble();
        }

        public static float GetSignedValue()
        {
            return (float)Random.NextDouble() * 2 - 1;
        }
        
        /// <summary>
        /// Get random int
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"> Excluded </param>
        /// <returns></returns>
        public static int Range(int min, int max)
        {
            return Random.Next(min, max);
        }
    }
}
