using Expansion.Engine.Classes.Camera;
using Expansion.Engine.Math3D;
using Expansion.Graphics.Classes;
using Expansion.Graphics.Providers.IProvider.Resources;
using System;

namespace Expansion.Graphics.Providers.IProvider
{
    internal interface IRenderer
    {
        string Name { get; }

        Window CreateWindow(string name, int W, int H);
        object CreateWidget(Widget parent, Type type, Rectangle layout);
        RenderTarget CreateRenderTarget(int width, int height);
        //Material CreateMaterial(string code);
        MeshImpl GetMeshImpl(MeshResource mesh);
        
        void StartFrame(RenderTarget renderTarget);
        void FinishFrame();
        void SetMaterial(Material mat);
        //void SetShader(PixelShader ps);
        void RenderMesh(MeshImpl mesh, Transform transform, Camera camera);
    }
}
