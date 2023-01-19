using IDE.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IDE.Core.Storage;

/// <summary>
/// a reference to a library
/// </summary>
public class LibraryProjectReference : ProjectDocumentReference
{
    /// <summary>
    /// the name of the library 
    /// </summary>
    [XmlAttribute("libraryName")]
    public string LibraryName { get; set; }

    ///// <summary>
    ///// the path to search for. If not found, search in other dedicated paths.
    ///// </summary>
    //[XmlAttribute("hintPath")]
    //public string HintPath { get; set; }

    [XmlAttribute("version")]
    public string Version { get; set; }

    public override string ToString()
    {
        var v = LibraryName;
        if (!string.IsNullOrEmpty(Version))
            v += $" (v{Version})";
        return v;
    }

    public bool IsSame(LibraryProjectReference reference)
    {
        return reference != null && LibraryName == reference.LibraryName;
    }


}
