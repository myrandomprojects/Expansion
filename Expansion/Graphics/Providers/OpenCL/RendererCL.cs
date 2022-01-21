using Expansion.Engine.Classes.Camera;
using Expansion.Engine.Math3D;
using Expansion.Graphics.Classes;
using Expansion.Graphics.Providers.IProvider;
using Expansion.Graphics.Providers.IProvider.Resources;
using System;
using System.Windows.Forms;
using static OpenCLTemplate.CLCalc.Program;

namespace Expansion.Graphics.Providers.OpenCL
{
    class RendererCL : IRenderer
    {
        static public RendererCL Instance { get; private set; }

        int[] screenSize = new int[2];
        RenderTargetCL currentScreen;

        //Kernel finalize;

        public static string StaticName { get { return "OpenCL"; } }
        public string Name { get { return "OpenCL"; } }

        public Window CreateWindow(string title, int W, int H) => new WindowCL(title, W, H);
        public RenderTarget CreateRenderTarget(int W, int H) => new RenderTargetCL(W, H);

        public RendererCL()
        {
            Instance = this;
        }

        public object CreateWidget(Widget parent, Type type, Rectangle layout)
        {
            var widget = type.GetConstructor(null).Invoke(null) as Widget;
            widget.Parent = parent;

            if (widget is SceneView sw)
            {
                sw.RenderTarget = CreateRenderTarget((int)layout.Width, (int)layout.Height);

                var pb = new PictureBox();
                pb.Left = (int)layout.Left;
                pb.Top = (int)layout.Left;
                pb.Width = (int)layout.Width;
                pb.Height = (int)layout.Height;

                widget.ProviderTag = pb;

                if (parent is WindowCL window)
                    window.form.Controls.Add(pb);
            }

            return widget;
        }

        public Material CreateMaterial(string code)
        {
            throw new NotImplementedException();
        }

        public MeshImpl GetMeshImpl(MeshResource mesh)
        {
            if (mesh.meshCL == null)
                mesh.meshCL = new MeshImpl(new VertexBufferCL(mesh.vertices), new IndexBufferCL(mesh.indices));

            return mesh.meshCL;
        }

        public void StartFrame(RenderTarget renderTarget)
        {
            currentScreen = renderTarget as RenderTargetCL;
            screenSize[0] = renderTarget.Width;
            screenSize[1] = renderTarget.Height;
        }

        public void FinishFrame()
        {
            //finalize.Execute(new MemoryObject[] { currentScreen., screen }, screenSize);

        }

        public void SetMaterial(Material mat)
        {
            throw new NotImplementedException();
        }

        public void RenderMesh(MeshResource mesh, Transform transform, Camera camera)
        {
            throw new NotImplementedException();
        }

        public void RenderMesh(MeshImpl mesh, Transform transform, Camera camera)
        {
            throw new NotImplementedException();
        }
    }
}
