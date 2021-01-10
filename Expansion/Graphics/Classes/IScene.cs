using Expansion.Engine.Classes.GameFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expansion.Graphics.Classes
{
    public interface IScene
    {
        List<MeshComponent> Meshes { get; }
    }
}
