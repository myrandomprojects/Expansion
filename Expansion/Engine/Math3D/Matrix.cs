namespace Expansion.Engine.Math3D
{
    public class Matrix
    {
        public float[] m;

        public Matrix()
        {
            m = new float[16];
        }

        public Matrix(params float[] m)
        {
            this.m = m;
        }

        static public Matrix Translate(Vector l)
        {
            return new Matrix(1, 0, 0, l.X,
                              0, 1, 0, l.Y,
                              0, 0, 1, l.Z,
                              0, 0, 0, 1);
        }

        static public Matrix Scale(Vector s)
        {
            return new Matrix(s.X, 0, 0, 0,
                              0, s.Y, 0, 0,
                              0, 0, s.Z, 0,
                              0, 0, 0, 1);
        }

        static public Matrix Zero { get; } = new Matrix();
        static public Matrix Identity { get; } = new Matrix(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
    }
}