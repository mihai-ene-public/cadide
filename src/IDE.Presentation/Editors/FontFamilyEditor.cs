using System.Collections;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using System.Linq;

namespace IDE.Core.Editors
{
    public class FontFamilyEditor : ComboBoxEditor
    {
        protected override IEnumerable CreateItemsSource(Xceed.Wpf.Toolkit.PropertyGrid.PropertyItem propertyItem)
        {
            return System.Windows.Media.Fonts.SystemFontFamilies
                                       .Select(f => f.Source)
                                       .OrderBy(s => s);
        }
    }
}
