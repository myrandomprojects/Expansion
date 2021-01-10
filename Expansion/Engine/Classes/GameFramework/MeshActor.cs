namespace Expansion.Engine.Classes.GameFramework
{
    public class MeshActor : GameObject
    {
        public MeshComponent Mesh { get; }
        public MeshActor()
        {
            Mesh = new MeshComponent();
            Components.Add(Mesh);
        }

        public void SetMesh(string path)
        {
            Mesh.SetMesh(path);
        }
    }
}