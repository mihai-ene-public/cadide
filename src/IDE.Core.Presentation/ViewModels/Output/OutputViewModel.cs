namespace IDE.Documents.Views
{
    using CommunityToolkit.Mvvm.Messaging;
    using Core.ViewModels;
    using IDE.Core;
    using IDE.Core.Interfaces;
    using System;
    using System.IO;
    using System.Text;

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
}