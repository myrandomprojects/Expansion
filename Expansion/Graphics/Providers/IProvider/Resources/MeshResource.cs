using Expansion.Engine.Math3D;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Expansion.Graphics.Providers.IProvider.Resources
{
    public partial class MeshResource : GPUResource
    {
        public string Path { get; }

        internal Vertex[] vertices;
        internal int[] indices;


        private int getVertexIndex(string[] vals, int o, List<Vertex> vertices, Dictionary<long, int> vCache, List<Vector> v, List<Vector> vt, List<Vector> vn)
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
                Vector p = v[indices[0] - 1];
                vertices.Add(new Vertex(p, vn[indices[2] - 1], indices[1] == 0 ? new Vector(p.values[0] + p.values[1], p.values[0] + p.values[2]) * 0.01f : vt[indices[1] - 1]));
                vCache.Add(key, ind);
                return ind;
            }

            return vCache[key];
        }
        internal MeshResource(string path)
        {
            Path = path;

            var lines = File.ReadAllLines(path);

            var v = new List<Vector>();
            var vt = new List<Vector>();
            var vn = new List<Vector>();

            var vertices = new List<Vertex>();
            var indices = new List<int>();

            var vCache = new Dictionary<long, int>();

            foreach (var line in lines)
            {
                var vals = line.Split(' ');

                if (line.StartsWith("vt"))
                    vt.Add(new Vector(vals.Skip(1).Select(val => float.Parse(val)).ToArray()));
                else if (line.StartsWith("vn"))
                    vn.Add(new Vector(vals.Skip(1).Select(val => float.Parse(val)).ToArray()));
                else if (line.StartsWith("v"))
                    v.Add(new Vector(vals.Skip(vals.Length - 3).Select(val => float.Parse(val)).ToArray()));
                else if (line.StartsWith("f"))
                {
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

            this.vertices = vertices.ToArray();
            this.indices = indices.ToArray();
        }
    }

    internal class MeshImpl
    {
        internal VertexBuffer Vertices { get; }
        internal IndexBuffer Indices { get; }

        public MeshImpl(VertexBuffer vBuf, IndexBuffer iBuf)
        {
            Vertices = vBuf;
            Indices = iBuf;
        }
    }
}