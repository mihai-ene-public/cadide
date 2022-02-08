using System.Linq;
using System.Collections.ObjectModel;
using IDE.Core.Storage;
using IDE.Core;

namespace IDE.Documents.Views
{
    public class ConnectDisplay : BaseViewModel
    {
        public ConnectDisplay(ObservableCollection<GateDisplay> gates)
        {
            Gates = gates;
        }

        public ObservableCollection<GateDisplay> Gates { get; set; }

        public string Pad { get; set; }

        public string GateName
        {
            get
            {
                var gate = Gates.FirstOrDefault(g => g.Gate.Id == GateId);
                if (gate != null)
                    return gate.Name;
                return "?";//default
            }
        }

        long gateId;
        public long GateId
        {
            get
            {
                return gateId;
            }
            set
            {
                gateId = value;
                OnPropertyChanged(nameof(GateId));
                OnPropertyChanged(nameof(GateName));
                OnPropertyChanged(nameof(PinName));
            }
        }

        string pin;
        public string Pin
        {
            get
            {
                return pin;
            }
            set
            {
                pin = value;
                OnPropertyChanged(nameof(Pin));
                OnPropertyChanged(nameof(PinName));
            }
        }

        public string PinName
        {
            get
            {
                return GetPinName(GateId, Pin);
            }
        }

        string GetPinName(long gateId, string pinNumber)
        {
            var gate = Gates.FirstOrDefault(g => g.Gate.Id == gateId);
            if (gate != null)
            {
                var pin = gate.Symbol.Items.OfType<Pin>().FirstOrDefault(p => p.Number == pinNumber.ToString());
                if (pin != null)
                {
                    return pin.Name;
                }
            }

            return "NC";
        }

    }

}
