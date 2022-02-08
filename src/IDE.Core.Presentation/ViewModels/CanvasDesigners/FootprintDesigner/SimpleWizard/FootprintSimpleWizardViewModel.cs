using IDE.Core;
using IDE.Core.Common;
using IDE.Core.Designers;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Wizards;
using System.Collections.Generic;

namespace IDE.Documents.Views
{
    public class FootprintWizardItem : BaseViewModel, IWizardBusinessObject
    {

        public FootprintWizardItem()
        {
            //pad
            PadType = PadType.SMD;
            PadShape = PadShape.Rectangular;
            PadSizeX = 1;
            PadSizeY = 2;
            PadDrill = 0.5;

            //pad placement
            PadSpacingX = 0.5;
            PadSpacingY = 0.5;
            PadOffsetX = 1;
            PadOffsetY = 1;

            //silkscreen
            SilkscreenWidth = 0.2;
        }

        PackageType packageType;
        public PackageType PackageType
        {
            get { return packageType; }
            set
            {
                packageType = value;
                OnPropertyChanged(nameof(PackageType));

                OnPackageTypeChanged();
            }
        }

        void OnPackageTypeChanged()
        {
            //padtype
            //default smd
            var thisPadType = PadType.SMD;
            var thisPadShape = PadShape.Rectangular;
            var thisPadPlacement = PadPlacement.Linear;

            switch (packageType)
            {
                case PackageType.DIP:
                case PackageType.PGA:
                case PackageType.SPGA:
                    thisPadType = PadType.THT;
                    break;
            }

            //padshape
            switch (packageType)
            {
                case PackageType.BGA:
                case PackageType.SBGA:
                case PackageType.PGA:
                case PackageType.SPGA:
                    thisPadShape = PadShape.Circular;
                    break;
            }

            //pad placement
            switch (packageType)
            {
                case PackageType.Capacitors:
                case PackageType.Diodes:
                case PackageType.Resistors:
                case PackageType.EdgeConnector:
                    thisPadPlacement = PadPlacement.Linear;
                    break;

                case PackageType.SOP:
                case PackageType.DIP:
                    thisPadPlacement = PadPlacement.Rectangular;
                    break;

                case PackageType.QFP:
                case PackageType.LCC:
                    thisPadPlacement = PadPlacement.Quad;
                    break;

                case PackageType.BGA:
                case PackageType.SBGA:
                case PackageType.PGA:
                case PackageType.SPGA:
                    thisPadPlacement = PadPlacement.GateArray;
                    break;
            }

            PadType = thisPadType;
            PadShape = thisPadShape;
            PadPlacement = thisPadPlacement;
        }

        int numberPads = 10;
        public int NumberPads
        {
            get { return numberPads; }
            set
            {
                numberPads = value;
                OnPropertyChanged(nameof(NumberPads));
            }
        }

        int numberPadsX = 8;
        public int NumberPadsX
        {
            get { return numberPadsX; }
            set
            {
                numberPadsX = value;
                OnPropertyChanged(nameof(NumberPadsX));
            }
        }

        int numberPadsY = 8;
        public int NumberPadsY
        {
            get { return numberPadsY; }
            set
            {
                numberPadsY = value;
                OnPropertyChanged(nameof(NumberPadsY));
            }
        }

        #region Pad Definition

        PadShape padShape;
        public PadShape PadShape
        {
            get { return padShape; }
            set
            {
                padShape = value;
                OnPropertyChanged(nameof(PadShape));
            }
        }

        PadType padType;
        public PadType PadType
        {
            get { return padType; }
            set
            {
                padType = value;
                OnPropertyChanged(nameof(PadType));
            }
        }

        double padSizeX;
        public double PadSizeX
        {
            get { return padSizeX; }
            set
            {
                padSizeX = value;
                OnPropertyChanged(nameof(PadSizeX));
                OnPropertyChanged(nameof(PadSizeY));
            }
        }

        double padSizeY;
        public double PadSizeY
        {
            get { return padSizeY; }
            set
            {
                padSizeY = value;
                OnPropertyChanged(nameof(PadSizeY));
            }
        }

        double padDrill;
        public double PadDrill
        {
            get { return padDrill; }
            set
            {
                padDrill = value;
                OnPropertyChanged(nameof(PadDrill));
            }
        }


        #endregion

        #region Pad to Pad Definition

        PadPlacement padPlacement;

        public PadPlacement PadPlacement
        {
            get { return padPlacement; }
            set
            {
                padPlacement = value;
                OnPropertyChanged(nameof(PadPlacement));
            }
        }

