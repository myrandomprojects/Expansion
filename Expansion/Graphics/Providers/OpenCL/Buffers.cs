using Expansion.Engine.Math3D;
using Expansion.Graphics.Providers.IProvider.Resources;
using System.Linq;
using static OpenCLTemplate.CLCalc.Program;

namespace Expansion.Graphics.Providers.OpenCL
{
    internal class VertexBufferCL : VertexBuffer
    {
        public Variable VBuffer { get; }

        public VertexBufferCL(Vertex[] vertices)
            : base(vertices.Length)
        {
            VBuffer = new Variable(vertices.SelectMany(v => v.values).ToArray());
        }
    }
    internal class IndexBufferCL : IndexBuffer
    {
        public Variable IBuffer { get; }

        public IndexBufferCL(int[] indices)
            : base(2, indices.Length, 0)
        {
            IBuffer = new Variable(indices.Select(i => (short)i).ToArray());
        }
    }
}