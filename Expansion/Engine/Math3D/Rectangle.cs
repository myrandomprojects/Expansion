namespace Expansion.Engine.Math3D
{
    public class Rectangle
    {
        public float Left { get; set; }
        public float Top { get; set; }
        public float Right { get; set; }
        public float Bottom { get; set; }

        public float Width { get { return Right - Left; } }
        public float Height { get { return Bottom - Top; } }

        public Vector Center { get { return new Vector(Left + Width / 2, Top + Height / 2); } }
    }
}