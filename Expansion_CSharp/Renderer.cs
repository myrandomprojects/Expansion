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

        public static void Initialize()
        {
            CLCalc.InitCL();
            List<Cloo.ComputeDevice> L = CLCalc.CLDevices;
            CLCalc.Program.DefaultCQ = 0;


            CLCalc.Program.Compile(ReadFilesOfType("*.h") + ReadFilesOfType("*.cpp"), out List<string> logs);

            clear = new CLCalc.Program.Kernel("clear");
            project = new CLCalc.Program.Kernel("project");
            rasterize = new CLCalc.Program.Kernel("rasterize");
            finalize = new CLCalc.Program.Kernel("finalize");

            renderTexture = new CLCalc.Program.Variable(typeof(float), 2 * ScreenSize[0] * ScreenSize[1]);
            clearColor = new CLCalc.Program.Variable(new byte[4] { 100, 149, 237, 255 });
            screen = new CLCalc.Program.Image2D(ComputeImageChannelType.UnsignedInt8, ScreenSize[0], ScreenSize[1]);
            worldSettings = new CLCalc.Program.Variable(new float[] { 1, 1, 1, 0, ScreenSize[0], ScreenSize[1] });
            trianglesBuff = new CLCalc.Program.Variable(new float[64 * 40]);
        }


        public static void Clear()
        {
            clear.Execute(new CLCalc.Program.Variable[] { renderTexture, clearColor }, ScreenSize);
        }

        public static void RenderMesh(Mesh mesh)
        {
            var vertexValuesPlain = new List<float>();

            foreach (var vertex in mesh.Vertices)
                vertexValuesPlain.AddRange(vertex.values);

            var vertices = new CLCalc.Program.Variable(vertexValuesPlain.ToArray());
            var indices = new CLCalc.Program.Variable(mesh.Indices);
            var tCount = new CLCalc.Program.Variable(new int[] { mesh.Indices.Length / 3 });
            var transform = new CLCalc.Program.Variable(new float[] {
                0, 0, 4, 0,
                2.4f, 2.3f, 4.4f, 0,
                1, 1, 1, 1
            });

            project.Execute(new CLCalc.Program.Variable[] { vertices, indices, transform, worldSettings, trianglesBuff }, mesh.Indices.Length / 3);
            rasterize.Execute(new CLCalc.Program.Variable[] { renderTexture, worldSettings, trianglesBuff, tCount }, ScreenSize);
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
            return string.Join("\r\n", Directory.GetFiles("renderer", type).Select(f => File.ReadAllText(f))) + "\r\n\r\n";
        }
    }
}
