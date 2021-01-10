using Expansion.Engine.Classes.Materials;
using Expansion.Graphics.Providers.IProvider.Resources;
using static OpenCLTemplate.CLCalc.Program;

namespace Expansion.Graphics.Providers.OpenCL
{
    internal class VertexShaderCL
    {
        Kernel shader;

        public VertexShaderCL(VertexShaderBase vs)
        {

        }
    }

    internal class PixelShaderCL
    {
        Kernel shader;
        
        public PixelShaderCL(PixelShaderBase ps)
        {

        }
    }
}