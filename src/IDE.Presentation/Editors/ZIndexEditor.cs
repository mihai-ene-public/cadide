using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using IDE.Controls.WPF.PropertyGrid;
using IDE.Controls.WPF.PropertyGrid.Editors;

namespace IDE.Core.Editors
{
    public class ZIndexEditor : ITypeEditor
    {

        public FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            var sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            var btnSendBack = new Button();
            btnSendBack.Content = GetFontAwesomeIcon(FontAwesome5.EFontAwesomeIcon.Solid_Minus);//"-";//"Send to back";
            btnSendBack.ToolTip = "Send to back";
            //btnSendBack.FontSize = 22;
            btnSendBack.FontWeight = FontWeights.Bold;
            btnSendBack.Padding = new Thickness(0);
            btnSendBack.Width = 24;
            btnSendBack.Height = 24;
            btnSendBack.Click += (s, e) =>
            {
                propertyItem.Value = (int)propertyItem.Value - 1;
            };

            var btnBringFront = new Button();
            btnBringFront.Margin = new Thickness(3,0,0,0);
            btnBringFront.Content = GetFontAwesomeIcon(FontAwesome5.EFontAwesomeIcon.Solid_Plus);//"+";//"Bring to front";
            btnBringFront.ToolTip= "Bring to front";
            //btnBringFront.FontSize = 18;
            btnBringFront.FontWeight = FontWeights.Bold;
            btnBringFront.Padding = new Thickness(0);
            btnBringFront.Width = 24;
            btnBringFront.Height = 24;
            btnBringFront.Click += (s, e) =>
            {
                propertyItem.Value = (int)propertyItem.Value + 1;
            };

            sp.Children.Add(btnSendBack);
            sp.Children.Add(btnBringFront);

            return sp;
        }

        object GetFontAwesomeIcon(FontAwesome5.EFontAwesomeIcon icon)
        {
            var c = new FontAwesome5.FontAwesome();
            c.Icon = icon;

            return c;
        }
    }
}
