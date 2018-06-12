using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expansion_CSharp
{
    class World
    {
        List<Actor> objects;

        World()
        {
            objects = new List<Actor>();
        }

        public void Spawn(Actor newObject, Transform transform)
        {
            newObject.Transform = transform;
            objects.Add(newObject);
        }

        public void Tick()
        {
            var r = objects.Last().Transform.Rotation;
            r.values[0] += 0.01f;
            objects.Last().Transform.Rotation = r;

            Renderer.Clear();
            foreach(var obj in objects)
            {
                Renderer.RenderMesh(obj.Mesh, obj.Material, obj.Transform);
            }
        }

        static public World Current { get; private set; } = new World();
    }
}