        double padSpacingX;

        public double PadSpacingX
        {
            get { return padSpacingX; }
            set
            {
                padSpacingX = value;
                OnPropertyChanged(nameof(PadSpacingX));
            }
        }

        double padSpacingY;

        public double PadSpacingY
        {
            get { return padSpacingY; }
            set
            {
                padSpacingY = value;
                OnPropertyChanged(nameof(PadSpacingY));
            }
        }

        double padOffsetX;

        public double PadOffsetX
        {
            get { return padOffsetX; }
            set
            {
                padOffsetX = value;
                OnPropertyChanged(nameof(PadOffsetX));
            }
        }

        double padOffsetY;

        public double PadOffsetY
        {
            get { return padOffsetY; }
            set
            {
                padOffsetY = value;
                OnPropertyChanged(nameof(PadOffsetY));
            }
        }

        #endregion

        #region Pad Layout

        PadNamingStyle padNamingStyle;
        public PadNamingStyle PadNamingStyle
        {
            get { return padNamingStyle; }
            set
            {
                padNamingStyle = value;
                OnPropertyChanged(nameof(PadNamingStyle));
            }
        }

        #endregion

        #region Silkscreen

        double silkscreenWidth;

        public double SilkscreenWidth
        {
            get { return silkscreenWidth; }
            set
            {
                silkscreenWidth = value;
                OnPropertyChanged(nameof(SilkscreenWidth));
            }
        }

        #endregion

        public void Cancel()
        {
        }


        const double startLeft = 20;
        const double startTop = 20;

        //double currentLeft;
        //double currentTop;

        public IList<ISelectableItem> CreateFootprintItems()
        {
            var primitives = new List<ISelectableItem>();
            var padsCount = 2;

            //double left = 0;
            //double top = 0;

            switch (packageType)
            {
                case PackageType.Resistors:
                case PackageType.Capacitors:
                case PackageType.Diodes:
                    padsCount = 2;
                    break;
                case PackageType.DIP:
                case PackageType.SOP:
                case PackageType.EdgeConnector:
                    padsCount = NumberPads;
                    break;
                case PackageType.LCC:
                case PackageType.QFP:
                    padsCount = NumberPadsX * 2 + NumberPadsY * 2;
                    break;
            }


            #region Pads

            for (int i = 0; i < padsCount; i++)
            {
                //left = startLeft;
                //top = startTop;

                BoardCanvasItemViewModel pad = null;
                LayerPrimitive padPrimitive = null;
                switch (padType)
                {
                    case PadType.SMD:
                        {
                            padPrimitive = new Smd
                            {
                                Width = PadSizeX,
                                Height = PadSizeY,
                                number = (i + 1).ToString()
                            };
                            break;
                        }
                    case PadType.THT:
                        {
                            padPrimitive = new Pad()
                            {
                                drill = PadDrill,
                                Width = PadSizeX,
                                Height = PadSizeY,
                                number = (i + 1).ToString()
                            };
                            break;
                        }
                }

                //pad.Primitive = padPrimitive;
                pad = (BoardCanvasItemViewModel)padPrimitive.CreateDesignerItem();
                (pad as SingleLayerBoardCanvasItem).LayerId = LayerConstants.SignalTopLayerId;
                //pad.Width = PadSizeX;
                //pad.Height = PadSizeY;
                //pad.Number = (i + 1).ToString();
                //pad.CornerRadius = PadShape == PadShape.Circular ? Math.Min(padSizeX, padSizeY) : 0;

                //location
                LayoutPad((IPadCanvasItem)pad, padsCount, i);
                primitives.Add((ISelectableItem)pad);

            }

            #endregion

            #region Outline

            //var line = new LineBoard()
            //{
            //    layerId = Layer.GetTopOverlayLayer().Id,
            //    width = SilkscreenWidth,
            //    x1 = 0,
            //    y1 = 0,
            //    x2 = 2 + PadSizeX,
            //    y2 = 2
            //};
            //var lineCanvasItem = new LineBoardCanvasItem();
            //lineCanvasItem.Primitive = line;

            //primitives.Add((ISelectableItem)lineCanvasItem);

            #endregion

            return primitives;
        }

