using Cloo;
using Expansion.Graphics.Providers.IProvider.Resources;
using static OpenCLTemplate.CLCalc.Program;

namespace Expansion.Graphics.Providers.OpenCL
{
    internal class RenderTargetCL : RenderTarget
    {
        //internal Variable renderTexture;
        internal Image2D screen;
        internal Image2D depth;

        public RenderTargetCL(int w, int h)
        {
            Width = w;
            Height = h;

            //renderTexture = new Variable(typeof(float), 2 * w * h);
            screen = new Image2D(ComputeImageChannelType.UnsignedInt8, w, h);
            depth = new Image2D(ComputeImageChannelType.UnsignedInt8, w, h);
        }


    }
}