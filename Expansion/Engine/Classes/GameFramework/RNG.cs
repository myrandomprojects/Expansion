using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expansion.Engine.Classes.GameFramework
{
    public class RNG
    {
        static public Random Random { get; } = new Random();

        static public int Next(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static T Choice<T>(IEnumerable<T> arr)
        {
            return Choice(arr.ToArray());
        }
        public static T Choice<T>(List<T> arr)
        {
            return arr[Random.Next(arr.Count)];
        }
        public static T Choice<T>(T[] arr)
        {
            return arr[Random.Next(arr.Length)];
        }
    }
}