        void LayoutPad(IPadCanvasItem pad, int totalPads, int padIndex)
        {
            //todo: fix these below
            switch (PadPlacement)
            {
                case PadPlacement.Linear:
                    pad.Y = startTop;
                    pad.X = startLeft + (padIndex) * PadSpacingX;//direction could matter
                    break;

                case PadPlacement.Rectangular:
                    {
                        pad.Rot = 90;//horizontal
                        var sidePads = totalPads / 2;
                        var rowIndex = padIndex < sidePads ? 0 : 1;
                        var vertDirection = rowIndex == 0 ? 1 : -1;
                        pad.X = startLeft + rowIndex * PadSpacingX;
                        var dy = vertDirection == 1 ? padIndex : totalPads - padIndex - 1;
                        pad.Y = startTop + dy * PadSpacingY;
                        break;
                    }

                case PadPlacement.Quad:
                    {
                        var edgeSize = totalPads / 4;//32/4=8
                        var padEdgeIndex = padIndex / edgeSize;
                        var indexInEdge = padIndex % edgeSize;
                        //int vertDirection = 1;

                        //if (padEdgeIndex % 2 == 0)//vertical
                        //{
                        //    pad.Rot = 90;

                        //}
                        switch (padEdgeIndex)
                        {
                            case 0://vertical -down
                                {

                                    pad.Rot = 90;
                                    pad.X = startLeft;
                                    pad.Y = startTop + PadOffsetY + indexInEdge * PadSpacingY;
                                    break;
                                }
                            case 1://horizontal-right
                                {
                                    pad.X = startLeft + PadOffsetX + indexInEdge * padSpacingX;
                                    pad.Y = startTop + 2 * PadOffsetY + (edgeSize - 1) * PadSpacingY;

                                    break;
                                }
                            case 2://vertical up
                                {
                                    pad.Rot = 90;
                                    pad.X = startLeft + 2 * PadOffsetX + (edgeSize - 1) * PadSpacingX;
                                    pad.Y = startTop + PadOffsetY + (edgeSize - 1 - indexInEdge) * PadSpacingY;
                                    break;
                                }
                            case 3://horizontal-left
                                {
                                    pad.X = startLeft + PadOffsetX + (edgeSize - 1 - indexInEdge) * PadSpacingX;
                                    pad.Y = startTop ;
                                    break;
                                }
                        }

                        break;
                    }
            }
        }
    }

    public class FootprintSimpleWizardViewModel : WizardViewModel<FootprintWizardItem>
    {
        public FootprintSimpleWizardViewModel() : base()
        {
            BuildSteps();
        }

        public string WindowTitle
        {
            get { return "Footprint Wizard"; }
        }

        //1. Package type
        //2. Pad Definition: SMD/THT; Pad Dimensions, Pad shape (Rectangular|Round)
        //3. Pad to pad definition: Distance betw pads (X,Y);
        //4. Pads Layout
        //5. Silkscreen

        void BuildSteps()
        {
            var welcomeModel = new FootprintWelcomeStepViewModel(BusinessObject);
            var packageTypeModel = new FootprintPackageTypeStepViewModel(BusinessObject);
            var padDef = new FootprintPadDefinitionStepViewModel(BusinessObject);
            var pad2PadDef = new FootprintPadToPadDefinitionStepViewModel(BusinessObject);
            var padsLayout = new FootprintPadsLayoutStepViewModel(BusinessObject);
            var silkModel = new FootprintSilkScreenStepViewModel(BusinessObject);
            var summaryModel = new FootprintSummaryStepViewModel(BusinessObject);

            var steps = new List<CompleteStep<FootprintWizardItem>>()
            {
                new CompleteStep<FootprintWizardItem>() { ViewModel = welcomeModel,
                   // ViewType = typeof(FootprintWelcomeStepView),
                    Visited = true },

                new CompleteStep<FootprintWizardItem>() { ViewModel = packageTypeModel,
                //    ViewType = typeof(FootprintPackageTypeStepView)
                },
                new CompleteStep<FootprintWizardItem>() { ViewModel = padDef,
                    //ViewType = typeof(FootprintPadDefinitionStepView)
                },
                new CompleteStep<FootprintWizardItem>() { ViewModel = pad2PadDef,
                    //ViewType = typeof(FootprintPadToPadDefinitionStepView)
                },
                new CompleteStep<FootprintWizardItem>() { ViewModel = padsLayout,
                    //ViewType = typeof(FootprintPadsLayoutStepView)
                },
                new CompleteStep<FootprintWizardItem>() { ViewModel = silkModel,
                    //ViewType = typeof(FootprintSilkScreenStepView)
                },

                new CompleteStep<FootprintWizardItem>() { ViewModel = summaryModel,
                    //ViewType = typeof(FootprintSummaryStepView)
                },
            };

            ProvideSteps(steps);
        }
    }
}
