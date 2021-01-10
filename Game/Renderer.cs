﻿using Cloo;
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
        static CLCalc.Program.Kernel bin;
        static CLCalc.Program.Kernel finalize;

        static CLCalc.Program.Variable
            renderTexture,
            clearColor,
            worldSettings,
            projVerticesBuff,
            trianglesBuff,
            triBoundsBuff,
            batchBuff;
        static CLCalc.Program.Image2D screen;

        static public CLCalc.Program.Image2D Texture0 { get; set; }
        static public CLCalc.Program.Image2D Texture1 { get; set; }

        static public List<Material> ResourceTextures { get; private set; }

        private static float[] GetWorldSettings(Vector1 lightdir)
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

            var mats = Content.LoadMaterials();

            string code = GPURenderer.types
                + GPURenderer.matrix
                + GPURenderer.clear
                + GPURenderer.project
                + GPURenderer.bin
                + GPURenderer.rasterize
                + GPURenderer.finalize
                + string.Join("\r\n\r\n\r\n\r\n\r\n", mats.Select(mat => mat.Code));
            CLCalc.Program.Compile(code, out List<string> logs);

            clear = new CLCalc.Program.Kernel("clear");
            project = new CLCalc.Program.Kernel("project");
            bin = new CLCalc.Program.Kernel("bin");
            finalize = new CLCalc.Program.Kernel("finalize");

            renderTexture = new CLCalc.Program.Variable(typeof(float), 2 * ScreenSize[0] * ScreenSize[1]);
            clearColor = new CLCalc.Program.Variable(new byte[4] { 100, 149, 237, 255 });
            screen = new CLCalc.Program.Image2D(ComputeImageChannelType.UnsignedInt8, ScreenSize[0], ScreenSize[1]);
            worldSettings = new CLCalc.Program.Variable(GetWorldSettings(new Vector1(3, -12, 2)));
            projVerticesBuff = new CLCalc.Program.Variable(new float[16 * 100000]);
            trianglesBuff = new CLCalc.Program.Variable(new float[64 * 10000]);
            triBoundsBuff = new CLCalc.Program.Variable(new float[4 * 10000]);
            batchBuff = new CLCalc.Program.Variable(new byte[1000 * 1000]);

            foreach (var m in mats)
                m.Kernel = new CLCalc.Program.Kernel("Material_" + m.Name);
            //ResourceTextures = new List<Material>(){ new Material(new Texture(Resources.T_Brick_Clay_Old_D), new Texture(Resources.T_Brick_Clay_Old_N)) };
        }


        public static void Clear()
        {
            clear.Execute(new CLCalc.Program.Variable[] { renderTexture, clearColor }, ScreenSize);
        }



        static CLCalc.Program.Variable transform;
        public static void RenderMesh(Mesh mesh, Material mat, Transform t)
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

            project.Execute(new CLCalc.Program.Variable[] {
                mesh.VBuffer,
                mesh.IBuffer,
                transform,
                worldSettings,
                projVerticesBuff,
                triBoundsBuff
            }, new int[] { mesh.Indices.Length / 3 });

            bin.Execute(new CLCalc.Program.Variable[] {
                triBoundsBuff,
                worldSettings,
                batchBuff
            }, new int[] { (mesh.Indices.Length / 3 + 254) / 255, 256 }, new int[] { 1, 256 });

            mat.Rasterize(renderTexture, worldSettings, projVerticesBuff, batchBuff, ScreenSize);
        }

        public static Bitmap Out()
        {
            finalize.Execute(new CLCalc.Program.MemoryObject[] { renderTexture, screen }, ScreenSize);

            //var f = new float[trianglesBuff.OriginalVarLength];
            //trianglesBuff.ReadFromDeviceTo(f);

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
