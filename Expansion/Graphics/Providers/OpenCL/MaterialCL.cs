using Expansion.Graphics.Providers.IProvider;
using System.Linq;
using static OpenCLTemplate.CLCalc.Program;

namespace Expansion.Graphics.Providers.OpenCL
{
    internal class MaterialCL
    {
        Kernel kernel;

        internal MaterialCL(Material mat)
        {
            var rastCode = "MAT_FUNC Material_" + mat.Name + "(MAT_ARGS()" + string.Join("", mat.MatTextures.Select(n => ", read_only image2d_t " + n.Name)) + ") " +
                "{ \r\n    MAT_HEADER() \r\n//" + mat.Code + "MAT_FOOTER() }";
        }
    }
}