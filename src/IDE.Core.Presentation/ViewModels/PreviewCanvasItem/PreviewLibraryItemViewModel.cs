using IDE.Core;
using IDE.Core.Designers;
using System.Collections.Generic;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System;
using System.Linq;
using IDE.Core.Types.Media;

namespace IDE.Documents.Views
{
    //the purpose of this class is to load a library item (symbol, footprint, etc) into canvasModel
    public abstract class PreviewLibraryItemViewModel : BaseViewModel
    {
        public PreviewLibraryItemViewModel()
        {
            var dispatcher = ServiceProvider.Resolve<IDispatcherHelper>();
            canvasModel = new DrawingViewModel(null, dispatcher);
        }

        protected readonly DrawingViewModel canvasModel;//= new DrawingViewModel(null);
        public DrawingViewModel CanvasModel
        {
            get
            {
                return canvasModel;
            }
        }

        protected void LoadPrimitives(IList<ISelectableItem> primitives)
        {
            canvasModel.Items = primitives;
        }

        public abstract void PreviewDocument(LibraryItem libraryItem, ISolutionProjectNodeModel projectModel);

        public void ZoomToFit()
        {
            canvasModel.ZoomToFit();
        }

        public static PreviewLibraryItemViewModel CreateFromDocument(LibraryItem libraryItem)
        {
            if (libraryItem == null)
                return null;

            var mapping = new Dictionary<Type, Type>
            {
                { typeof(Symbol), typeof(PreviewSymbolViewModel) },
                { typeof(Footprint), typeof(PreviewFootprintViewModel) },
                { typeof(ModelDocument), typeof(PreviewModelViewModel) },
                { typeof(SchematicDocument), typeof(PreviewSchematicViewModel) }
            };

            var libraryItemType = libraryItem.GetType();
            if (mapping.ContainsKey(libraryItemType))
            {
                var model = (PreviewLibraryItemViewModel)Activator.CreateInstance(mapping[libraryItemType]);

                return model;
            }

            throw new NotSupportedException();
        }
    }



}
