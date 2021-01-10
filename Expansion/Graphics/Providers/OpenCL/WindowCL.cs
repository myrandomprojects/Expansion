using Expansion.Graphics.Classes;
using Expansion.Graphics.Providers.IProvider;
using System.Windows.Forms;

namespace Expansion.Graphics.Providers.OpenCL
{

    internal class WindowCL : Window
    {
        internal Form form;

        public override bool IsOpen { get; protected set; } = true;

        public WindowCL(string title, int W, int H)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new Form
            {
                ClientSize = new System.Drawing.Size(W, H),
                Text = title
            };
            form.FormClosed += FormClosed;


            //form.Controls.Add(pb);
        }

        private void FormClosed(object sender, FormClosedEventArgs e)
        {
            IsOpen = false;
        }

        public override void Tick()
        {
            form.Refresh();
        }

        private void UpdateWidget(Widget widget)
        {
            if (widget is SceneView sw)
                (sw.ProviderTag as PictureBox).Image = (sw.RenderTarget as RenderTargetCL).screen.ReadBitmap();

            foreach (var child in widget.Children)
                UpdateWidget(child);
        }
    }
}