using Cloo;
using OpenCLTemplate;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Expansion_CSharp
{
    class Renderer
    {
        static int[] ScreenSize = { 640, 640 };

        static CLCalc.Program.Kernel clear;
        static CLCalc.Program.Kernel project;
        static CLCalc.Program.Kernel rasterize;
        static CLCalc.Program.Kernel finalize;

        static CLCalc.Program.Variable
            renderTexture,
            clearColor,
            worldSettings,
            trianglesBuff;
        static CLCalc.Program.Image2D screen;

        static public CLCalc.Program.Image2D Texture0 { get; set; }
        static public CLCalc.Program.Image2D Texture1 { get; set; }

        static public List<Material> ResourceTextures { get; private set; }

        private static float[] GetWorldSettings(Vector lightdir)
        {
            lightdir = lightdir * (1 / lightdir.Length);

            return new float[] {
                lightdir.values[0], lightdir.values[1], lightdir.values[2], 0,
                ScreenSize[0], ScreenSize[1]
            };
        }






        public static void Initialize()
        {
            CLCalc.InitCL();
            List<ComputeDevice> L = CLCalc.CLDevices;
            CLCalc.Program.DefaultCQ = 0;


            string code = ReadFilesOfType("*.h") + ReadFilesOfType("*.cpp");
            CLCalc.Program.Compile(code, out List<string> logs);

            clear = new CLCalc.Program.Kernel("clear");
            project = new CLCalc.Program.Kernel("project");
            rasterize = new CLCalc.Program.Kernel("rasterize");
            finalize = new CLCalc.Program.Kernel("finalize");

            renderTexture = new CLCalc.Program.Variable(typeof(float), 2 * ScreenSize[0] * ScreenSize[1]);
            clearColor = new CLCalc.Program.Variable(new byte[4] { 100, 149, 237, 255 });
            screen = new CLCalc.Program.Image2D(ComputeImageChannelType.UnsignedInt8, ScreenSize[0], ScreenSize[1]);
            worldSettings = new CLCalc.Program.Variable(GetWorldSettings(new Vector(3, 12, 15)));
            trianglesBuff = new CLCalc.Program.Variable(new float[64 * 1000]);

            ResourceTextures = new List<Material>(){
                new Material(new Texture(Resources.T_Brick_Clay_Old_D), new Texture(Resources.T_Brick_Clay_Old_N))
            };
        }


        public static void Clear()
        {
            clear.Execute(new CLCalc.Program.Variable[] { renderTexture, clearColor }, ScreenSize);
        }



        static CLCalc.Program.Variable transform;
        public static void RenderMesh(Mesh mesh, Transform t)
        {
            /*var transform = new CLCalc.Program.Variable(new float[] {
                0, 0, 4, 0,
                2.4f, 2.3f, 4.4f, 0,
                1, 1, 1, 1
            });*/

            if (transform == null)
                transform = new CLCalc.Program.Variable(new float[12]);
            transform.WriteToDevice(t.values);

            mesh.LoadBuffers();

            project.Execute(new CLCalc.Program.Variable[] { mesh.VBuffer, mesh.IBuffer, transform, worldSettings, trianglesBuff }, mesh.Indices.Length / 3);
            rasterize.Execute(new CLCalc.Program.MemoryObject[] { renderTexture, worldSettings, trianglesBuff, Texture0, Texture1 }, ScreenSize);
        }

        public static Bitmap Out()
        {
            finalize.Execute(new CLCalc.Program.MemoryObject[] { renderTexture, screen }, ScreenSize);

            var f = new float[trianglesBuff.OriginalVarLength];
            trianglesBuff.ReadFromDeviceTo(f);

            return screen.ReadBitmap();
        }

        static public int Width { get { return ScreenSize[0]; } }
        static public int Height { get { return ScreenSize[1]; } }


        static string ReadFilesOfType(string type)
        {
            if (type == "*.h")
                return string.Join("\r\n", Directory.GetFiles("renderer", type).Reverse().Select(f => File.ReadAllText(f))) + "\r\n\r\n";
            return string.Join("\r\n", Directory.GetFiles("renderer", type).Select(f => File.ReadAllText(f))) + "\r\n\r\n";
        }
    }
}
