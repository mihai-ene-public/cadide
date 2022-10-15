using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using IDE.Core.Types.Media;
using IDE.Core.Types.Media3D;
using System.Globalization;

namespace IDE.Core.Storage;

/// <summary>
/// this is a solid body mesh. It is usually imported from external files (.stl, etc)
/// </summary>
public class Mesh3DItem : MeshPrimitive
{
    public Mesh3DItem()
    {

    }

    [XmlAttribute("centerX")]
    public double CenterX { get; set; }

    [XmlAttribute("centerY")]
    public double CenterY { get; set; }

    [XmlAttribute("centerZ")]
    public double CenterZ { get; set; }

    [XmlAttribute("rotationX")]
    public double RotationX { get; set; }

    [XmlAttribute("rotationY")]
    public double RotationY { get; set; }

    [XmlAttribute("rotationZ")]
    public double RotationZ { get; set; }


    //[XmlElement("model")]
    //public XmlCDataSection ModelCData
    //{
    //    get;set;
    //}

    [XmlElement("meshGeometry")]
    public MeshGeometryData MeshGeometry { get; set; }
}

public class MeshGeometryData
{
    [XmlAttribute("Positions")]
    public string PositionsString
    {
        get
        {
            if (Positions != null && Positions.Length > 0)
            {
                var str = string.Join(' ',
                    Positions.Select(p => $"{p.X.ToString(CultureInfo.InvariantCulture)},{p.Y.ToString(CultureInfo.InvariantCulture)},{p.Z.ToString(CultureInfo.InvariantCulture)}"));

                return str;
            }

            return string.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                var pointsStrings = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Positions = new XPoint3D[pointsStrings.Length];
                for (int i = 0; i < pointsStrings.Length; i++)
                {
                    var coords = pointsStrings[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var x = double.Parse(coords[0], CultureInfo.InvariantCulture);
                    var y = double.Parse(coords[1], CultureInfo.InvariantCulture);
                    var z = double.Parse(coords[2], CultureInfo.InvariantCulture);

                    Positions[i] = new XPoint3D(x, y, z);
                }
            }
        }
    }

    [XmlAttribute("Normals")]
    public string NormalsString
    {
        get
        {
            if (Normals != null && Normals.Length > 0)
            {
                var str = string.Join(' ',
                    Normals.Select(p => $"{p.X.ToString(CultureInfo.InvariantCulture)},{p.Y.ToString(CultureInfo.InvariantCulture)},{p.Z.ToString(CultureInfo.InvariantCulture)}"));

                return str;
            }

            return string.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                var pointsStrings = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Normals = new XVector3D[pointsStrings.Length];
                for (int i = 0; i < pointsStrings.Length; i++)
                {
                    var coords = pointsStrings[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var x = double.Parse(coords[0], CultureInfo.InvariantCulture);
                    var y = double.Parse(coords[1], CultureInfo.InvariantCulture);
                    var z = double.Parse(coords[2], CultureInfo.InvariantCulture);

                    Normals[i] = new XVector3D(x, y, z);
                }
            }
        }
    }

    [XmlAttribute("TextureCoordinates")]
    public string TextureCoordinatesString
    {
        get
        {
            if (TextureCoordinates != null && TextureCoordinates.Length > 0)
            {
                var str = string.Join(' ',
                    TextureCoordinates.Select(p => $"{p.X.ToString(CultureInfo.InvariantCulture)},{p.Y.ToString(CultureInfo.InvariantCulture)}"));

                return str;
            }

            return string.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                var pointsStrings = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                TextureCoordinates = new XPoint[pointsStrings.Length];
                for (int i = 0; i < pointsStrings.Length; i++)
                {
                    var coords = pointsStrings[i].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var x = double.Parse(coords[0], CultureInfo.InvariantCulture);
                    var y = double.Parse(coords[1], CultureInfo.InvariantCulture);

                    TextureCoordinates[i] = new XPoint(x, y);
                }
            }
        }
    }

    [XmlAttribute("TriangleIndices")]
    public string TriangleIndicesString
    {
        get
        {
            if (Indices != null && Indices.Length > 0)
            {
                var str = string.Join(' ',
                    Indices.Select(p => p.ToString()));

                return str;
            }

            return string.Empty;
        }
        set
        {
            if (!string.IsNullOrEmpty(value))
            {
                var pointsStrings = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                Indices = new int[pointsStrings.Length];
                for (int i = 0; i < pointsStrings.Length; i++)
                {
                    var p = int.Parse(pointsStrings[i], CultureInfo.InvariantCulture);

                    Indices[i] = p;
                }
            }
        }
    }

    [XmlIgnore]
    public int[] Indices { get; set; }

    [XmlIgnore]
    public XPoint3D[] Positions { get; set; }

    [XmlIgnore]
    public XVector3D[] Normals { get; set; }

    [XmlIgnore]
    public XPoint[] TextureCoordinates { get; set; }
}
