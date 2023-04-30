using IDE.Core.Interfaces;
using IDE.Core.ViewModels;
using IDE.Documents.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IDE.Core.Editors
{
    public static class CanvasHelper
    {
        public static ICanvasDesignerFileViewModel GetCanvasModelFromCurrentActiveDocument()
        {
            var app = ServiceProvider.Resolve<IApplicationViewModel>();
            if (app != null)
            {
                var canvas = app.ActiveDocument as ICanvasDesignerFileViewModel;

                return canvas;
            }

            return null;
        }
    }
}
