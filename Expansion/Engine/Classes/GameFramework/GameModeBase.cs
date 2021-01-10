using System;

namespace Expansion.Engine.Classes.GameFramework
{
    public class GameModeBase
    {
        public Type PlayerControllerClass { get; protected set; } = typeof(PlayerController);
        public Type PlayerCharacterClass { get; protected set; }
        
        public GameModeBase() { }

        public PlayerController CreatePlayerController()
        {
            return PlayerControllerClass.GetConstructor(null).Invoke(null) as PlayerController;
        }

        public GameObject CreatePlayerCharacter()
        {
            return PlayerCharacterClass.GetConstructor(null).Invoke(null) as GameObject;
        }
    }
}
