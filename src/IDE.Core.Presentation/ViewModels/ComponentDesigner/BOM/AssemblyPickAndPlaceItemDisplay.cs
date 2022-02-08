using System.ComponentModel;

namespace IDE.Documents.Views
{
    public class AssemblyPickAndPlaceItemDisplay
    {
        public string PartName { get; set; }
        //public string Comment { get; set; }
        public string Layer { get; set; }
        public string Footprint { get; set; }

        //[Display(Name ="CenterX(mm)")]
        [DisplayName("CenterX(mm)")]
        public string CenterX { get; set; }

        //[Display(Name = "CenterY(mm)")]
        [DisplayName("CenterY(mm)")]
        public string CenterY { get; set; }
        public string Rot { get; set; }
    }
}
