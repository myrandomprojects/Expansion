using System;
using System.Collections.Generic;
using System.Text;

namespace Expansion.Engine.Math3D
{
    public class Transform
    {
        public Vector Position { get; set; }
        public Vector Rotation { get; set; }
        public Vector Scale { get; set; }

        public static Transform Identity { get; } = new Transform(Vector.Zero3, Vector.Zero3, Vector.One3);

        public Transform(Vector p, Vector r, Vector s)
        {
            Position = p;
            Rotation = r;
            Scale = s;
        }
    }
}
