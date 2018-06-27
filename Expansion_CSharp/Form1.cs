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

            //Mesh SM_Rock_Chunk = Mesh.CreateFromResource(Resources.SM_Rock_Chunk);

            Renderer.Initialize();

            /*
            World.Current.Spawn(
                new MeshActor(Mesh.Plane, Content.LoadMaterial("MinionMaterial")),
                new Transform(
                    new Vector(0, 0, 1),
                    new Vector(0, 0, 0),
                    new Vector(1, 1, 1)
                )
            );
            */

            World.Current.Spawn(
                new MeshActor(Mesh.SM_Rock_Chunk, Content.LoadMaterial("RockMaterial")),
                new Transform(
                    new Vector(0.3f, 0, 7.1f),
                    new Vector(2.4f, 2.3f, 4.4f),
                    new Vector(0.01f, 0.01f, 0.01f)
                )
            );
            /*
            World.Current.Spawn(
                new MeshActor(Mesh.Buff_White, Content.LoadMaterial("MinionMaterial")),
                new Transform(
                    new Vector(0.2f, 0.3f, 3),
                    new Vector(1, 0, 0),
                    new Vector(0.01f, 0.01f, 0.01f)
                )
            );
            */


            World.Current.Tick();

            var bmp = Renderer.Out();

            pictureBox1.Location = Point.Empty;
            pictureBox1.Size = bmp.Size;

            ClientSize = pictureBox1.Size;

            pictureBox1.Image = bmp;

            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            World.Current.Tick();

            var bmp = Renderer.Out();

            pictureBox1.Image = bmp;

            timer1.Start();
        }
    }
}
