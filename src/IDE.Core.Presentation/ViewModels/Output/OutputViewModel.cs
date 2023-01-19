using CommunityToolkit.Mvvm.Messaging;
using IDE.Core.Interfaces;
using IDE.Core.ViewModels;
using System.Text;

namespace IDE.Documents.Views;

/// <summary>
/// Implementation is based on Output Tool Window from Gemini project:
/// https://github.com/tgjones/gemini
/// </summary>
public class OutputToolWindow : ToolViewModel, IOutputToolWindow
{
    #region fields
  //  private readonly OutputWriter outputWriter;
    private readonly StringBuilder textModel;

    public const string ToolContentId = "<OutputToolWindow>";
    #endregion fields

    public OutputToolWindow(): base("Output")
    {
        textModel = new StringBuilder();

        ContentId = ToolContentId;

        StrongReferenceMessenger.Default.Register<IOutputToolWindow, string>(this, (vm, message) => AppendLine(message));
        StrongReferenceMessenger.Default.Register<IOutputToolWindow, ClearOutputMessage>(this, (vm, message) => Clear());
        
        StrongReferenceMessenger.Default.Register<IOutputToolWindow, ActivateOutputToolWindowMessage>(this, 
            (vm, message) =>
        {
            IsVisible = message.IsVisible;
            IsActive = message.IsActive;
        });
    }

    #region properties
  


    public string Text
    {
        get
        {
            return textModel.ToString();
        }
        set { }//seems silly but it is needed for SimpleTextEditor binding
    }

    public override PaneLocation PreferredLocation
    {
        get { return PaneLocation.Bottom; }
    }
    #endregion properties

    #region methods

    public void Clear()
    {
        textModel.Clear();
        OnTextChanged();
    }

    private void OnTextChanged()
    {
        OnPropertyChanged(nameof(Text));
    }

    public void AppendLine(string text)
    {
        textModel.AppendLine(text);
        OnTextChanged();
    }

    public void Append(string text)
    {
        textModel.Append(text);
        OnTextChanged();
    }

    #endregion methods
}