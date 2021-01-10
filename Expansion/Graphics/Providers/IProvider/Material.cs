using Expansion.Graphics.Providers.IProvider.Resources;
using Expansion.Graphics.Rendering;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Expansion.Graphics.Providers.IProvider
{
    public partial class Material
    {
        public string Name { get; }
        public string Code { get; }

        public TextureResource[] MatTextures { get; internal set; }

        public MaterialParams Params { get; } = new MaterialParams();

        internal Material(string name, string code)
        {
            Name = name;
            Code = code;

            var textures = new List<TextureResource>();
            var names = new List<string>();
            foreach (Match match in Regex.Matches(code, @"sample[A-Za-z]*\(([A-Za-z0-9_]+)\)"))
            {
                string val = match.Groups[1].Value;
                if (names.Contains(val))
                    continue;

                textures.Add(ResourceManager.LoadTexture(val));
                names.Add(val);
            }

        }
    }

    internal class MatImpl
    {

    }
}