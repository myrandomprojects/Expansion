using Expansion.Engine.Classes.GameFramework;
using Expansion.Engine.Math3D;

namespace Expansion_CSharp.GameClasses
{
    internal class GameGame : Expansion.Engine.Game
    {
        public GameGame()
        {
            GameMode = new GameMode();

            var map = World.SpawnObject<MeshActor>(Transform.Identity);
            map.SetMesh("meshes/level.fbx");

            for(int i=0;i<4;i++)
            {
                var spawner = World.SpawnObject<PlayerStart>(new Transform(new Vector(-50+100*(i%2), 20, -50+100*(i/2)), Vector.Zero3, Vector.One3));
                //spawner.ViewTarget = Vector.Zero3;
            }
            

            AddPlayer();
            //StartGame();
        }
    }
}