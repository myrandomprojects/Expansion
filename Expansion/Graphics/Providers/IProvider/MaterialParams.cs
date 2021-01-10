using Expansion.Engine.Math3D;
using System.Collections.Generic;
using System.Linq;

namespace Expansion.Graphics.Providers.IProvider
{
    public class MaterialParams : List<object>
    {
        public Matrix World { get { return this[0] as Matrix; } set { this[0] = value; } }
        public Matrix View { get { return this[1] as Matrix; } set { this[1] = value; } }
        public Vector LightDirection { get { return this[2] as Vector; } set { this[2] = value; } }
        public Vector ScreenSize { get { return this[3] as Vector; } set { this[3] = value; } }

        public MaterialParams()
        {
            this.Add(new Matrix());
            this.Add(new Matrix());
            this.Add(new Vector(0, 0, 0));
            this.Add(new Vector(0, 0));
        }

        internal float[] Raw()
        {
            return this.SelectMany(x => x is Vector v ? v.values : (x as Matrix).m).ToArray();
        }
    }
}