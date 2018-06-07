using System.Linq;

namespace Expansion_CSharp
{
    class Vector
    {
        public float[] values;

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
    }
    class Vertex
    {
        public float[] values = new float[12];

        public Vector Position { get { return new Vector(values[0], values[1], values[2]); } set { values[0] = value.values[0]; values[1] = value.values[1]; values[2] = value.values[2]; } }
        public Vector Normal { get { return new Vector(values[4], values[5], values[6]); } set { values[4] = value.values[0]; values[5] = value.values[1]; values[6] = value.values[2]; } }
        public Vector TexUV { get { return new Vector(values[8], values[9]); } set { values[8] = value.values[0]; values[9] = value.values[1]; } }

        public Vertex(Vector pos, Vector norm, Vector uv)
        {
            Position = pos;
            Normal = norm;
            TexUV = uv;
        }
    }
    class Mesh
    {
        public Vertex[] Vertices { get; private set; }
        public int[] Indices { get; private set; }

        public Mesh(int vCount, int iCount)
        {
            Vertices = new Vertex[vCount];
            Indices = new int[iCount];
        }

        private static Mesh CreateCube()
        {
            Mesh square = new Mesh(24, 36);

            Vector[] normals =
            {
                new Vector(-1, 0, 0),
                new Vector(1, 0, 0),
                new Vector(0, -1, 0),
                new Vector(0, 1, 0),
                new Vector(0, 0, -1),
                new Vector(0, 0, 1),
            };

            int vIndex = 0, iIndex = 0;

            Vector center = new Vector(0, 0, 4);

            for (int i = 0; i < 6; i++)
            {
                Vector v1 = normals[i]*0.5f;
                Vector v2 = new Vector(v1.values[2], v1.values[0], v1.values[1]);
                Vector v3 = new Vector(v1.values[1], v1.values[2], v1.values[0]);

                if (v1.values.Sum() < 0)
                    Swap(ref v2, ref v3);

                //v1 += center;

                square.Vertices[vIndex++] = new Vertex(v1 - v2 - v3, normals[i], new Vector(0, 0));
                square.Vertices[vIndex++] = new Vertex(v1 + v2 - v3, normals[i], new Vector(1, 0));
                square.Vertices[vIndex++] = new Vertex(v1 - v2 + v3, normals[i], new Vector(0, 1));
                square.Vertices[vIndex++] = new Vertex(v1 + v2 + v3, normals[i], new Vector(1, 1));

                square.Indices[iIndex++] = vIndex - 4; square.Indices[iIndex++] = vIndex - 3; square.Indices[iIndex++] = vIndex - 2;
                square.Indices[iIndex++] = vIndex - 3; square.Indices[iIndex++] = vIndex - 1; square.Indices[iIndex++] = vIndex - 2;
            }

            return square;
        }
        
        public static readonly Mesh Cube = CreateCube();




        static void Swap<T>(ref T x, ref T y)
        {
            T t = y;
            y = x;
            x = t;
        }
    }
}
