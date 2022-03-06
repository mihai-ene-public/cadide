using System.Collections.Generic;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views;
public interface IComponentDesigner : IFileBaseViewModel
{
    string Prefix { get; }

    FootprintDisplay Footprint { get; }
    IList<GateDisplay> Gates { get; }

}
