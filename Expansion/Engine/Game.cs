using Expansion.Engine.Classes.GameFramework;
using Expansion.Graphics.Classes;
using Expansion.Graphics.Providers.IProvider;
using System.Collections.Generic;
using System.Linq;

namespace Expansion.Engine
{
    public class Game
    {
        public List<Window> Windows { get; private set; } = new List<Window>();
        public List<PlayerController> Players { get; } = new List<PlayerController>();

        public World World { get; } = new World();

        public GameModeBase GameMode { get; set; }

        public Game()
        {

        }

        public void AddPlayer()
        {
            var pc = GameMode.CreatePlayerController();
            pc.Game = this;
            Players.Add(pc);

            var spawner = RNG.Choice(World.Objects.OfType<PlayerStart>());
            var character = World.SpawnObject(GameMode.PlayerCharacterClass, spawner.Transform);
            pc.Possess(character);
        }

        public void UpdateWorld()
        {

        }

        /*
        public void LoadLevel(Type type)
        {
            var level = type.GetConstructor(new Type[0]).Invoke(null) as Level;


        }*/

        public void Run()
        {
            while (Windows.Count > 0)
            {
                foreach (var pc in Players)
                    pc.Tick();

                World.Tick();

                foreach (var window in Windows)
                    window.Tick();

                System.Windows.Forms.Application.DoEvents();

                Windows = Windows.Where(w => w.IsOpen).ToList();
            }
        }
    }
}
