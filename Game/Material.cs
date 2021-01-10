using OpenCLTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static OpenCLTemplate.CLCalc.Program;

namespace Expansion_CSharp
{
    internal class Material
    {
        private Texture[] textures;

        public string Name { get; private set; }
        public string Code { get; private set; }

        public Kernel Kernel { get; set; }

        public Material(string name, string matBody)
        {
            Name = name;

            var textures = new List<Texture>();
            var names = new List<string>();
            foreach (Match match in Regex.Matches(matBody, @"sample[A-Za-z]*\(([A-Za-z0-9_]+)\)"))
            {
                string val = match.Groups[1].Value;
                if (names.Contains(val))
                    continue;

                textures.Add(Content.LoadTexture(val));
                names.Add(val);
            }

            Code = "MAT_FUNC Material_" + name + "(MAT_ARGS()" + string.Join("", names.Select(n => ", read_only image2d_t " + n)) + ") "+
                "{ \r\n    MAT_HEADER() \r\n//" + matBody + "MAT_FOOTER() }";

            /*Code = Resources.MaterialBase
                .Replace("$NAME$", name)
                .Replace("$ARGS$", string.Join("", names.Select(n => ", read_only image2d_t " + n)))
                .Replace("$BODY$");
            */
            this.textures = textures.ToArray();
        }

        public MemoryObject[] MatArgs()
        {
            var objs = new MemoryObject[textures.Length];
            int i = 0;
            foreach (var t in textures)
                objs[i++] = t.GPUResource;
            return objs;
        }

        internal void Rasterize(Variable renderTexture, Variable worldSettings, Variable projVerticesBuff, Variable batches, int[] screenSize)
        {
            /*
            var f = new float[projVerticesBuff.OriginalVarLength];
            projVerticesBuff.ReadFromDeviceTo(f);
            string a = "";
            for (int i = 0; i < 64; i++)
                a += f[128 + i] + ", ";
            */

            Kernel.Execute(
                new MemoryObject[] { renderTexture, worldSettings, projVerticesBuff, batches }.Concat(textures.Select(t => t.GPUResource)).ToArray(),
                screenSize,
                new int[] { 16, 16 }
            );
        }
    }
}


/*
 
	BaseColor = 
		Add(
			Multiply(
				Multiply(
					Lerp(
						COLOR(0.6, 0.5, 0.2),
						COLOR(1.06, 3.265, 4),
						sample(T_Water_Screen)
					),
					sample(T_CliffRock)
				),
				1
			),

		)
*/