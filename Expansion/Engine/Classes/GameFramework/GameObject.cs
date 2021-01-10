using Expansion.Engine.Math3D;
using System.Collections.Generic;

namespace Expansion.Engine.Classes.GameFramework
{
    public class GameObject
    {
        public Transform Transform { get; set; }
        public List<Component> Components { get; } = new List<Component>();

    }
}
