using Expansion.Engine.Classes.GameFramework;

namespace Expansion_CSharp.GameClasses
{
    internal class GameMode : GameModeBase
    {
        public GameMode()
        {
            PlayerControllerClass = typeof(PlayerCharacter);
            PlayerCharacterClass = typeof(PlayerCharacter);
        }
    }
}