using Expansion.Engine.Classes.Camera;
using Expansion.Graphics.Providers.IProvider.Resources;

namespace Expansion.Graphics.Classes
{
    public partial class SceneView : Widget
    {
        public IScene Scene { get; set; }
        public Camera Camera { get; set; }
        public RenderTarget RenderTarget { get; set; }

        public override void SetLayout(int x, int y, int width, int height)
        {
            base.SetLayout(x, y, width, height);

            RenderTarget = Renderer.CreateRenderTarget(width, height);
        }
    }
}
