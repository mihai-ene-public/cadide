using IDE.Core;
using IDE.Core.BOM;
using IDE.Core.Commands;
using IDE.Core.Settings;
using IDE.Core.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;

namespace IDE.Documents.Views
{
    //public class BomSearchViewModel : DialogViewModel
    //{
    //    public BomSearchViewModel()
    //    {
    //        BomResults = new ObservableCollection<BomItemDisplay>();

    //        componentEditorBOMSetting = ApplicationServices.SettingsManager.GetSetting<ComponentEditorBOMSetting>();
    //    }

    //    ComponentEditorBOMSetting componentEditorBOMSetting;

    //    public string WindowTitle
    //    {
    //        get
    //        {
    //            return "Search Part (experimental)";
    //        }
    //    }

    //    string searchQuery;
    //    public string SearchQuery
    //    {
    //        get { return searchQuery; }
    //        set
    //        {
    //            searchQuery = value;
    //            OnPropertyChanged(nameof(SearchQuery));
    //        }
    //    }

    //    public ObservableCollection<BomItemDisplay> BomResults { get; set; }

    //    BomItemDisplay selectedItem;
    //    public BomItemDisplay SelectedItem
    //    {
    //        get { return selectedItem; }
    //        set
    //        {
    //            selectedItem = value;
    //            OnPropertyChanged(nameof(SelectedItem));
    //        }
    //    }

    //    bool addParameters = true;
    //    public bool AddParameters
    //    {
    //        get { return addParameters; }
    //        set
    //        {
    //            addParameters = value;
    //            OnPropertyChanged(nameof(AddParameters));
    //        }
    //    }

    //    ICommand searchCommand;

    //    public ICommand SearchCommand
    //    {
    //        get
    //        {
    //            if (searchCommand == null)
    //                searchCommand = CreateCommand(p =>
    //                  {
    //                      ExecuteSearch();
    //                  });

    //            return searchCommand;
    //        }
    //    }

    //    void ExecuteSearch()
    //    {
    //        try
    //        {
    //            BomResults.Clear();

    //            if (string.IsNullOrEmpty(searchQuery))
    //                return;
    //            var provider = new BomProvider();
    //            provider.ApiKey = EncryptDescrypt.DecodeFromBase64(componentEditorBOMSetting.ApiKey);
    //            provider.FilterSuppliers = componentEditorBOMSetting.FilterSuppliers;
    //            //provider.ShowInStock=componentEditorBOMSetting.sho
    //            provider.Suppliers = componentEditorBOMSetting.Suppliers;

    //            var results = provider.SearchByQuery(searchQuery);

    //            BomResults.AddRange(results.Select(r => new BomItemDisplay
    //            {
    //                ImageURLSmall = r.imageURLSmall,
    //                ImageURLMedium = r.imageURLMedium,
    //                Supplier = r.seller,
    //                Sku = r.sku,
    //                Manufacturer = r.manufacturer,
    //                MPN = r.mpn,
    //                Description = r.description,
    //                RoHS = r.rohs,
    //                Package = r.package,
    //                Packaging = r.packaging,
    //                Stock = r.stock,
    //                Currency = r.currency,
    //                Prices = r.prices.Select(kvp => new PriceDisplay
    //                {
    //                    Number = kvp.Key,
    //                    Price = kvp.Value
    //                }).ToList(),
    //                Properties = r.specs.Select(kvp => new NameValuePair
    //                {
    //                    Name = kvp.Key,
    //                    Value = kvp.Value
    //                }).ToList(),
    //                Documents = r.datasheets.Select(kvp => new NameValuePair
    //                {
    //                    Name = kvp.Key,
    //                    Value = kvp.Value
    //                }).ToList(),
    //            }));
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageDialog.Show(ex.Message);
    //        }

    //    }

    //}
}
