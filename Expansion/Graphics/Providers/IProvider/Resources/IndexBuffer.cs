namespace Expansion.Graphics.Providers.IProvider.Resources
{
    internal class IndexBuffer
    {
        public int Stride { get; }
        public int Size { get; }
        public int Usage { get; }

        public IndexBuffer(int stride, int size, int usage)
        {
            Stride = stride;
            Size = size;
            Usage = usage;
        }
    }
}