using OpenCLTemplate;
using System.Drawing;

namespace Expansion_CSharp
{
    internal class Texture
    {
        static int _ID = 0;
        public readonly int ID = _ID++;

        public Bitmap bmp { get; private set; }
        public CLCalc.Program.Image2D GPUResource { get; private set; }
        
        public Texture(Bitmap bmp)
        {
            this.bmp = bmp;
            GPUResource = new CLCalc.Program.Image2D(bmp);
        }
    }
}