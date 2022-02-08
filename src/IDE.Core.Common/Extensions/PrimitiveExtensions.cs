using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Core.Common
{
    public static class PrimitiveExtensions
    {
        public static ISelectableItem CreateDesignerItem(this IPrimitive primitive)
        {
            var mapper = ServiceProvider.GetService<IPrimitiveToCanvasItemMapper>();
            var canvasItem = mapper.CreateDesignerItem(primitive);

            return canvasItem;
        }

        public static bool IsSame(this ILibraryItem thisItem, ILibraryItem other)
        {
            if (thisItem.GetType() != other.GetType())
                throw new Exception("Compared items must have the same type.");

            //serialize the 2 and compare the XML
            var thisXml = XmlHelper.SerializeObjectToXmlString(thisItem);
            var otherXml = XmlHelper.SerializeObjectToXmlString(other);

            return thisXml == otherXml;
        }

        public static List<ISelectableItem> GetDesignerPrimitiveItems(this Symbol symbol)
        {
            if (symbol.Items != null)
            {
                return symbol.Items.Select(p => p.CreateDesignerItem()).ToList();
            }
            else return new List<ISelectableItem>();
        }

        public static List<ISelectableItem> GetDesignerPrimitiveItems(this FontSymbol symbol)
        {
            if (symbol.Items != null)
            {
                return symbol.Items.Select(p => p.CreateDesignerItem()).ToList();
            }
            else return new List<ISelectableItem>();
        }

        public static ISolutionExplorerNodeModel CreateSolutionExplorerNodeModel(this IProjectFileRef fileItem)
        {
            var mapper = ServiceProvider.GetService<ISolutionExplorerNodeMapper>();
            var item = mapper.CreateSolutionExplorerNodeModel(fileItem);

            return item;
        }

        public static ISettingModel CreateModelItem(this ISettingData setting)
        {
            var mapper = ServiceProvider.GetService<ISettingsDataToModelMapper>();
            var item = mapper.CreateModelItem(setting);

            return item;
        }

        public static IBoardRuleModel CreateRuleItem(this IBoardRuleData rule)
        {
            var mapper = ServiceProvider.GetService<IBoardRulesDataToModelMapper>();
            var item = mapper.CreateRuleItem(rule);

            return item;
        }

        public static ISchematicRuleModel CreateRuleItem(this ISchematicRuleData rule)
        {
            var mapper = ServiceProvider.GetService<ISchematicRulesToModelMapper>();
            var item = mapper.CreateRuleItem(rule);

            return item;
        }

        public static bool IsOnSameSignalWith(this ISignalPrimitiveCanvasItem thisSignal, ISignalPrimitiveCanvasItem otherSignalItem)
        {
            return thisSignal != null
                && otherSignalItem != null
                && thisSignal.Signal != null
                && otherSignalItem.Signal != null
                && thisSignal.Signal.Name == otherSignalItem.Signal.Name;
        }

        public static double GetAngleOrientation(this IPinCanvasItem pin, bool isText)
        {
            //we make the text to be always displayed as to be easy to be read either from top-bottom or on the side
            if (isText)
            {
                switch (pin.Orientation)
                {
                    case pinOrientation.Right:
                    case pinOrientation.Left:
                        //should end zero
                        return -(double)pin.Orientation;
                    case pinOrientation.Up:
                    case pinOrientation.Down:
                        //should end up 90
                        return 270d - (double)pin.Orientation;
                }
            }
            else
            {
                //clockwise
                return (double)pin.Orientation;
            }

            return 0.0d;
        }
    }
}
