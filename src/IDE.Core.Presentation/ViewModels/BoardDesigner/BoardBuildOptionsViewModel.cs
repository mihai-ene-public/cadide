using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using System.Collections.Generic;
using System.Linq;

namespace IDE.Documents.Views
{
    public class BoardBuildOptionsViewModel : BaseViewModel, IBoardBuildOptions
    {
        public BoardBuildOptionsViewModel(IBoardDesigner boardModel)
        {
            board = boardModel;
            Bom = new BoardBuildOptionsBomViewModel((BoardDesignerFileViewModel)board);

            Assembly = new BoardBuildOptionsAssemblyViewModel((BoardDesignerFileViewModel)board);
        }

        IBoardDesigner board;

        #region Gerber



        int gerberFormatBeforeDecimal;
        public int GerberFormatBeforeDecimal
        {
            get
            {
                return gerberFormatBeforeDecimal;
            }
            set
            {
                gerberFormatBeforeDecimal = value;
                OnPropertyChanged(nameof(GerberFormatBeforeDecimal));
            }
        }

        public IList<int> GerberFormatBeforeDecimalValues
        {
            get { return new List<int> { 2, 3, 4 }; }
        }

        //--------------------------------------

        int gerberFormatAfterDecimal;
        public int GerberFormatAfterDecimal
        {
            get
            {
                return gerberFormatAfterDecimal;
            }
            set
            {
                gerberFormatAfterDecimal = value;
                OnPropertyChanged(nameof(GerberFormatAfterDecimal));
            }
        }

        //From Gerber spec: "The resolution of a Gerber file must be at least 0.001 mil or 25 nm"
        //in: number of decimals: 6 (highest allowed value) (resolution of 0.001mil or 25nm)
        //mm: number of decimals: 5 (resolution of 10nm) or 6 (resolution of 1nm)
        public IList<int> GerberFormatAfterDecimalValues
        {
            get { return new List<int> { 5, 6 }; }
        }


        bool gerberPlotBoardOutlineOnAllLayers;
        public bool GerberPlotBoardOutlineOnAllLayers
        {
            get
            {
                return gerberPlotBoardOutlineOnAllLayers;
            }
            set
            {
                gerberPlotBoardOutlineOnAllLayers = value;
                OnPropertyChanged(nameof(GerberPlotBoardOutlineOnAllLayers));
            }
        }


        bool gerberCreateZipFile;
        public bool GerberCreateZipFile
        {
            get
            {
                return gerberCreateZipFile;
            }
            set
            {
                gerberCreateZipFile = value;
                OnPropertyChanged(nameof(GerberCreateZipFile));
            }
        }

        bool gerberWriteGerberMetadata;
        public bool GerberWriteGerberMetadata
        {
            get
            {
                return gerberWriteGerberMetadata;
            }
            set
            {
                gerberWriteGerberMetadata = value;
                OnPropertyChanged(nameof(GerberWriteGerberMetadata));
            }
        }

        bool gerberWriteNetListAttributes;
        public bool GerberWriteNetListAttributes
        {
            get
            {
                return gerberWriteNetListAttributes;
            }
            set
            {
                gerberWriteNetListAttributes = value;
                OnPropertyChanged(nameof(GerberWriteNetListAttributes));
            }
        }

        bool gerberCreateGerberAssemblyDrawings;
        public bool GerberCreateGerberAssemblyDrawings
        {
            get
            {
                return gerberCreateGerberAssemblyDrawings;
            }
            set
            {
                gerberCreateGerberAssemblyDrawings = value;
                OnPropertyChanged(nameof(GerberCreateGerberAssemblyDrawings));
            }
        }

        bool gerberCreateGerberPickAndPlaceFiles;
        public bool GerberCreateGerberPickAndPlaceFiles
        {
            get
            {
                return gerberCreateGerberPickAndPlaceFiles;
            }
            set
            {
                gerberCreateGerberPickAndPlaceFiles = value;
                OnPropertyChanged(nameof(GerberCreateGerberPickAndPlaceFiles));
            }
        }

        List<LayerType> plotLayerTypes = new List<LayerType>
                                {
                                     LayerType.Signal,
                                     LayerType.Plane,
                                     LayerType.SolderMask,
                                     LayerType.PasteMask,
                                     LayerType.SilkScreen,
                                     LayerType.Mechanical,
                                     LayerType.Generic,
                                     LayerType.BoardOutline
                                };
        public IList<ILayerDesignerItem> GerberPlotLayers
        {
            get
            {
                return board.LayerItems.Where(l => plotLayerTypes.Contains(l.LayerType)).ToList();
            }
        }

        OutputUnits gerberUnits;
        public OutputUnits GerberUnits
        {
            get
            {
                return gerberUnits;
            }
            set
            {
                gerberUnits = value;
                OnPropertyChanged(nameof(GerberUnits));
            }
        }

        #endregion

        #region NC Drill



