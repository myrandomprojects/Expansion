using OpenCLTemplate;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Expansion_CSharp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            /*
            var vertices = new CLCalc.Program.Variable(new float[] {
                -1.08f, 0.92f, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                -0.6f, -1, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                1, 1, 2.4f, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            });

            var indices = new CLCalc.Program.Variable(new int[] { 0, 1, 2 });
            */

            Renderer.Initialize();

            Renderer.Clear();

            Renderer.RenderMesh(Mesh.Cube);

            var bmp = Renderer.Out();

            pictureBox1.Location = Point.Empty;
            pictureBox1.Size = bmp.Size;

            ClientSize = pictureBox1.Size;

            pictureBox1.Image = bmp;

        }
    }
}
