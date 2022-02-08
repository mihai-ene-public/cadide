using IDE.Core;
using IDE.Core.Interfaces;

namespace IDE.Documents.Views
{
    public class ItemDisplay : BaseViewModel
    {
        /// <summary>
        /// for icon converter
        /// </summary>
        public TemplateType ItemType { get; set; }

        public string Name { get; set; }

        public string LibraryName { get; set; }

        public string Description { get; set; }

        // public string FoundPath { get; set; }

        //Canvas

        //Items

        //Document (Symbol | Footprnt | Component)
        public object Document { get; set; }

        public virtual void OnPreviewChanged()
        {

        }

        public virtual void PreviewDocument()
        {

        }
    }
}
