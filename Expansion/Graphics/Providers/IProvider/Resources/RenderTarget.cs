namespace Expansion.Graphics.Providers.IProvider.Resources
{
    public class RenderTarget
    {
        internal IRenderer Renderer { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
    }
}