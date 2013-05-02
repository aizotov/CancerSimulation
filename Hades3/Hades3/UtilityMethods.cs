using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Hades3
{
    class UtilityMethods
    {

        public static Vector3 FindEquidistantPoint(Vector3 v1, Vector3 v2)
        {
            Console.WriteLine("equidistant point between" + v1 + "  and  " + v2 + "  is...");

            float x = Math.Abs(v1.X - v2.X);
            x = x / 2;
            float y = Math.Abs(v1.Y - v2.Y);
            y = y / 2;
            float z = Math.Abs(v1.Z - v2.Z);
            z = z / 2;

            Console.WriteLine(new Vector3(x, y, z));

            return new Vector3(x, y, z);
        }

        public static String ListToString<T>(List<T> collection)
        {
            String s = "";
            foreach (T element in collection)
                s += element.ToString();
            return s;
        }

        public static int CityBlockDistance(Vector3 v1, Vector3 v2)
        {
            return (int)(Math.Abs(v1.X - v2.X) + Math.Abs(v1.Y - v2.Y) + Math.Abs(v1.Z - v2.Z));
        }

        public static T GetRandomElementInList<T>(IList<T> collection)
        {
            Random r = new Random();
            return collection[r.Next(collection.Count)];
        }

        public static float AbsoluteMax(float x, float y, float z)
        {
            return Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(z));
        }

    }
}
