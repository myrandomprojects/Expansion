using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Expansion.Engine.Classes.GameFramework;
using Expansion.Engine.Math3D;
using Expansion.Graphics.Classes;

namespace Expansion.Engine
{
    public class World : IScene
    {
        public List<GameObject> Objects { get; } = new List<GameObject>();
        public List<MeshComponent> Meshes => Objects.SelectMany(o => o.Components.OfType<MeshComponent>()).ToList();

        public GameObject SpawnObject(Type type, Transform location)
        {
            var obj = type.GetConstructor(null).Invoke(null) as GameObject;
            Debug.Assert(obj != null);

            obj.Transform = location;
            Objects.Add(obj);
            return obj;
        }

        public T SpawnObject<T>(Transform location) where T: GameObject
        {
            return SpawnObject(typeof(T), location) as T;
        }

        internal void Tick()
        {
            throw new NotImplementedException();
        }
    }
}