        int nCDrillFormatBeforeDecimal;
        public int NCDrillFormatBeforeDecimal
        {
            get
            {
                return nCDrillFormatBeforeDecimal;
            }
            set
            {
                nCDrillFormatBeforeDecimal = value;
                OnPropertyChanged(nameof(NCDrillFormatBeforeDecimal));
            }
        }

        public IList<int> NCDrillFormatBeforeDecimalValues
        {
            get { return new List<int> { 2, 3, 4 }; }
        }

        //-------------------------------------------------------------

        int nCDrillFormatAfterDecimal;
        public int NCDrillFormatAfterDecimal
        {
            get
            {
                return nCDrillFormatAfterDecimal;
            }
            set
            {
                nCDrillFormatAfterDecimal = value;
                OnPropertyChanged(nameof(NCDrillFormatAfterDecimal));
            }
        }

        public IList<int> NCDrillFormatAfterDecimalValues
        {
            get { return new List<int> { 3, 4, 5 }; }
        }

        OutputUnits nCDrillUnits;
        public OutputUnits NCDrillUnits
        {
            get
            {
                return nCDrillUnits;
            }
            set
            {
                nCDrillUnits = value;
                OnPropertyChanged(nameof(NCDrillUnits));
            }
        }

        #endregion

        public BoardBuildOptionsBomViewModel Bom { get; set; }

        public BoardBuildOptionsAssemblyViewModel Assembly { get; set; }

        public void LoadFrom(BoardDocument boardDoc)
        {
            if (boardDoc.OutputOptions != null)
            {
                //gerber
                GerberFormatBeforeDecimal = boardDoc.OutputOptions.GerberFormatBeforeDecimal;
                GerberFormatAfterDecimal = boardDoc.OutputOptions.GerberFormatAfterDecimal;
                GerberUnits = boardDoc.OutputOptions.GerberUnits;
                GerberPlotBoardOutlineOnAllLayers = boardDoc.OutputOptions.GerberPlotBoardOutlineOnAllLayers;
                GerberCreateZipFile = boardDoc.OutputOptions.GerberCreateZipFile;
                GerberWriteGerberMetadata = boardDoc.OutputOptions.GerberWriteGerberMetadata;
                GerberWriteNetListAttributes = boardDoc.OutputOptions.GerberWriteNetListAttributes;
                GerberCreateGerberAssemblyDrawings = boardDoc.OutputOptions.GerberCreateGerberAssemblyDrawings;
                GerberCreateGerberPickAndPlaceFiles = boardDoc.OutputOptions.GerberCreateGerberPickAndPlaceFiles;

                //ND Drills
                NCDrillFormatBeforeDecimal = boardDoc.OutputOptions.NCDrillFormatBeforeDecimal;
                NCDrillFormatAfterDecimal = boardDoc.OutputOptions.NCDrillFormatAfterDecimal;
                NCDrillUnits = boardDoc.OutputOptions.NCDrillUnits;

            }

            Bom.LoadFrom(boardDoc.OutputOptions?.Bom);

            Assembly.LoadFrom(boardDoc.OutputOptions?.Assembly);
        }

        public void PreviewBom()
        {
            Bom.PreviewBom();
        }

        public void PreviewAssembly()
        {
            Assembly.PreviewAssembly();
        }

        internal void SaveTo(BoardDocument boardDoc)
        {
            boardDoc.OutputOptions = new BoardOutputOptions();

            //gerber
            boardDoc.OutputOptions.GerberFormatBeforeDecimal = GerberFormatBeforeDecimal;
            boardDoc.OutputOptions.GerberFormatAfterDecimal = GerberFormatAfterDecimal;
            boardDoc.OutputOptions.GerberUnits = GerberUnits;
            boardDoc.OutputOptions.GerberPlotBoardOutlineOnAllLayers = GerberPlotBoardOutlineOnAllLayers;
            boardDoc.OutputOptions.GerberCreateZipFile = GerberCreateZipFile;
            boardDoc.OutputOptions.GerberWriteGerberMetadata = GerberWriteGerberMetadata;
            boardDoc.OutputOptions.GerberWriteNetListAttributes = GerberWriteNetListAttributes;
            boardDoc.OutputOptions.GerberCreateGerberAssemblyDrawings = GerberCreateGerberAssemblyDrawings;
            boardDoc.OutputOptions.GerberCreateGerberPickAndPlaceFiles = GerberCreateGerberPickAndPlaceFiles;

            //ND Drills
            boardDoc.OutputOptions.NCDrillFormatBeforeDecimal = NCDrillFormatBeforeDecimal;
            boardDoc.OutputOptions.NCDrillFormatAfterDecimal = NCDrillFormatAfterDecimal;
            boardDoc.OutputOptions.NCDrillUnits = NCDrillUnits;

            Bom.SaveTo(boardDoc.OutputOptions.Bom);
            Assembly.SaveTo(boardDoc.OutputOptions.Assembly);
        }
    }
}
