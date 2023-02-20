using System.IO;
using IDE.Core;
using IDE.Core.Interfaces;
using IDE.Core.Storage;

namespace IDE.Documents.Views;

public class BuiltInTemplateRepository : IBuiltInTemplateRepository
{
    public void CreateItemFromTemplate(BuiltInTemplateItemInfo templateItemInfo, string itemFilePath)
    {
        var document = templateItemInfo.Documents.FirstOrDefault();

        //var destFolder = Path.GetDirectoryName(itemFilePath);
        var filePath = $"{itemFilePath}.{templateItemInfo.TemplateItem.Extension}";

        if(File.Exists(filePath))
        {
            throw new Exception($"File already exists: {filePath}");
        }

        if (document == null)
        {
            File.WriteAllText(filePath, "");
        }
        else
        {
            XmlHelper.Save(document, filePath);
        }
    }

    public IList<TemplateItemInfo> LoadTemplates(TemplateType templateType)
    {
        var templates = new List<TemplateItemInfo>
        {
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Project,
                    Name = "Board project",
                    Description = "An empty project.",
                    Extension = "project"
                },
                Documents = new List<object>
                {
                    new ProjectDocument
                    {
                        OutputType = ProjectOutputType.Board,
                        References = new List<ProjectDocumentReference>
                        {
                            new PackageProjectReference
                            {
                                PackageId = "System.Libraries",
                                PackageVersion = "0.2.0-preview",
                                PackageSource = "https://github.com/mihai-ene-public/xnocad.packages.index/raw/main/index.json"
                            }
                        }
                    }
                }
            },

            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Project,
                    Name = "Library project",
                    Description = "An empty library project. Here you can define components, footprints, symbols, etc",
                    Extension = "project"
                },
                Documents = new List<object>
                {
                    new ProjectDocument
                    {
                        OutputType = ProjectOutputType.Library,
                        References = new List<ProjectDocumentReference>
                        {
                            new PackageProjectReference
                            {
                                PackageId = "System.Libraries",
                                PackageVersion = "0.2.0-preview",
                                PackageSource = "https://github.com/mihai-ene-public/xnocad.packages.index/raw/main/index.json"
                            }
                        }
                    }
                }
            },


            //Boards
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Board,
                    Name = "Board 2 layers (Top - Bottom)",
                    Description = "A new empty board on 2 layers: Top and bottom",
                    Extension = "board"
                },
                Documents = new List<object>
                {
                    new BoardDocument
                    {
                        Layers = new List<Layer>
                        {
                            Layer.GetTopSilkscreenLayer(),
                            Layer.GetTopPasteLayer(),
                            Layer.GetTopSolderLayer(),
                            Layer.GetTopCopperLayer(),
                            Layer.GetDielectricLayer(),
                            Layer.GetBottomCopperLayer(),
                            Layer.GetBottomSolderLayer(),
                            Layer.GetBottomPasteLayer(),
                            Layer.GetBottomSilkscreenLayer(),
                            Layer.GetTopMechanicalLayer(),
                            Layer.GetBottomMechanicalLayer(),
                            Layer.GetMillingLayer(),
                            Layer.GetBoardOutlineLayer()
                        }
                    }
                }
            },
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Board,
                    Name = "Board 4 layers (Top, GND, VCC, Bottom)",
                    Description = "A new empty board on 4 layers: 2 signal and 2 planes",
                    Extension = "board"
                },
                Documents = new List<object>
                {
                    new BoardDocument
                    {
                        Layers = new List<Layer>
                        {
                            Layer.GetTopSilkscreenLayer(),
                            Layer.GetTopPasteLayer(),
                            Layer.GetTopSolderLayer(),
                            Layer.GetTopCopperLayer(),
                            Layer.GetDielectricLayer(),
                            new Layer
                            {
                                Id = 101,
                                Name = "Plane(GND)",
                                Type = LayerType.Plane,
                                Color = "#FF483D8B",
                                Plot = true
                            },
                            new Layer
                            {
                                Id = 602,
                                Name = "Dielectric",
                                Type = LayerType.Dielectric,
                                Color = "#FFD2691E",
                                Thickness = 1,
                                Plot = true
                            },
                            new Layer
                            {
                                Id = 102,
                                Name = "Plane(VCC)",
                                Type = LayerType.Plane,
                                Color = "#FF008B8B",
                                Plot = true
                            },
                            new Layer
                            {
                                Id = 603,
                                Name = "Dielectric",
                                Type = LayerType.Dielectric,
                                Color = "#FFD2691E",
                                Thickness = 1,
                                Plot = true
                            },
                            Layer.GetBottomCopperLayer(),
                            Layer.GetBottomSolderLayer(),
                            Layer.GetBottomPasteLayer(),
                            Layer.GetBottomSilkscreenLayer(),
                            Layer.GetTopMechanicalLayer(),
                            Layer.GetBottomMechanicalLayer(),
                            Layer.GetMillingLayer(),
                            Layer.GetBoardOutlineLayer()
                        }
                    }
                }
            },

            //Schematic
             new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Schematic,
                    Name = "Schematic",
                    Description = "A new empty schematic",
                    Extension = "schematic"
                },
                Documents = new List<object>
                {
                    new SchematicDocument
                    {
                        Sheets = new List<Sheet>
                        {
                            new Sheet{ Name = "Main sheet" }
                        }
                    }
                }
            },

             //Component
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Component,
                    Name = "Component",
                    Description = "A new empty component",
                    Extension = "component"
                },
                Documents = new List<object>
                {
                    new ComponentDocument
                    {
                    }
                }
            },

             //Footprint
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Footprint,
                    Name = "Footprint",
                    Description = "A new empty footprint",
                    Extension = "footprint"
                },
                Documents = new List<object>
                {
                    new Footprint
                    {
                        Layers = new List<Layer>
                        {
                            Layer.GetTopSilkscreenLayer(),
                            Layer.GetTopPasteLayer(),
                            Layer.GetTopSolderLayer(),
                            Layer.GetTopCopperLayer(),
                            Layer.GetTopMechanicalLayer(),
                            Layer.GetMillingLayer()
                        }
                    }
                }
            },

            //Model
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Model,
                    Name = "Model",
                    Description = "A new empty model",
                    Extension = "model"
                },
                Documents = new List<object>
                {
                    new ModelDocument
                    {
                    }
                }
            },

            //Symbol
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Symbol,
                    Name = "Symbol",
                    Description = "A new empty symbol",
                    Extension = "symbol"
                },
                Documents = new List<object>
                {
                    new Symbol
                    {
                    }
                }
            },

            //Text
            new BuiltInTemplateItemInfo
            {
                TemplateItem = new TemplateItem
                {
                    TemplateType = TemplateType.Misc,
                    Name = "Text",
                    Description = "A new empty text",
                    Extension = "txt"
                },
                Documents = new List<object>
                {
                    //this is a content document: txt
                }
            },

        };

        return templates.Where(t => t.TemplateItem.TemplateType == templateType).ToList();
    }
}
