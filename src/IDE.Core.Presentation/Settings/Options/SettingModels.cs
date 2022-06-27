using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Themes;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IDE.Core.Settings.Options
{

    public class EnvironmentGeneralSettingModel : BasicSettingModel
    {
        public override bool IsVisible => false;

        string currentTheme = ThemesManager.DefaultThemeName;
        public string CurrentTheme
        {
            get
            {
                return currentTheme;
            }

            set
            {
                if (currentTheme != value)
                {
                    currentTheme = value;
                }
            }
        }

        string selectedLanguage = SettingsManager.DefaultLocal;

        public string LanguageSelected
        {
            get
            {
                return selectedLanguage;
            }

            set
            {
                if (selectedLanguage != value)
                {
                    selectedLanguage = value;
                }
            }
        }

        public int NumberItemsVisibleInWindowMenu { get; set; } = 10;

        public int NumberItemsVisibleInMRU { get; set; } = 10;

        public bool AutoDetectFileIsChanged { get; set; } = true;

        public bool AutoReloadFiles { get; set; } = false;

        public bool CheckForUpdates { get; set; } = false;

        public override void ResetSetting()
        {
            NumberItemsVisibleInMRU = 10;
            NumberItemsVisibleInWindowMenu = 10;
            AutoDetectFileIsChanged = true;
            AutoReloadFiles = false;
            CheckForUpdates = false;
        }

        public override ISettingData ToData()
        {
            return new EnvironmentGeneralSetting
            {
                AutoDetectFileIsChanged = AutoDetectFileIsChanged,
                AutoReloadFiles = AutoReloadFiles,
                CheckForUpdates = CheckForUpdates,
                CurrentTheme = CurrentTheme,
                LanguageSelected = LanguageSelected,
                Name = Name,
                NumberItemsVisibleInMRU = NumberItemsVisibleInMRU,
                NumberItemsVisibleInWindowMenu = NumberItemsVisibleInWindowMenu
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as EnvironmentGeneralSetting;

            AutoDetectFileIsChanged = s.AutoDetectFileIsChanged;
            AutoReloadFiles = s.AutoReloadFiles;
            CheckForUpdates = s.CheckForUpdates;
            CurrentTheme = s.CurrentTheme;
            LanguageSelected = s.LanguageSelected;
            Name = "Environment";//s.Name;
            NumberItemsVisibleInMRU = s.NumberItemsVisibleInMRU;
            NumberItemsVisibleInWindowMenu = s.NumberItemsVisibleInWindowMenu;
        }
    }

    public class EnvironmentFolderLibsSettingModel : BasicSettingModel
    {
        ObservableCollection<FolderName> folders = new ObservableCollection<FolderName>();
        public ObservableCollection<FolderName> Folders
        {
            get { return folders; }
            set
            {
                folders = value;
                OnPropertyChanged(nameof(Folders));
            }
        }

        public override bool IsVisible => true;

        public override void ResetSetting()
        {
            var list = new List<string>();
            list.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\XNoCAD\Libraries");
            list.Add("Libraries");//start up folder of IDE.exe

            Folders = new ObservableCollection<FolderName>(list.Select(f => new FolderName { Name = f }));
        }

        public override ISettingData ToData()
        {
            return new EnvironmentFolderLibsSettingData
            {
                Name = Name,
                Folders = Folders.Select(f => f.Name).ToList()
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as EnvironmentFolderLibsSettingData;
            Folders.Clear();

            Name = s.Name;
            Folders.AddRange(s.Folders.Select(f => new FolderName { Name = f }));
        }

        public class FolderName
        {
            public string Name { get; set; }
        }
    }

    public class EnvironmentKeyboardSettingModel : BasicSettingModel
    {
        //this is hidden because is a little complicaated to make the key binding and to work
        public override bool IsVisible => false;

        public List<KeyboardSetting> KeySettings { get; set; } = new List<KeyboardSetting>();

        public override ISettingData ToData()
        {
            return new EnvironmentKeyboardSetting
            {
                Name = Name,
                KeySettings = KeySettings.Select(s => s.Clone()).ToList()
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as EnvironmentKeyboardSetting;

            Name = s.Name;
            KeySettings = s.KeySettings.Select(ss => ss.Clone()).ToList();
        }

        public override void ResetSetting()
        {
        }
    }

    public class BoardEditorGeneralSettingModel : BasicSettingModel
    {
        public bool OnlineDRC { get; set; } = false;

        public bool RepourPolysOnDocumentChange { get; set; } = true;
        public bool ShowUnconnectedSignals { get; set; } = true;

        public override void ResetSetting()
        {
            OnlineDRC = false;
            RepourPolysOnDocumentChange = true;
            ShowUnconnectedSignals = true;
        }

        public override ISettingData ToData()
        {
            return new BoardEditorGeneralSetting
            {
                Name = Name,
                OnlineDRC = OnlineDRC,
                RepourPolysOnDocumentChange = RepourPolysOnDocumentChange,
                ShowUnconnectedSignals = ShowUnconnectedSignals
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as BoardEditorGeneralSetting;
            if (s == null)
                throw new NotImplementedException();

            Name = "Board Editor";//s.Name;
            OnlineDRC = s.OnlineDRC;
            RepourPolysOnDocumentChange = s.RepourPolysOnDocumentChange;
            ShowUnconnectedSignals = s.ShowUnconnectedSignals;
        }
    }

    public class BoardEditorColorsSettingModel : BasicSettingModel
    {
        public override bool IsVisible => true;

        XColor canvasBackground;
        public XColor CanvasBackground
        {
            get { return canvasBackground; }
            set
            {
                canvasBackground = value;
                OnPropertyChanged(nameof(CanvasBackground));
            }
        }

        XColor gridColor;
        public XColor GridColor
        {
            get { return gridColor; }
            set
            {
                gridColor = value;
                OnPropertyChanged(nameof(GridColor));
            }
        }

        //GridStyle: Lines/Dots
        //LayerColors

        public override void ResetSetting()
        {
            //CanvasBackground = XColors.Gray;
            //GridColor = XColor.FromHexString("#FF616161");
            CanvasBackground = XColor.FromHexString("#FF616161");
            GridColor = XColors.Gray;
        }

        public override ISettingData ToData()
        {
            return new BoardEditorColorsSetting
            {
                Name = Name,
                CanvasBackground = CanvasBackground.ToHexString(),
                GridColor = GridColor.ToHexString()
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as BoardEditorColorsSetting;
            Name = "Board Editor Colors";//s.Name;
            CanvasBackground = XColor.FromHexString(s.CanvasBackground);
            GridColor = XColor.FromHexString(s.GridColor);
        }
    }

    public class BoardEditorRoutingSettingModel : BasicSettingModel
    {
        public override bool IsVisible => false;


        public bool IgnoreObstacles { get; set; } = true;
        public bool PushObstacles { get; set; } = true;
        public bool WalkAroundObstacles { get; set; } = true;
        public bool StopAtFirstObstacle { get; set; } = true;

        public override void ResetSetting()
        {
            IgnoreObstacles = true;
            PushObstacles = true;
            WalkAroundObstacles = true;
            StopAtFirstObstacle = true;
        }

        public override ISettingData ToData()
        {
            return new BoardEditorRoutingSetting
            {
                Name = Name,
                IgnoreObstacles = IgnoreObstacles,
                PushObstacles = PushObstacles,
                StopAtFirstObstacle = StopAtFirstObstacle,
                WalkAroundObstacles = WalkAroundObstacles
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as BoardEditorRoutingSetting;

            Name = "Board Routing";//s.Name;
            IgnoreObstacles = s.IgnoreObstacles;
            PushObstacles = s.PushObstacles;
            StopAtFirstObstacle = s.StopAtFirstObstacle;
            WalkAroundObstacles = s.WalkAroundObstacles;
        }
    }

    public class BoardEditorPrimitiveDefaultsModel : BasicSettingModel
    {
        public BoardEditorPrimitiveDefaultsModel()
        {

        }

        public BoardEditorPrimitiveDefaultsModel(bool createDefaults)
        {
            if (createDefaults)
                CreateDefaultPrimitives();
        }

        public override bool IsVisible => false;

        public List<LayerPrimitive> Primitives { get; set; } = new List<LayerPrimitive>();

        public T GetPrimitive<T>() where T : LayerPrimitive
        {
            return Primitives.OfType<T>().FirstOrDefault();
        }

        void CreateDefaultPrimitives()
        {
            Primitives = new List<LayerPrimitive>
            {
                new ArcBoard
                {
                     BorderWidth = 0.2,
                     SizeDiameter = 2,
                },
                new CircleBoard
                {
                     BorderWidth = 0.5,
                },
                new LineBoard
                {
                    width = 0.2
                },
                new TextBoard
                {
                    TextAlign = XTextAlignment.Center,
                    TextDecoration = TextDecorationEnum.None,
                    FontSize = 24,
                    FontFamily = "Segoe UI",
                    Value = "Text"
                },
                new RectangleBoard
                {
                    BorderWidth = 0.5,
                }
                ,
                new PolygonBoard
                {
                    BorderWidth = 0.5,
                },
                new Pad
                {
                    Width = 0.5,
                    Height=0.6
                },
                new Smd
                {
                   Width = 0.5,
                   Height = 0.6
                },
                //Junction?
                //NetLabel
                //NetWire
            };
        }

        public override void ResetSetting()
        {
        }

        public override ISettingData ToData()
        {
            return new BoardEditorPrimitiveDefaults
            {
                Name = Name,
                Primitives = Primitives.Select(p => (LayerPrimitive)p.Clone()).ToList()
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as BoardEditorPrimitiveDefaults;

            Name = s.Name;
            Primitives = s.Primitives.Select(p => (LayerPrimitive)p.Clone()).ToList();
        }
    }

    public class SchematicEditorColorsSettingModel : BasicSettingModel
    {
        public override bool IsVisible => true;

        XColor canvasBackground;
        public XColor CanvasBackground
        {
            get { return canvasBackground; }
            set
            {
                canvasBackground = value;
                OnPropertyChanged(nameof(CanvasBackground));
            }
        }

        XColor gridColor;
        public XColor GridColor
        {
            get { return gridColor; }
            set
            {
                gridColor = value;
                OnPropertyChanged(nameof(GridColor));
            }
        }

        //GridStyle: Lines/Dots
        public override void ResetSetting()
        {
            CanvasBackground = XColor.FromHexString("#FF616161");
            GridColor = XColors.Gray;
        }

        public override ISettingData ToData()
        {
            return new SchematicEditorColorsSetting
            {
                Name = Name,
                CanvasBackground = CanvasBackground.ToHexString(),
                GridColor = GridColor.ToHexString()
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as SchematicEditorColorsSetting;
            Name = "Schematic Editor Colors";//s.Name;
            CanvasBackground = XColor.FromHexString(s.CanvasBackground);
            GridColor = XColor.FromHexString(s.GridColor);
        }
    }

    public class SchematicEditorPrimitiveDefaultsModel : BasicSettingModel
    {
        public SchematicEditorPrimitiveDefaultsModel()
        {

        }

        public SchematicEditorPrimitiveDefaultsModel(bool createDefaults)
        {
            if (createDefaults)
                CreateDefaultPrimitives();
        }

        public override bool IsVisible => false;

        public List<SchematicPrimitive> Primitives { get; set; } = new List<SchematicPrimitive>();

        public T GetPrimitive<T>() where T : SchematicPrimitive
        {
            return Primitives.OfType<T>().FirstOrDefault();
        }

        void CreateDefaultPrimitives()
        {
            Primitives = new List<SchematicPrimitive>
            {
                new Arc
                {
                     BorderWidth = 0.5,
                     BorderColor = "#FF000080",
                     FillColor = "#00FFFFFF",
                     Size = new XSize(2,2),
                },
                new Circle
                {
                     BorderWidth = 0.5,
                     BorderColor = "#FF000080",
                     FillColor = "#00FFFFFF"
                },
                new Ellipse
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF"
                },
                new LineSchematic
                {
                    lineStyle = LineStyle.Solid,
                    LineColor = "#FF000080",
                    width = 0.5
                },
                new Text
                {
                    TextAlign = XTextAlignment.Center,
                    TextDecoration = TextDecorationEnum.None,
                    textColor = "#FFFFFF",
                    backgroundColor = "#00FFFFFF",
                    FontSize = 24,
                    FontFamily = "Segoe UI",
                    Value = "Text"
                },
                new Rectangle
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF"
                }
                ,
                new Polygon
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF"
                },
                new Pin
                {
                    PinLength = 3,
                    Width = 0.5,
                    pinType = PinType.Passive,
                    Orientation = pinOrientation.Right,
                    PinColor = "#FF000080",
                    PinNameColor = "#FF000080",
                    PinNumberColor = "#FF000080"
                },
                new ImagePrimitive
                {
                    BorderWidth = 0.5,
                    BorderColor = "#FF000080",
                    FillColor = "#00FFFFFF",
                    CornerRadius = 0
                }
                //Junction?
                //NetLabel
                //NetWire
            };
        }

        public override void ResetSetting()
        {
        }

        public override ISettingData ToData()
        {
            return new SchematicEditorPrimitiveDefaults
            {
                Name = Name,
                Primitives = Primitives.Select(p => (SchematicPrimitive)p.Clone()).ToList()
            };
        }

        public override void LoadFromData(ISettingData settingData)
        {
            var s = settingData as SchematicEditorPrimitiveDefaults;

            Name = s.Name;
            Primitives = s.Primitives.Select(p => (SchematicPrimitive)p.Clone()).ToList();
        }
    }

    //public class ComponentEditorBOMSettingModel : BasicSettingModel
    //{
    //    bool filterSuppliers = false;
    //    public bool FilterSuppliers
    //    {
    //        get { return filterSuppliers; }
    //        set
    //        {
    //            filterSuppliers = value;
    //            OnPropertyChanged(nameof(FilterSuppliers));
    //        }
    //    }

    //    ObservableCollection<SupplierName> suppliers = new ObservableCollection<SupplierName>();
    //    public ObservableCollection<SupplierName> Suppliers
    //    {
    //        get { return suppliers; }
    //        set
    //        {
    //            suppliers = value;
    //            OnPropertyChanged(nameof(Suppliers));
    //        }
    //    }

    //    string apiKey;
    //    public string ApiKey
    //    {
    //        get { return apiKey; }
    //        set
    //        {
    //            apiKey = value;
    //            OnPropertyChanged(nameof(ApiKey));
    //        }
    //    }

    //    public override void ResetSetting()
    //    {
    //        var list = new List<string>();
    //        list.Add("Digi-Key");
    //        list.Add("Mouser");
    //        list.Add("Farnell");

    //        Suppliers = new ObservableCollection<SupplierName>(list.Select(f => new SupplierName { Name = f }));
    //        FilterSuppliers = false;
    //        ApiKey = null;
    //    }

       

       

    //    public override ISettingData ToData()
    //    {
    //        return new ComponentEditorBOMSetting
    //        {
    //            Name = Name,
    //            ApiKey = EncryptDescrypt.EncodeToBase64(ApiKey),
    //            FilterSuppliers = FilterSuppliers,
    //            Suppliers = Suppliers.Select(f => f.Name).ToList()
    //        };
    //    }

    //    public override void LoadFromData(ISettingData settingData)
    //    {
    //        var s = settingData as ComponentEditorBOMSetting;

    //        Name = "Component Editor BOM";
    //        ApiKey = EncryptDescrypt.DecodeFromBase64(s.ApiKey);
    //        FilterSuppliers = s.FilterSuppliers;
    //        Suppliers = new ObservableCollection<SupplierName>(s.Suppliers.Select(f => new SupplierName { Name = f }));
    //    }

    //    public class SupplierName
    //    {
    //        public string Name { get; set; }
    //    }
    //}
}
