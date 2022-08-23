using IDE.Core.Converters;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.Toolkit;
using IDE.Controls.WPF.PropertyGrid.Editors;
//using Xceed.Wpf.Toolkit.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class XColorEditor : TypeEditor<ColorPicker>
    {
        protected override ColorPicker CreateEditor()
        {
            var c = new PropertyGridEditorColorPicker();
            c.BorderThickness = new Thickness();
            c.DisplayColorAndName = true;

            return c;
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = ColorPicker.SelectedColorProperty;
        }

        protected override IValueConverter CreateValueConverter()
        {
            return XColorToColorConverter.Instance;
        }
    }
}
