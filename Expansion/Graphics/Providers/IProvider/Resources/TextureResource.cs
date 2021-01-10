using System.IO;

namespace Expansion.Graphics.Providers.IProvider.Resources
{
    public partial class TextureResource : GPUResource
    {
        public string Name { get; }
        public string TexturePath { get; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        internal TextureResource(string path)
        {
            TexturePath = path;
            Name = Path.GetFileNameWithoutExtension(path);

        }
    }
}