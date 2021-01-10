namespace Expansion.Engine.Math3D
{
    public class Vertex
    {
        public float[] values = new float[16];
        int tangentCount = 0;
        
        public Vertex(Vector pos, Vector normal, Vector tex)
        {
            Position = pos;
            Normal = normal;
            TexUV = tex;
        }

        public Vector Position { get { return new Vector(values[0], values[1], values[2]); } set { values[0] = value.values[0]; values[1] = value.values[1]; values[2] = value.values[2]; } }
        public Vector Normal { get { return new Vector(values[4], values[5], values[6]); } set { values[4] = value.values[0]; values[5] = value.values[1]; values[6] = value.values[2]; } }
        public Vector TexUV { get { return new Vector(values[8], values[9]); } set { values[8] = value.values[0]; values[9] = value.values[1]; } }
        public Vector Tangent { get { return new Vector(values[10], values[11], values[12]); } set { values[10] = value.values[0]; values[11] = value.values[1]; values[12] = value.values[2]; } }
        public Vector Binormal { get { return new Vector(values[13], values[14], values[15]); } set { values[13] = value.values[0]; values[14] = value.values[1]; values[15] = value.values[2]; } }

    }
}