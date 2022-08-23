using System.Collections;
using IDE.Controls.WPF.PropertyGrid;
using IDE.Controls.WPF.PropertyGrid.Editors;
using System.Linq;

namespace IDE.Core.Editors
{
    public class FontFamilyEditor : ComboBoxEditor
    {
        protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
        {
            return System.Windows.Media.Fonts.SystemFontFamilies
                                       .Select(f => f.Source)
                                       .OrderBy(s => s);
        }
    }
}
