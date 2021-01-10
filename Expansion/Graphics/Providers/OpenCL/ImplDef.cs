using Expansion.Graphics.Providers.OpenCL;
using System.Drawing;
using static OpenCLTemplate.CLCalc.Program;

namespace Expansion.Graphics.Providers.IProvider.Resources
{
    public partial class MeshResource : GPUResource
    {
        internal MeshImpl meshCL;
    }

    public partial class TextureResource
    {
        Image2D textureCL;
        internal Image2D TextureCL => textureCL = textureCL ?? new Image2D((Bitmap)Image.FromFile(TexturePath));
    }

    internal partial class PixelShader : Shader
    {
        //PixelShaderCL ps;

        //internal PixelShaderCL ImplCL { get { return ps = ps ?? new PixelShaderCL(this); } }

    }
}
namespace Expansion.Graphics.Providers.IProvider
{
    public partial class Material
    {
        //MaterialCL matCL;

        //internal MaterialCL MaterialCL { get { return matCL = matCL ?? new MaterialCL(Code); } }
    }
}

namespace Expansion.Engine.Classes.Materials
{
    public abstract partial class VertexShaderBase
    {
        VertexShaderCL vsCL;
        internal VertexShaderCL ShaderCL => vsCL = vsCL ?? new VertexShaderCL(this);
    }


    public abstract partial class PixelShaderBase
    {
        PixelShaderCL psCL;
        internal PixelShaderCL ShaderCL => psCL = psCL ?? new PixelShaderCL(this);
    }
}