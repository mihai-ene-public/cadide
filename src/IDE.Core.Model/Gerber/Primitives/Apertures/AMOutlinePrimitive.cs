using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Core.Model.Gerber.Primitives.Apertures;

public class AMOutlinePrimitive : ApertureMacroPrimitive
{
    public AMOutlinePrimitive()
    {
        Code = 4;

        throw new NotImplementedException();
    }

    public override void WriteTo(TextWriter writer)
    {
        //writer.WriteLine($"0 {Comment}*");
    }
}



