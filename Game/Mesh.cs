﻿using OpenCLTemplate;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Expansion_CSharp
{
    class Vector1
    {
        public float[] values;

        public float Length { get { return (float)Math.Sqrt(values.Select(v => v * v).Sum()); } }
        public Vector1 Normalized { get { return this * (1 / Length); } }

        public Vector1(params float[] v)
        {
            values = v;
        }

        static public Vector1 operator +(Vector1 a, Vector1 b)
        {
            float[] v2 = new float[a.values.Length];
            for (int i = 0; i < v2.Length; i++)
                v2[i] = a.values[i] + b.values[i];
            return new Vector1(v2);
        }

        static public Vector1 operator -(Vector1 a, Vector1 b)
        {
            float[] v2 = new float[a.values.Length];
            for (int i = 0; i < v2.Length; i++)
                v2[i] = a.values[i] - b.values[i];
            return new Vector1(v2);
        }

        static public Vector1 operator *(Vector1 a, float b)
        {
            float[] v2 = new float[a.values.Length];
            for (int i = 0; i < v2.Length; i++)
                v2[i] = a.values[i] * b;
            return new Vector1(v2);
        }

        public override string ToString()
        {
            return "{" + string.Join(";", values.Select(v => string.Format("{0:0.00}", v))) + "}";
        }

        internal bool IsZero()
        {
            return values.All(v => v == 0);
        }
    }
    class Vertex
    {
        public float[] values = new float[16];
        int tCount = 0;

        public Vector1 Position { get { return new Vector1(values[0], values[1], values[2]); } set { values[0] = value.values[0]; values[1] = value.values[1]; values[2] = value.values[2]; } }
        public Vector1 Normal { get { return new Vector1(values[4], values[5], values[6]); } set { values[4] = value.values[0]; values[5] = value.values[1]; values[6] = value.values[2]; } }
        public Vector1 TexUV { get { return new Vector1(values[8], values[9]); } set { values[8] = value.values[0]; values[9] = value.values[1]; } }
        public Vector1 Tangent { get { return new Vector1(values[10], values[11], values[12]); } set { values[10] = value.values[0]; values[11] = value.values[1]; values[12] = value.values[2]; } }
        public Vector1 Binormal { get { return new Vector1(values[13], values[14], values[15]); } set { values[13] = value.values[0]; values[14] = value.values[1]; values[15] = value.values[2]; } }

        public Vertex(Vector1 pos, Vector1 norm, Vector1 uv)
        {
            if (pos != null)
                Position = pos;

            if (norm != null)
                Normal = norm;

            if (uv != null)
                TexUV = uv;
        }

        public void AddTangentBinormal(Vector1 tangent, Vector1 binormal)
        {
            Tangent = (Tangent * tCount + tangent) * (1 / (tCount + 1));
            Binormal = (Binormal * tCount + binormal) * (1 / (tCount + 1));
        }
    }
    class Mesh1
    {
        public Vertex[] Vertices { get; private set; }
        public int[] Indices { get; private set; }

        public CLCalc.Program.Variable VBuffer { get; private set; }
        public CLCalc.Program.Variable IBuffer { get; private set; }

        public void PrecalculateTangentBinormal()
        {
            var v = Vertices;
            var ind = Indices;
            for (int i = 0; i < Indices.Length; i += 3)
            {
                Vector1 v1, v2;
                Vector1 vt1, vt2;
                //float den;

                if (v[ind[i + 0]].TexUV.IsZero() && v[ind[i + 1]].TexUV.IsZero() && v[ind[i + 2]].TexUV.IsZero())
                    continue;

                // Calculate the two vectors for this face.
                v1 = v[ind[i + 1]].Position - v[ind[i + 0]].Position;
                v2 = v[ind[i + 2]].Position - v[ind[i + 0]].Position;

                // Calculate the tu and tv texture space vectors.
                vt1 = v[ind[i + 1]].TexUV - v[ind[i + 0]].TexUV;
                vt2 = v[ind[i + 2]].TexUV - v[ind[i + 0]].TexUV;

                // Calculate the denominator of the tangent/binormal equation.
                //den = 1.0f / (vt1.values[0] * vt2.values[1] - vt2.values[0] * vt1.values[1]);

                // Calculate the cross products and multiply by the coefficient to get the tangent and binormal.
                Vector1 tangent = (v2 * vt1.values[1] - v1 * vt2.values[1]).Normalized;
                //tangent.x = (tvVector[1] * vector1[0] - tvVector[0] * vector2[0]) * den;
                //tangent.y = (tvVector[1] * vector1[1] - tvVector[0] * vector2[1]) * den;
                //tangent.z = (tvVector[1] * vector1[2] - tvVector[0] * vector2[2]) * den;

                Vector1 binormal = (v1 * vt2.values[0] - v2 * vt1.values[0]).Normalized;

                for (int j = 0; j < 3; j++)
                    v[ind[i + j]].AddTangentBinormal(tangent, binormal);

                //binormal.x = (tuVector[0] * vector2[0] - tuVector[1] * vector1[0]) * den;
                //binormal.y = (tuVector[0] * vector2[1] - tuVector[1] * vector1[1]) * den;
                //binormal.z = (tuVector[0] * vector2[2] - tuVector[1] * vector1[2]) * den;

                /*
                // Calculate the length of this normal.
                length = sqrt((tangent.x * tangent.x) + (tangent.y * tangent.y) + (tangent.z * tangent.z));

                // Normalize the normal and then store it
                tangent.x = tangent.x / length;
                tangent.y = tangent.y / length;
                tangent.z = tangent.z / length;

                // Calculate the length of this normal.
                length = sqrt((binormal.x * binormal.x) + (binormal.y * binormal.y) + (binormal.z * binormal.z));

                // Normalize the normal and then store it
                binormal.x = binormal.x / length;
                binormal.y = binormal.y / length;
                binormal.z = binormal.z / length;
                */
            }
        }

        public Mesh1(int vCount, int iCount)
        {
            Vertices = new Vertex[vCount];
            Indices = new int[iCount];

        }
        public Mesh1(Vertex[] v, int[] ind)
        {
            Vertices = v;
            Indices = ind;

            PrecalculateTangentBinormal();
        }

        public void LoadBuffers()
        {
            if (VBuffer != null)
                return;

            var vertexValuesPlain = new List<float>();

            foreach (var vertex in Vertices)
                vertexValuesPlain.AddRange(vertex.values);

            VBuffer = new CLCalc.Program.Variable(vertexValuesPlain.ToArray());
            IBuffer = new CLCalc.Program.Variable(Indices);
        }

        private static Mesh1 CreatePlane()
        {
            var vertices = new Vertex[4];

            for (int i = 0; i < 4; i++)
                vertices[i] = new Vertex(new Vector1(-0.5f + i % 2, 0.5f - i / 2, 0), new Vector1(0, 0, -1), new Vector1(i % 2, 1 - i / 2));

            var indices = new int[] { 0, 1, 2, 1, 3, 2 };

            return new Mesh1(vertices, indices);
        }

        private static Mesh1 CreateCube()
        {
            Mesh1 square = new Mesh1(24, 36);

            Vector1[] normals =
            {
                new Vector1(-1, 0, 0),
                new Vector1(1, 0, 0),
                new Vector1(0, -1, 0),
                new Vector1(0, 1, 0),
                new Vector1(0, 0, -1),
                new Vector1(0, 0, 1),
            };

            int vIndex = 0, iIndex = 0;

            Vector1 center = new Vector1(0, 0, 4);

            for (int i = 0; i < 6; i++)
            {
                Vector1 v1 = normals[i] * 0.5f;
                Vector1 v2 = new Vector1(v1.values[2], v1.values[0], v1.values[1]);
                Vector1 v3 = new Vector1(v1.values[1], v1.values[2], v1.values[0]);

                if (v1.values.Sum() < 0)
                    Swap(ref v2, ref v3);

                //v1 += center;

                square.Vertices[vIndex++] = new Vertex(v1 - v2 - v3, normals[i], new Vector1(0, 1));
                square.Vertices[vIndex++] = new Vertex(v1 + v2 - v3, normals[i], new Vector1(1, 1));
                square.Vertices[vIndex++] = new Vertex(v1 - v2 + v3, normals[i], new Vector1(0, 0));
                square.Vertices[vIndex++] = new Vertex(v1 + v2 + v3, normals[i], new Vector1(1, 0));

                square.Indices[iIndex++] = vIndex - 4; square.Indices[iIndex++] = vIndex - 3; square.Indices[iIndex++] = vIndex - 2;
                square.Indices[iIndex++] = vIndex - 3; square.Indices[iIndex++] = vIndex - 1; square.Indices[iIndex++] = vIndex - 2;
            }

            square.PrecalculateTangentBinormal();

            return square;
        }

        static int getVertexIndex(string[] vals, int o, List<Vertex> vertices, Dictionary<long, int> vCache, List<Vector1> v, List<Vector1> vt, List<Vector1> vn)
        {
            long key = 0;
            int[] indices = { 0, 0, 0 };
            for (int i = 0; i < 3; i++)
            {
                long.TryParse(vals[o + i], out var k);
                key = (key << 16) | k;
                indices[i] = (int)k;
            }

            if (!vCache.ContainsKey(key))
            {
                int ind = vertices.Count;
                Vector1 p = v[indices[0] - 1];
                vertices.Add(new Vertex(p, vn[indices[2] - 1], indices[1] == 0 ? new Vector1(p.values[0] + p.values[1], p.values[0] + p.values[2]) * 0.01f : vt[indices[1] - 1]));
                vCache.Add(key, ind);
                return ind;
            }

            return vCache[key];
        }

        public static Mesh1 CreateFromResource(string res)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            var lines = res.Split('\r', '\n');
            var v = new List<Vector1>();
            var vt = new List<Vector1>();
            var vn = new List<Vector1>();

            var vertices = new List<Vertex>();
            var indices = new List<int>();

            var vCache = new Dictionary<long, int>();
            //var getIndex = (vals, i)=>()

            foreach (var line in lines)
            {
                var vals = line.Split(' ');

                if (line.StartsWith("vt"))
                    vt.Add(new Vector1(vals.Skip(1).Select(val => float.Parse(val)).ToArray()));
                else if (line.StartsWith("vn"))
                    vn.Add(new Vector1(vals.Skip(1).Select(val => float.Parse(val)).ToArray()));
                else if (line.StartsWith("v"))
                    v.Add(new Vector1(vals.Skip(vals.Length - 3).Select(val => float.Parse(val)).ToArray()));
                else if (line.StartsWith("f"))
                {
                    if (vertices == null)
                    {
                        var r = new Random();
                        vertices = v.Select(vv => new Vertex(vv, new Vector1(0, 1, 0), new Vector1((float)r.NextDouble()*0, (float)r.NextDouble() * 0))).ToList();

                        for (int i = 0; i < Math.Min(v.Count, vn.Count); i++)
                            vertices[i].Normal = vn[i];

                        for (int i = 0; i < Math.Min(v.Count, vt.Count); i++)
                            vertices[i].TexUV = vt[i];
                    }
                    vals = line.Split(' ', '/');
                    indices.AddRange(new int[] {
                        getVertexIndex(vals, 1, vertices, vCache, v, vt, vn),
                        getVertexIndex(vals, 4, vertices, vCache, v, vt, vn),
                        getVertexIndex(vals, 7, vertices, vCache, v, vt, vn)
                    });
                    if (vals.Length > 11)
                        indices.AddRange(new int[] {
                            getVertexIndex(vals, 1, vertices, vCache, v, vt, vn),
                            getVertexIndex(vals, 7, vertices, vCache, v, vt, vn),
                            getVertexIndex(vals, 10, vertices, vCache, v, vt, vn)
                        });
                }

            }

            return new Mesh1(vertices.ToArray(), indices.ToArray());
        }

        public static readonly Mesh1 Plane = CreatePlane();
        public static readonly Mesh1 Cube = CreateCube();
        public static readonly Mesh1 Level = CreateFromResource(Resources.level);
        public static readonly Mesh1 SM_Rock_Chunk = CreateFromResource(Resources.SM_Rock_Chunk);
        public static readonly Mesh1 Minion = CreateFromResource(Resources.Minion);
        public static readonly Mesh1 Buff_White = CreateFromResource(Resources.Buff_White);


        static void Swap<T>(ref T x, ref T y)
        {
            T t = y;
            y = x;
            x = t;
        }
    }
}
