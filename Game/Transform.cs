namespace Expansion_CSharp
{
    internal class Transform1
    {
        public float[] values = new float[12];

        public Vector1 Position { get { return new Vector1(values[0], values[1], values[2]); } set { values[0] = value.values[0]; values[1] = value.values[1]; values[2] = value.values[2]; } }
        public Vector1 Rotation { get { return new Vector1(values[4], values[5], values[6]); } set { values[4] = value.values[0]; values[5] = value.values[1]; values[6] = value.values[2]; } }
        public Vector1 Scale { get { return new Vector1(values[8], values[9], values[10]); } set { values[8] = value.values[0]; values[9] = value.values[1]; values[10] = value.values[2]; } }

        public Transform1(Vector1 p, Vector1 r, Vector1 s)
        {
            Position = p;
            Rotation = r;
            Scale = s;
        }
    }
}