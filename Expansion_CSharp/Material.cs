namespace Expansion_CSharp
{
    internal class Material
    {
        private Texture BaseColor;
        private Texture Normal;

        public Material(Texture baseColor, Texture normal)
        {
            BaseColor = baseColor;
            Normal = normal;
        }

        public void Activate()
        {
            Renderer.Texture0 = BaseColor.GPUResource;
            Renderer.Texture1 = Normal.GPUResource;
        }
    }
}