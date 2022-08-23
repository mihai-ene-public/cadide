using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using IDE.Controls.WPF.PropertyGrid;
using IDE.Controls.WPF.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class LayerDesignerItemEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {

            var comboBox = new ComboBox();
            //var layersWindow = ServiceProvider.GetService<LayersToolWindowViewModel>();
            //if (layersWindow != null)
            //{
            //    comboBox.ItemsSource = layersWindow.LayeredDocument?.LayerItems;
            //}

            var layeredDoc = GetCurrentLayeredDocument();
            comboBox.ItemsSource = layeredDoc?.LayerItems;

            SetItemTemplate(comboBox);

            //create the binding from the bound property item to the editor
            var _binding = new Binding("Value"); //bind to the Value property of the PropertyItem
            _binding.Source = propertyItem;
            _binding.ValidatesOnExceptions = true;
            _binding.ValidatesOnDataErrors = true;
            _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(comboBox, ComboBox.SelectedItemProperty, _binding);

            return comboBox;
        }

        ILayeredViewModel GetCurrentLayeredDocument()
        {
            var app = ServiceProvider.Resolve<IApplicationViewModel>();//ServiceProvider.GetService<IApplicationViewModel>();
            if (app != null)
            {
                var layeredDoc = app.ActiveDocument as ILayeredViewModel;
                return layeredDoc;
            }
            return null;
        }

        void SetItemTemplate(ComboBox combo)
        {

            //var itemTemplate = new DataTemplate(typeof(LayerDesignerItem));
            //var factory = new FrameworkElementFactory(typeof(StackPanel));
            //itemTemplate.VisualTree = factory;

            var datatemplateXaml = @"<DataTemplate>
                    <Grid>
                        <Grid.ColumnDefinitions>
                            <ColumnDefinition Width='Auto' />
                            <ColumnDefinition Width='*' />
                        </Grid.ColumnDefinitions>

                        <Rectangle Grid.Column='0'
                                   Fill='{Binding LayerColor,Converter={x:Static conv:ColorToBrushConverter.Instance}, UpdateSourceTrigger=PropertyChanged, FallbackValue=White}'
                                   Width='20'
                                   Height='15'
                                   VerticalAlignment='Center'
                                   HorizontalAlignment='Left' />
                        <TextBlock Grid.Column='1'
                                   VerticalAlignment='Center'
                                    Margin='5,0,0,0'
                                   Text='{Binding LayerName, UpdateSourceTrigger=PropertyChanged, FallbackValue=unknown}' />
                       
                    </Grid>
                </DataTemplate>";

            using (var ms = new MemoryStream(Encoding.ASCII.GetBytes(datatemplateXaml)))
            {
                var pc = new ParserContext();
                pc.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                pc.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                pc.XmlnsDictionary.Add("conv", "clr-namespace:IDE.Core.Converters;assembly=IDE.Presentation");

                var datatemplate = (DataTemplate)XamlReader.Load(ms, pc);

                combo.ItemTemplate = datatemplate;
            }

        }
    }
}
