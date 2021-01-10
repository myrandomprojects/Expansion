using Expansion.Graphics.Classes;

namespace Expansion.Graphics.Rendering
{
    public class DefaultRenderer : IRenderingPipeline
    {
        static IRenderingPipeline Instance { get; } = new DefaultRenderer();

        private DefaultRenderer() { }

        public void Render(SceneView scene)
        {
            var meshes = scene.Scene.Meshes;
            var renderer = scene.RenderTarget.Renderer;

            renderer.StartFrame(scene.RenderTarget);
            foreach(var mesh in meshes)
            {

            }
            renderer.FinishFrame();
        }
    }
}