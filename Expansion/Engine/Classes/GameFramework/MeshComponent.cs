using Expansion.Graphics.Providers.IProvider;
using Expansion.Graphics.Providers.IProvider.Resources;
using Expansion.Graphics.Rendering;

namespace Expansion.Engine.Classes.GameFramework
{
    public class MeshComponent : Component
    {
        internal MeshResource Mesh { get; private set; }
        internal Material Material { get; private set; }

        public MeshComponent()
        {

        }

        public void SetMesh(string path)
        {
            Mesh = ResourceManager.LoadResource(path) as MeshResource;
        }
    }
}