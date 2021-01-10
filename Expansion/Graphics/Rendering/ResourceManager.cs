using Expansion.Graphics.Providers.IProvider.Resources;
using System.Collections.Generic;
using System.IO;

namespace Expansion.Graphics.Rendering
{
    public class ResourceManager
    {
        static Dictionary<string, GPUResource> resources = new Dictionary<string, GPUResource>();

        static public GPUResource LoadResource(string path)
        {
            if (resources.ContainsKey(path))
                return resources[path];

            GPUResource res = null;

            if (Path.GetExtension(path) == ".fbx")
                res = new MeshResource(path);
            else if (Path.GetExtension(path) == ".png" || Path.GetExtension(path) == ".BMP")
                res = new TextureResource(path);

            resources[path] = res;

            return res;
        }

        static public TextureResource LoadTexture(string path)
        {
            if (File.Exists(path))
                return LoadResource(path) as TextureResource;

            if (File.Exists(path + ".png"))
                return LoadResource(path + ".png") as TextureResource;
            if (File.Exists(path + ".BMP"))
                return LoadResource(path + ".BMP") as TextureResource;

            return null;
        }
    }
}
