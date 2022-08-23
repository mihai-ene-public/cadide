using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Utilities;
using IDE.Core.ViewModels;
using System;
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
    public class HoleDefinitionEditor : ITypeEditor
    {
        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var holeItem = propertyItem.Value as HoleCanvasItem;

            var stackPanel = new StackPanel() { Margin = new Thickness(5, 5, 0, 5) };
            var slotStackPanel = new StackPanel();

            //Hole type
            var cbHoleTypes = new ComboBox(); ;// { Margin = new Thickness { Top = 5 } };
            var holeTypes = new[] { DrillType.Drill, DrillType.Slot };
            cbHoleTypes.ItemsSource = holeTypes;
            cbHoleTypes.SelectionChanged += (s, e) =>
              {
                  var selectedType = (DrillType)cbHoleTypes.SelectedItem;
                  if (selectedType == DrillType.Slot)
                      slotStackPanel.Visibility = Visibility.Visible;
                  else
                      slotStackPanel.Visibility = Visibility.Collapsed;
              };

            var cbHolesBinding = new Binding($"{nameof(propertyItem.Value)}.{nameof(HoleCanvasItem.DrillType)}");
            cbHolesBinding.Source = propertyItem;
            cbHolesBinding.ValidatesOnExceptions = true;
            cbHolesBinding.ValidatesOnDataErrors = true;
            cbHolesBinding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            BindingOperations.SetBinding(cbHoleTypes, ComboBox.SelectedItemProperty, cbHolesBinding);

            stackPanel.Children.Add(cbHoleTypes);

            //drill
            stackPanel.Children.Add(new TextBlock { Text = "Drill size", Margin = new Thickness { Top = 10 }, FontWeight=FontWeights.Bold });
            var drillElement = new SizeMilimetersUnitsEditor().GetEditorElement(propertyItem, $"{nameof(propertyItem.Value)}.{nameof(holeItem.Drill)}");
            stackPanel.Children.Add(drillElement);

            //drill offset X
            stackPanel.Children.Add(new TextBlock { Text = "Drill offset X", Margin = new Thickness { Top = 5 }, FontWeight = FontWeights.Bold });
            var offXElement = new PositionXUnitsEditor().GetEditorElement(propertyItem, $"{nameof(propertyItem.Value)}.{nameof(holeItem.X)}");
            stackPanel.Children.Add(offXElement);

            //drill offset Y
            stackPanel.Children.Add(new TextBlock { Text = "Drill offset Y", Margin = new Thickness { Top = 5 }, FontWeight = FontWeights.Bold });
            var offYElement = new PositionYUnitsEditor().GetEditorElement(propertyItem, $"{nameof(propertyItem.Value)}.{nameof(holeItem.Y)}");
            stackPanel.Children.Add(offYElement);

            stackPanel.Children.Add(slotStackPanel);
            //if (holeItem.DrillType == DrillType.Slot)
            {
                //slot height
                slotStackPanel.Children.Add(new TextBlock { Text = "Slot height", Margin = new Thickness { Top = 5 }, FontWeight = FontWeights.Bold });
                var slotHeightElement = new SizeMilimetersUnitsEditor().GetEditorElement(propertyItem, $"{nameof(propertyItem.Value)}.{nameof(holeItem.Height)}");
                slotStackPanel.Children.Add(slotHeightElement);

                //slot Rotation

                slotStackPanel.Children.Add(new TextBlock { Text = "Slot rotation", Margin = new Thickness { Top = 5 }, FontWeight = FontWeights.Bold });
                var slotRotElement = new TextBox();
                var slotRotElementBinding = new Binding(nameof(holeItem.Rot));
                slotRotElementBinding.Source = holeItem;//propertyItem;
                slotRotElementBinding.ValidatesOnExceptions = true;
                slotRotElementBinding.ValidatesOnDataErrors = true;
                slotRotElementBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                slotRotElementBinding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
                BindingOperations.SetBinding(slotRotElement, TextBox.TextProperty, slotRotElementBinding);


                slotStackPanel.Children.Add(slotRotElement);
            }



            return stackPanel;
        }
    }
}
