using System;
using System.Linq;

namespace Expansion.Engine.Math3D
{
    public class Vector
    {
        public float[] values;

        public float X { get { return values[0]; } set { values[0] = value; } }
        public float Y { get { return values[1]; } set { values[1] = value; } }
        public float Z { get { return values[2]; } set { values[2] = value; } }

        public float Length { get { return (float)Math.Sqrt(values.Select(v => v * v).Sum()); } }
        public Vector Normalized { get { return this * (1 / Length); } }
        public bool IsZero { get { return values.All(v => v == 0); } }
        
        public static Vector Zero3 { get; } = new Vector(0, 0, 0);
        public static Vector One3 { get; } = new Vector(1, 1, 1);

        public Vector(params float[] v)
        {
            values = v;
        }

        static public Vector operator +(Vector a, Vector b)
        {
            float[] v2 = new float[a.values.Length];
            for (int i = 0; i < v2.Length; i++)
                v2[i] = a.values[i] + b.values[i];
            return new Vector(v2);
        }

        static public Vector operator -(Vector a, Vector b)
        {
            float[] v2 = new float[a.values.Length];
            for (int i = 0; i < v2.Length; i++)
                v2[i] = a.values[i] - b.values[i];
            return new Vector(v2);
        }

        static public Vector operator *(Vector a, float b)
        {
            float[] v2 = new float[a.values.Length];
            for (int i = 0; i < v2.Length; i++)
                v2[i] = a.values[i] * b;
            return new Vector(v2);
        }

        public override string ToString()
        {
            return "{" + string.Join(";", values.Select(v => string.Format("{0:0.00}", v))) + "}";
        }

    }
}