using Expansion.Engine.Math3D;
using Expansion.Graphics.Providers.IProvider.Resources;
using Expansion.Graphics.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Expansion.Engine.Classes.Materials
{
    public interface IMaterialType { }

    public abstract class IMaterialValue<T> { }
    public class Expression<T> : IMaterialValue<T>
        //where T: new(IMaterialValue<T>)
    {
        public string Name { get; }
        public object[] Args { get; }

        public Expression(string name, params object[] args)
        {
            Name = name;
            Args = args;
        }

        //public T Call(string name, params IMaterialType[] args) => new T(new Expression(name, args));
    }

    public class MaterialParam<T> : IMaterialValue<T>
    {
    }
    public class MaterialConstant<T> : IMaterialValue<T>
    {
        public T Value { get; }

        public MaterialConstant(T val)
        {
            Value = val;
        }
    }
    public class VertexParam<T> : IMaterialValue<T>
    {
        public VertexParamType Type { get; }
        public string Name { get; }

        public VertexParam(VertexParamType type)
        {
            Type = type;
        }
    }

    public enum VertexParamType
    {
        Position,
        Normal,
        Tangent,
        Binormal,
        TexUV,
        Color
    }
    public class VertexDeclaration
    {
        public VertexParamType[] Params { get; }

        public VertexDeclaration(params VertexParamType[] par)
        {
            Params = par;
        }

        //VertexParam<Vector> FindParam(VertexParamType type){return new }

        public Float3 Position { get { return new Float3(new VertexParam<Float3>(VertexParamType.Position)); } }
        public Float3 Normal { get { return new Float3(new VertexParam<Float3>(VertexParamType.Normal)); } }
        public Float3 Tangent { get { return new Float3(new VertexParam<Float3>(VertexParamType.Tangent)); } }
        public Float3 Binormal { get { return new Float3(new VertexParam<Float3>(VertexParamType.Binormal)); } }
        public Float2 TexUV { get { return new Float2(new VertexParam<Float2>(VertexParamType.TexUV)); } }
    }

    public abstract partial class VertexShaderBase
    {
    }

    public abstract class PixelShaderBase
    {
        public Float4 out_color;
    }




    public class DefaultVS : VertexShaderBase
    {
        public Float4x4 WorldTransform { get; } = new Float4x4(new MaterialParam<Float4x4>());
        public Float4x4 ViewTransform { get; } = new Float4x4(new MaterialParam<Float4x4>());

        public readonly Float3 in_position = new Float3(new VertexParam<Float3>(VertexParamType.Position));
        public readonly Float3 in_normal = new Float3(new VertexParam<Float3>(VertexParamType.Normal));
        public readonly Float3 in_tangent = new Float3(new VertexParam<Float3>(VertexParamType.Tangent));
        public readonly Float3 in_binormal = new Float3(new VertexParam<Float3>(VertexParamType.Binormal));
        public readonly Float2 in_texUV = new Float2(new VertexParam<Float2>(VertexParamType.TexUV));

        public Float3 out_position;
        public Float3 out_normal;
        public Float3 out_tangent;
        public Float3 out_binormal;
        public Float2 out_texUV;

        public DefaultVS()
        {
            var worldView = WorldTransform * ViewTransform;

            out_position = in_position * worldView;
            out_normal = in_normal * worldView;
            out_tangent = in_tangent * worldView;
            out_binormal = in_binormal * worldView;
            out_texUV = in_texUV;
        }
    }

    public class DefaultPS : PixelShaderBase
    {
        public Float3 in_position;
        public Float3 in_normal;
        public Float3 in_tangent;
        public Float3 in_binormal;
        public Float2 in_texUV;

        public DefaultPS()
        {
        }
    }

    public abstract class MaterialBase
    {
        public Float4 BaseColor { get; protected set; }
        public Float3 Normal { get; protected set; }

        public Float2 TexUV { get; } = new Float2(new VertexParam<Float2>(VertexParamType.TexUV));


        public abstract void MatMain();
        protected MaterialBase()
        {
            MatMain();

            //var normal = PS.in_normal * Normal.Z + PS.in_tangent * Normal.X + PS.in_binormal * Normal.Y;

            //PS.out_color = 
        }

        public DefaultVS VS = new DefaultVS();
        public DefaultPS PS = new DefaultPS();



        public List<GPUResource> Resources { get; } = new List<GPUResource>();





        protected T Call<T>(string name, params object[] args)
            where T : class
        {
            var expr = new Expression<Float4>(name, args);

            var constr = typeof(T).GetConstructor(new Type[] { typeof(Expression<Float4>) });
            var obj = constr.Invoke(new object[] { expr }) as T;
            return obj;
        }

        protected GPUResource GetResource(string name) => ResourceManager.LoadResource(name);
        protected Float4 Sample(string texName, Float2 uv) => Call<Float4>("sample", ResourceManager.LoadResource(texName), uv);
    }






}