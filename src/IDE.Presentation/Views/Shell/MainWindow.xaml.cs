using System;
using IDE.Core.Utilities;
using IDE.Core.Interfaces;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using IDE.Core;
using System.IO;
using System.Windows.Threading;
using IDE.Controls.WPF.Windows;
using IDE.Controls.WPF.Docking.Layout;
using IDE.Core.ViewModels;

namespace IDE.App.Views.Shell;

public partial class MainWindow : ModernWindow, ILayoutableWindow
{
    public MainWindow()
    {
        InitializeComponent();

        InitComponents();

        Loaded += (s, e) => Init();
    }

    private void InitComponents()
    {
        ILayoutUpdateStrategy layoutUpdateStrategy = new LayoutInitializer();

        dockingManager.LayoutUpdateStrategy = layoutUpdateStrategy;

        LoadPanesStyleSelector();

        //var layoutInitializer = layoutUpdateStrategy as LayoutInitializer;
        //layoutInitializer.EnsureLayout(dockingManager.Layout);
    }

    private void LoadPanesStyleSelector()
    {
        var _panesStyleSelector = new PanesStyleSelector();
        var ResourceLocator = ServiceProvider.Resolve<IResourceLocator>();

        var newStyle = ResourceLocator.FindResource<Style>(
                                     "IDE.Presentation",
                                     "Resources/Styles/AvalonDockStyles.xaml",
                                     "FileStyle");

        _panesStyleSelector.RegisterStyle(typeof(FileBaseViewModel), newStyle);

        newStyle = ResourceLocator.FindResource<Style>(
                                "IDE.Presentation",
                                "Resources/Styles/AvalonDockStyles.xaml",
                                "ToolStyle");

        _panesStyleSelector.RegisterStyle(typeof(ToolViewModel), newStyle);

        dockingManager.LayoutItemContainerStyleSelector = _panesStyleSelector;
    }

    void Init()
    {
        var appVM = ServiceProvider.Resolve<IApplicationViewModel>();

        appVM.LoadLayoutRequested +=
         (s, e) =>
         {
             //todo: de-comment this line when we're ready 
             //LoadXmlLayout(e);
         };

        Closing += (s, e) =>
         {
             appVM.OnClosing(s, e);
         };

        // When the ViewModel asks to be closed, close the window.
        // Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
        appVM.RequestClose += (s, e) =>
        {
            try
            {
                //todo: de-comment when we're ready 
                //save xml layout
                //var xmlLayout = GetXmlAvalonDockLayout();
                //appVM.SaveXmlLayout(xmlLayout);
            }
            catch (Exception ex)
            {
                MessageDialog.Show(ex.Message);
            }

            // Save session data and close application
            appVM.OnClosed();
        };

        appVM.RequestExit += (s, e) =>
          {
              try { Close(); } catch { }
          };
    }

    public void AddCommanBindings(IList<CommandBindingData> bindings)
    {
        foreach (var cmdData in bindings)
        {
            var cmdBinding = new CommandBinding(cmdData.Command, (s, e) =>
            {
                var c = cmdData.ExecuteAction;
                c(e.Parameter);
                if (cmdData.HandledAction != null)
                {
                    var handled = cmdData.HandledAction;
                    e.Handled = handled(e.Parameter);
                }
            },
            (s, e) =>
            {
                var c = cmdData.CanExecuteAction;
                e.CanExecute = c(e.Parameter);
            });

            CommandBindings.Add(cmdBinding);
        }
    }

    string GetXmlAvalonDockLayout()
    {
        if (dockingManager == null)
            return string.Empty;

        var xmlLayoutString = string.Empty;
        try
        {
            using (var fs = new StringWriter())
            {
                var xmlLayout = new XmlLayoutSerializer(dockingManager);

                xmlLayout.Serialize(fs);

                xmlLayoutString = fs.ToString();
            }
        }
        catch
        {
        }

        return xmlLayoutString;
    }

    void LoadXmlLayout(string xmlLayout)
    {
        if (string.IsNullOrEmpty(xmlLayout))
            return;

        if (dockingManager == null)
            return;

        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                var sr = new StringReader(xmlLayout);
                var layoutSerializer = new XmlLayoutSerializer(dockingManager);
                layoutSerializer.LayoutSerializationCallback += UpdateLayout;
                layoutSerializer.Deserialize(sr);
            }
            catch
            {
                //logger.ErrorFormat("Error Loading Layout: {0}\n\n{1}", exp.Message, xmlLayout);
            }

        }), DispatcherPriority.Background);

    }

    /// <summary>
    /// Convert a Avalondock ContentId into a viewmodel instance
    /// that represents a document or tool window. The re-load of
    /// this component is cancelled if the Id cannot be resolved.
    /// 
    /// The result is (viewmodel Id or Cancel) is returned in <paramref name="args"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void UpdateLayout(object sender, LayoutSerializationCallbackEventArgs args)
    {
        try
        {
            //var resolver = DataContext as IViewModelResolver;

            //if (resolver == null)
            //    return;

            //// Get a matching viewmodel for a view through DataContext of this view
            //var content_view_model = resolver.ContentViewModelFromID(args.Model.ContentId);

            //if (content_view_model == null)
            //    args.Cancel = true;

            //// found a match - return it
            //args.Content = content_view_model;
        }
        catch (Exception exp)
        {
            Console.WriteLine(exp.Message);
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        if (DataContext is IApplicationViewModel application)
        {
            if (application.ActiveDocument is ICanvasDesignerFileViewModel canvasDesigner)
            {
                switch (e.Key)
                {
                    case Key.X:
                        {
                            canvasDesigner.MirrorXSelectedItems();
                            break;
                        }

                    case Key.Y:
                        {
                            canvasDesigner.MirrorYSelectedItems();
                            break;
                        }
                    case Key.L:
                        {
                            canvasDesigner.ChangeFootprintPlacement();
                            break;
                        }
                }
            }
        }
    }
}
