using IDE.Core.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IDE.Core.Designers
{
    public class SchematicBus : BaseViewModel, IBus
    {
        //Id is not used, nor saved
        public long Id { get; set; }

        string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name == value)
                    return;
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        bool isHighlighted;
        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                if (isHighlighted == value) return;
                isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        public bool IsNamed()
        {
            return name != null && !name.StartsWith("Bus");
        }

        //segments (netlabel,  wire)
        public List<BusSegmentCanvasItem> BusItems { get; set; } = new List<BusSegmentCanvasItem>();

        public ObservableCollection<string> Nets { get; set; } = new ObservableCollection<string>();

       // public IDrawingViewModel CanvasModel { get; set; }

        public void AddNet(string newNetName)
        {
            if (Nets.Contains(newNetName))
                return;

            Nets.Add(newNetName);
        }

        public void HighlightBus(bool newHighlight)
        {
            IsHighlighted = newHighlight;

            //todo: should we highlight the nets?
            //var netsToHighlight = CanvasModel.Items.OfType<NetSegmentCanvasItem>().Where(ns => ns.IsPlaced && ns.Net != null && Nets.Contains(ns.Net.Name))
            //                     .Select(n => n.Net)
            //                     .Distinct()
            //                     .ToList();

            //foreach (var net in netsToHighlight)
            //    net.HighlightNet(newHighlight);

        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Name))
                return "not assigned";
            return Name;
        }
    }
}
