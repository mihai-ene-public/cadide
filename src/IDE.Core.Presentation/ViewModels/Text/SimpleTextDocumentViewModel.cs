using IDE.Core.Interfaces;
using IDE.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace IDE.Documents.Views
{
    public class SimpleTextDocumentViewModel : FileBaseViewModel, ISimpleTextDocument
    {
        string text;
        public string Text
        {
            get { return text; }
            set
            {
                if (text == value)
                    return;

                text = value;
                OnPropertyChanged(nameof(Text));

                IsDirty = true;
            }
        }

        protected override async Task LoadDocumentInternal(string filePath)
        {
            Text = await File.ReadAllTextAsync(filePath);

            IsDirty = false;
        }

        protected override void SaveDocumentInternal(string filePath)
        {
            File.WriteAllText(filePath, Text);
        }

        public override IList<IDocumentToolWindow> GetToolWindowsWhenActive()
        {
            return new List<IDocumentToolWindow>();
        }

        public override void RegisterDocumentType(IDocumentTypeManager docTypeManager)
        {
        }
    }
}
