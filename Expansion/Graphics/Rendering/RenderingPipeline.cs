using Expansion.Graphics.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Expansion.Graphics.Rendering
{
    interface IRenderingPipeline
    {
        
        void Render(SceneView scene);
    }
}
