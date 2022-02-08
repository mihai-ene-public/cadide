using System;
using System.Windows.Media.Media3D;
using STPLoader.Implementation.Parser;
using System.Collections.Generic;
using System.Linq;

namespace STPLoader.Implementation.Model.Entity
{
    public class CartesianPoint : Entity
    {
        public Vector3D Vector;
        public string Info;

        public override void Init()
        {
            Info = ParseHelper.ParseString(Data[0]);

            Vector = new Vector3D(double.Parse(Data[1].Trim()), double.Parse(Data[2].Trim()), double.Parse(Data[3].Trim()));
        }

        public override string ToString()
        {
            return String.Format("<CartesianPoint({0}, {1})", Info, Vector);
        }
    }
}
