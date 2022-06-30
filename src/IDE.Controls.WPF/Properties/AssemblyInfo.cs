using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Markup;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("IDE.Controls.WPF")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("IDE.Controls.WPF")]
[assembly: AssemblyCopyright("Copyright ©  2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


//In order to begin building localizable applications, set
//<UICulture>CultureYouAreCodingWith</UICulture> in your .csproj file
//inside a <PropertyGroup>.  For example, if you are using US english
//in your source files, set the <UICulture> to en-US.  Then uncomment
//the NeutralResourceLanguage attribute below.  Update the "en-US" in
//the line below to match the UICulture setting in the project file.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly:ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                             //(used if a resource is not found in the page,
                             // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                      //(used if a resource is not found in the page,
                                      // app, or any theme specific resource dictionaries)
)]

[assembly: XmlnsPrefix("http://schemas.xceed.com/wpf/xaml/toolkit", "xctk")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.Core.Converters")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.Core.Input")]
//[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.Core.Media")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.Core.Utilities")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.Chromes")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.Primitives")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.PropertyGrid")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.PropertyGrid.Attributes")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.PropertyGrid.Commands")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.PropertyGrid.Converters")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.PropertyGrid.Editors")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/toolkit", "Xceed.Wpf.Toolkit.Panels")]

[assembly: XmlnsPrefix("http://schemas.xceed.com/wpf/xaml/avalondock", "xcad")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/avalondock", "Xceed.Wpf.AvalonDock")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/avalondock", "Xceed.Wpf.AvalonDock.Controls")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/avalondock", "Xceed.Wpf.AvalonDock.Converters")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/avalondock", "Xceed.Wpf.AvalonDock.Layout")]
[assembly: XmlnsDefinition("http://schemas.xceed.com/wpf/xaml/avalondock", "Xceed.Wpf.AvalonDock.Themes")]

[assembly: XmlnsPrefix("http://schemas.fontawesome.com/icons/", "fa5")]
[assembly: XmlnsDefinition("http://schemas.fontawesome.com/icons/", "FontAwesome5")]
[assembly: XmlnsDefinition("http://schemas.fontawesome.com/icons/", "FontAwesome5.Converters")]


