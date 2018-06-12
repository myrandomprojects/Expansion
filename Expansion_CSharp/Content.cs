using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace Expansion_CSharp
{
    internal class Content
    {
        static Dictionary<string, Texture> textures = new Dictionary<string, Texture>();
        static Dictionary<string, Material> materials;
        static Dictionary<string, Mesh> meshes = new Dictionary<string, Mesh>();

        static internal Texture LoadTexture(string name)
        {
            if (textures.ContainsKey(name))
                return textures[name];

            return textures[name] = new Texture((Bitmap)Array.Find(typeof(Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic), p => p.Name == name).GetValue(null));
        }

        static internal Material LoadMaterial(string name)
        {
            return materials[name];
        }
        
        internal static List<Material> LoadMaterials()
        {
            var props = typeof(Resources).GetProperties(BindingFlags.Static | BindingFlags.NonPublic).Where(p => p.PropertyType == typeof(string) && (p.GetValue(null) as string).StartsWith("#MATERIAL"));
            materials = props.ToDictionary(p => p.Name, p => new Material(p.Name, p.GetValue(null) as string));
            return materials.Select(k => k.Value).ToList();
        }
    }
}