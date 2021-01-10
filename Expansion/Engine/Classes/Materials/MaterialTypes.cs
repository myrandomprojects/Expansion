
namespace Expansion.Engine.Classes.Materials
{
    public class Float : IMaterialType
    {
        public IMaterialValue<Float> Source { get; }

        public Float(IMaterialValue<Float> src) { Source = src; }

        public static implicit operator Float(float x) => new Float(new MaterialConstant<Float>(x));
    }


    public class Float2 : IMaterialType
    {
        public IMaterialValue<Float2> Source { get; }

        public Float X { get; set; }
        public Float Y { get; set; }

        public Float2(IMaterialValue<Float2> src) { Source = src; }

        static public Float2 operator *(Float2 vec, Float2 mat) => new Float2(new Expression<Float2>("*", vec, mat));
    }

    public class Float3 : IMaterialType
    {
        public IMaterialValue<Float3> Source { get; }

        public Float X { get; set; }
        public Float Y { get; set; }
        public Float Z { get; set; }

        public Float3(IMaterialValue<Float3> src) { Source = src; }

        static public Float3 operator +(Float3 a, Float3 b) => new Float3(new Expression<Float3>("+", a, b));
        static public Float3 operator *(Float3 a, Float3 b) => new Float3(new Expression<Float3>("*", a, b));
        static public Float3 operator *(Float3 a, Float x) => new Float3(new Expression<Float3>("*", a, x));

        static public Float3 operator *(Float3 vec, Float3x3 mat) => new Float3(new Expression<Float3>("*", vec, mat));
        static public Float3 operator *(Float3 vec, Float4x4 mat) => new Float3(new Expression<Float3>("*", vec, mat));
    }

    public class Float4 : IMaterialType
    {
        public IMaterialValue<Float4> Source { get; }

        public Float X { get; set; }
        public Float Y { get; set; }
        public Float Z { get; set; }
        public Float W { get; set; }

        public Float4(IMaterialValue<Float4> src) { Source = src; }
        public Float4(Float x, Float y, Float z, Float w) { X = x;Y = y;Z = z;W = w; }

        static public Float4 operator *(Float4 vec, Float4x4 mat) => new Float4(new Expression<Float4>("*", vec, mat));
    }

    public class Float3x3 : IMaterialType
    {
        public IMaterialValue<Float3x3> Source { get; }
    }

    public class Float4x4 : IMaterialType
    {
        public IMaterialValue<Float4x4> Source { get; }

        public Float4x4(IMaterialValue<Float4x4> src) { Source = src; }

        static public Float4x4 operator *(Float4x4 a, Float4x4 b) => new Float4x4(new Expression<Float4x4>("*", a, b));
    }
}