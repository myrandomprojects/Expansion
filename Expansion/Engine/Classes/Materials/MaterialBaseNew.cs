using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expansion.Engine.Classes.MaterialsNew
{
    public enum VertexParamType
    {
        Position,
        Normal,
        Tangent,
        Binormal,
        TexUV,
        Color
    }

    class ValueBuffer : Attribute
    {
        public string BufferName { get; set; }
        public ValueBuffer(string name) { BufferName = name; }
    }

    class MaterialBase
    {
        [ValueBuffer("MVP")] public int World;
        [ValueBuffer("MVP")] public int View;
        [ValueBuffer("MVP")] public int Projection;

        [ValueBuffer("Light")] public int LightPos;
    }
}
