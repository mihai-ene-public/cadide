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
        public static IDrawingViewModel GetCanvasModelFromCurrentActiveDocument()
        {
            var app = ServiceProvider.Resolve<IApplicationViewModel>();//ServiceProvider.GetService<IApplicationViewModel>();
            if (app!=null)
            {
                var canvas = app.ActiveDocument as CanvasDesignerFileViewModel;
                var canvasModel = canvas?.CanvasModel;

                return canvasModel;
            }

            return null;
        }
    }
}
