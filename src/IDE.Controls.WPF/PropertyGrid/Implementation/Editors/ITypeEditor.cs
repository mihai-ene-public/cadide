using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IDE.Controls.WPF.PropertyGrid.Editors;
public interface ITypeEditor
{
    FrameworkElement ResolveEditor(PropertyItem propertyItem);
}
