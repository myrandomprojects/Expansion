using System.Collections.Generic;

namespace Expansion.Graphics.Providers.IProvider.Resources
{
    internal class Shader
    {
        public string Source { get; }

        public List<ShaderVariable> Inputs { get; } = new List<ShaderVariable>();

        protected Shader(string source)
        {
            Source = source;
        }
    }

    internal partial class VertexShader : Shader
    {
        public VertexShader(string source) : base(source)
        {

        }
    }

    internal partial class PixelShader : Shader
    {
        public TextureResource[] MatTextures { get; internal set; }

        public PixelShader(string source) : base(source)
        {
            
        }
    }
}