using Expansion.Engine.Classes.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Expansion_CSharp.GameClasses.Materials
{
    class RockMaterial : MaterialBase
    {
        public override void Implement()
        {
            BaseColor = Sample("T_Brick_Clay_Old_D.BMP", TexUV);
            Normal = Call<Float3>(".", Sample("T_Brick_Clay_Old_N.BMP", TexUV), "xyz");
        }
    }
}
