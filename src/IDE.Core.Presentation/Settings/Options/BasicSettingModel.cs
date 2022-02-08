using IDE.Core.Interfaces;

namespace IDE.Core.Settings.Options
{
    //public abstract class BasicSettingNodeModel : 
    //{

    //}

    public abstract class BasicSettingModel : BaseViewModel, ISettingModel
    {
        public virtual bool IsVisible => true;

        public string Name { get; set; }
        public abstract ISettingData ToData();

        public abstract void LoadFromData(ISettingData settingData);

        public abstract void ResetSetting();
    }

    //public class ComponentEditorBOMSettingModel : BasicSettingModel
    //{
    //    bool filterSuppliers = false;
    //    public bool FilterSuppliers
    //    {
    //        get { return filterSuppliers; }
    //        set
    //        {
    //            filterSuppliers = value;
    //            OnPropertyChanged(nameof(FilterSuppliers));
    //        }
    //    }

    //    ObservableCollection<SupplierName> suppliers = new ObservableCollection<SupplierName>();
    //    public ObservableCollection<SupplierName> Suppliers
    //    {
    //        get { return suppliers; }
    //        set
    //        {
    //            suppliers = value;
    //            OnPropertyChanged(nameof(Suppliers));
    //        }
    //    }

    //    string apiKey;
    //    public string ApiKey
    //    {
    //        get { return apiKey; }
    //        set
    //        {
    //            apiKey = value;
    //            OnPropertyChanged(nameof(ApiKey));
    //        }
    //    }

    //    public override void ResetSetting()
    //    {
    //        var list = new List<string>();
    //        list.Add("Digi-Key");
    //        list.Add("Mouser");
    //        list.Add("Farnell");

    //        Suppliers = new ObservableCollection<SupplierName>(list.Select(f => new SupplierName { Name = f }));
    //        FilterSuppliers = false;
    //        ApiKey = null;
    //    }

       

       

    //    public override ISettingData ToData()
    //    {
    //        return new ComponentEditorBOMSetting
    //        {
    //            Name = Name,
    //            ApiKey = EncryptDescrypt.EncodeToBase64(ApiKey),
    //            FilterSuppliers = FilterSuppliers,
    //            Suppliers = Suppliers.Select(f => f.Name).ToList()
    //        };
    //    }

    //    public override void LoadFromData(ISettingData settingData)
    //    {
    //        var s = settingData as ComponentEditorBOMSetting;

    //        Name = "Component Editor BOM";
    //        ApiKey = EncryptDescrypt.DecodeFromBase64(s.ApiKey);
    //        FilterSuppliers = s.FilterSuppliers;
    //        Suppliers = new ObservableCollection<SupplierName>(s.Suppliers.Select(f => new SupplierName { Name = f }));
    //    }

    //    public class SupplierName
    //    {
    //        public string Name { get; set; }
    //    }
    //}
}
