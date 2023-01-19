using Eagle;
using IDE.Core.Common.Geometries;
using IDE.Core.Coordinates;
using IDE.Core.Interfaces;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using IDE.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IDE.Core.Importers
{
    public class EagleImporter
    {

        readonly List<LayerMappingItem> footprintLayerMapping = new List<LayerMappingItem>
        {
            new LayerMappingItem
            {
                 SourceLayerId = 1,
                 SourceLayerName = "Top",

                 DestLayerId = LayerConstants.SignalTopLayerId,
                 DestLayerName = "Top Layer"
            },
            new LayerMappingItem
            {
                 SourceLayerId = 2,
                 SourceLayerName = "Route2",

                 DestLayerId = 2,
                 DestLayerName = "L2"
            },
            new LayerMappingItem
            {
                 SourceLayerId = 3,
                 SourceLayerName = "Route3",

                 DestLayerId = 3,
                 DestLayerName = "L3"
            },
            new LayerMappingItem
            {
                 SourceLayerId = 4,
                 SourceLayerName = "Route4",

                 DestLayerId = 4,
                 DestLayerName = "L4"
            },
            new LayerMappingItem
            {
                 SourceLayerId = 5,
                 SourceLayerName = "Route2",

                 DestLayerId = 5,
                 DestLayerName = "L5"
            },
            new LayerMappingItem
            {
                 SourceLayerId = 6,
                 SourceLayerName = "Route2",

                 DestLayerId = 6,
                 DestLayerName = "L6"
            },
            new LayerMappingItem
            {
                 SourceLayerId = 16,
                 SourceLayerName = "Bottom",

                 DestLayerId = LayerConstants.SignalBottomLayerId,
                 DestLayerName = "Bottom"
            },

             new LayerMappingItem
            {
                 SourceLayerId = 21,
                 SourceLayerName = "tPlace",

                 DestLayerId = LayerConstants.SilkscreenTopLayerId,
                 DestLayerName = "Top Silk"
            },
            //  new LayerMappingItem
            //{
            //     SourceLayerId = 25,
            //     SourceLayerName = "tNames",

            //     DestLayerId = LayerConstants.SilkscreenTopLayerId,
            //     DestLayerName = "Top Silk"
            //},
               new LayerMappingItem
            {
                 SourceLayerId = 29,
                 SourceLayerName = "tStop",

                 DestLayerId = LayerConstants.SolderTopLayerId,
                 DestLayerName = "Top Silk"
            },
                new LayerMappingItem
            {
                 SourceLayerId = 46,
                 SourceLayerName = "Milling",

                 DestLayerId = LayerConstants.MultiLayerMillingId,
                 DestLayerName = "Milling"
            },
                 new LayerMappingItem
            {
                 SourceLayerId = 51,
                 SourceLayerName = "tDocu",

                 DestLayerId = LayerConstants.MechanicalTopLayerId,
                 DestLayerName = "Mechanical 1"
            },
                 new LayerMappingItem
            {
                 SourceLayerId = 39,
                 SourceLayerName = "tKeepOut",

                 DestLayerId = LayerConstants.MechanicalTopLayerId,
                 DestLayerName = "Mechanical 1"
            },
                  new LayerMappingItem
            {
                 SourceLayerId = 31,
                 SourceLayerName = "tCream",

                 DestLayerId = LayerConstants.PasteTopLayerId,
                 DestLayerName = "Top Paste"
            },

        };

        public const string libraryExtension = ".lbr";
        public const string boardExtension = ".brd";

        public static string FilesFilter => string.Join(';', $"*{libraryExtension}"
#if DEBUG
                                                        , $"*{boardExtension}"
#endif
                                                        );

        /// <summary>
        /// Imports a .lbr (library) or an .brd (project) into a new Project
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationFolder"></param>
        public void Import(string sourceFilePath, string destinationFolder)
        {
            var extension = Path.GetExtension(sourceFilePath);
            switch (extension.ToLower())
            {
                case libraryExtension:
                    ImportLibrary(sourceFilePath, destinationFolder);
                    break;

                case boardExtension:
                    ImportProject(sourceFilePath, destinationFolder);
                    break;
            }
        }

        void ImportLibrary(string sourceFilePath, string destinationFolder)
        {
            //pinNumber = 1;
            //padNumber = 1;

            var libraryFile = XmlHelper.Load<eagle>(sourceFilePath);
            if (libraryFile.drawing != null)
            {
                var libObj = libraryFile.drawing.Item as library;
                var footprints = new List<Footprint>();
                var symbols = new List<Symbol>();
                var components = new List<ComponentDocument>();


                ExtractLibrary(libObj, footprints, symbols, components);

                #region Create Output

                var solutionFolder = destinationFolder;
                var slnName = Path.GetFileName(destinationFolder);
                var solDoc = new SolutionDocument();


                var projDoc = new ProjectDocument
                {
                    OutputType = ProjectOutputType.Library
                };
                var projName = Path.GetFileName(destinationFolder);

                solDoc.Children.Add(new SolutionProjectItem { RelativePath = $"{projName}/{projName}.project" });

                var projFolder = Path.Combine(solutionFolder, projName);
                Directory.CreateDirectory(projFolder);

                //var projSymbolsFolder = new ProjectFolderFile { RelativePath = "Symbols" };
                //var projFootprintsFolder = new ProjectFolderFile { RelativePath = "Footprints" };
                //var projComponentsFolder = new ProjectFolderFile { RelativePath = "Components" };

                //projDoc.Files.Add(projSymbolsFolder);
                //projDoc.Files.Add(projFootprintsFolder);
                //projDoc.Files.Add(projComponentsFolder);

                #region symbols

                var symbolsFolderPath = Path.Combine(projFolder, "Symbols");
                Directory.CreateDirectory(symbolsFolderPath);
                foreach (var smb in symbols)
                {
                    //var name = smb.Name.Replace(@"/", "_").Replace(@"\", "_");
                    //name = name.Replace(':', '_');
                    var name = ensureFileNameSafe(smb.Name);
                    var smbPath = Path.Combine(symbolsFolderPath, $"{name}.symbol");
                    XmlHelper.Save(smb, smbPath);
                    // projSymbolsFolder.Files.Add(new ProjectSymbolFile { RelativePath = Path.GetFileName(smbPath) });
                }


                #endregion

                #region footprints

                var footprintsFolderPath = Path.Combine(projFolder, "Footprints");
                Directory.CreateDirectory(footprintsFolderPath);
                foreach (var fpt in footprints)
                {
                    var name = ensureFileNameSafe(fpt.Name);
                    var fptPath = Path.Combine(footprintsFolderPath, $"{name}.footprint");
                    XmlHelper.Save(fpt, fptPath);
                    // projFootprintsFolder.Files.Add(new ProjectFootprintFile { RelativePath = Path.GetFileName(fptPath) });
                }


                #endregion

                #region components

                var compsFolderPath = Path.Combine(projFolder, "Components");
                Directory.CreateDirectory(compsFolderPath);
                foreach (var comp in components)
                {
                    //var deviceSetFolderPath = Path.Combine(compsFolderPath, kvpComp.Key);
                    //Directory.CreateDirectory(deviceSetFolderPath);

                    var name = ensureFileNameSafe(comp.Name);
                    var cmpPath = Path.Combine(compsFolderPath, $"{name}.component");
                    XmlHelper.Save(comp, cmpPath);
                    // projComponentsFolder.Files.Add(new ProjectComponentFile { RelativePath = Path.GetFileName(cmpPath) });
                }

                #endregion

                var slnpath = Path.Combine(solutionFolder, $"{slnName}.solution");
                XmlHelper.Save(solDoc, slnpath);

                var projPath = Path.Combine(projFolder, $"{projName}.project");
                XmlHelper.Save(projDoc, projPath);

                #endregion
            }
        }

        private void ExtractLibrary(library libObj, List<Footprint> footprints, List<Symbol> symbols, List<ComponentDocument> components)
        {
            if (libObj == null)
                return;

            //packages => footprints
            #region Footprints

            if (libObj.packages != null && libObj.packages.Packages != null)
            {
                foreach (var package in libObj.packages.Packages)
                {
                    var footprintName = package.name;

                    var footprintExists = footprints.Any(f => f.Name == footprintName);
                    if (footprintExists)
                        continue;

                    var footprintDoc = new Footprint();
                    // padNumber = 1;
                    footprintDoc.Id = LibraryItem.GetNextId();
                    footprintDoc.Name = footprintName;
                    footprintDoc.Items = new List<LayerPrimitive>();

                    if (package.Items == null || package.Items.Count == 0)
                        continue;

                    foreach (var item in package.Items)
                    {
                        var primitive = GetFootprintPrimitive(item);

                        if (primitive != null)
                            footprintDoc.Items.Add(primitive);
                    }

                    footprints.Add(footprintDoc);
                }
            }

            #endregion


            //symbols
            #region symbols

            //schematic, symbols are on imperial
            cartezianCS.Origin = new XPoint(25.4 * 2, 25.4 * 2);

            if (libObj.symbols != null && libObj.symbols.Symbols != null)
            {
                foreach (var smb in libObj.symbols.Symbols)
                {
                    var symbolExists = symbols.Any(f => f.Name == smb.name);
                    if (symbolExists)
                        continue;

                    var symbolDoc = new Symbol
                    {
                        Name = smb.name,
                        Id = LibraryItem.GetNextId(),
                        Items = new List<SchematicPrimitive>()
                    };

                    if (smb.Items == null || smb.Items.Count == 0)
                        continue;

                    foreach (var item in smb.Items)
                    {
                        var primitive = GetSchematicPrimitive(item);

                        if (primitive != null)
                            symbolDoc.Items.Add(primitive);
                    }

                    symbols.Add(symbolDoc);
                }
            }

            #endregion

            //components
            #region components

            if (libObj.devicesets != null && libObj.devicesets.DeviceSets != null)
            {
                //a device set is in fact multiple footprints grouped by a list of gates
                foreach (var deviceSet in libObj.devicesets.DeviceSets)
                {
                    if (deviceSet.gates == null || deviceSet.gates.Gates == null)
                        continue;
                    if (deviceSet.devices == null || deviceSet.devices.Devices == null)
                        continue;

                    var deviceSetName = deviceSet.name;
                    var prefix = deviceSet.prefix;
                    var gateId = 1;

                    //var gates = deviceSet.gates.Gates.Select(g => new Gate
                    //{
                    //    Id = gateId++, //todo: generate this (not with LibItem.NextId())
                    //    name = "G1",
                    //    symbolId = 0,//todo: lookup this by g.name
                    //    symbolName = ""
                    //}).ToList();

                    var gates = new List<Gate>();
                    foreach (var g in deviceSet.gates.Gates)
                    {
                        var symbol = symbols.FirstOrDefault(s => s.Name == g.symbol);
                        var sId = string.Empty;
                        var sName = "symbolName";
                        if (symbol != null)
                        {
                            sId = symbol.Id;
                            sName = symbol.Name;
                        }
                        var newGate = new Gate
                        {
                            Id = gateId,
                            name = g.name,//$"G${gateId}",
                            symbolId = sId,
                            symbolName = sName
                        };

                        gates.Add(newGate);

                        gateId++;
                    }


                    foreach (var device in deviceSet.devices.Devices)
                    {
                        var deviceName = device.name;
                        if (deviceName == null)
                            deviceName = string.Empty;

                        var cName = GetComponentName(deviceSetName, device.name);

                        var compExists = components.Any(f => f.Name == cName);
                        if (compExists)
                            continue;

                        var compDoc = new ComponentDocument
                        {
                            Id = LibraryItem.GetNextId(),
                            Name = cName,
                            Prefix = prefix,
                            Type = ComponentType.Standard,
                            Gates = gates,
                        };

                        var fp = footprints.FirstOrDefault(f => f.Name == device.package);

                        if (fp != null)
                        {
                            compDoc.Footprint = new FootprintRef
                            {
                                footprintId = fp.Id,//lookup
                                footprintName = fp.Name
                            };

                            if (device.connects != null && device.connects.ConnectList != null)
                            {
                                compDoc.Footprint.Connects = device.connects.ConnectList.Select(c => new Connect
                                {
                                    gateId = gates.FirstOrDefault(g => g.name == c.gate).Id, //lookup
                                    pin = c.pin,
                                    pad = c.pad
                                }).ToList();
                            }
                        }


                        components.Add(compDoc);
                    }
                }
            }

            #endregion
        }

        string ensureFileNameSafe(string name)
        {
            // / \ : * ? "
            var forbiddenChars = new[] { '/', '\\', ':', '*', '?', '"', '<', '>', '|' };
            foreach (var c in forbiddenChars)
                name = name.Replace(c, '_');

            return name;
        }

        LayerPrimitive GetFootprintPrimitive(object item)
        {
            LayerPrimitive primitive = null;

            if (item is circle)
            {
                var circle = item as circle;
                var layer = GetFootprintLayerIdMapping(circle.layer);

                if (layer > 0)
                    primitive = new CircleBoard
                    {
                        x = ToCanvasX(circle.x),
                        y = ToCanvasY(circle.y),
                        layerId = layer,//lookup in mapping
                        IsFilled = false,
                        BorderWidth = circle.width,
                        Diameter = circle.radius * 2
                    };
            }
            else if (item is hole)
            {
                var hole = item as hole;
                primitive = new Hole
                {
                    x = ToCanvasX(hole.x),
                    y = ToCanvasY(hole.y),
                    drill = hole.drill
                };
            }
            else if (item is pad)
            {
                //we must register the name and do other bindings
                var p = item as pad;
                var d = p.diameter;
                var rot = 360 - GetRotFromString(p.rot);
                if (d == 0)
                    d = p.drill * 2;
                var height = d;
                if (p.shape == padShape.@long)
                {
                    height = 2.0 * d;//this ratio seems to be set in design rules: by default 100%: ratio 2:1)
                    rot -= 90;
                }

                primitive = new Pad
                {
                    x = ToCanvasX(p.x),
                    y = ToCanvasY(p.y),
                    number = p.name,//we must generate this
                    rot = rot,
                    Width = d,
                    Height = height,
                    drill = p.drill,
                    CornerRadius = 0.5 * d,
                    AutoGenerateSolderMask = p.stop == padStop.yes
                    // SolderMaskExpansion = 0.1
                };

            }
            else if (item is polygon)
            {
                var poly = item as polygon;
                var layer = GetFootprintLayerIdMapping(poly.layer);

                if (layer > 0)
                {
                    primitive = new PolygonBoard
                    {
                        IsFilled = true,
                        BorderWidth = poly.width,
                        layerId = layer,
                        vertices = GetOutlineFromPoly(poly.vertices).Select(v => new Vertex { x = v.X, y = v.Y }).ToList()
                    };
                }
            }
            else if (item is rectangle)
            {
                var r = item as rectangle;
                var rr = new XRect(ToCanvas(new XPoint(r.x1, r.y1)), ToCanvas(new XPoint(r.x2, r.y2)));
                var rot = GetRotFromString(r.rot);
                var width = rr.Width;
                var height = rr.Height;

                var center = rr.GetCenter();
                var layer = GetFootprintLayerIdMapping(r.layer);


                if (layer > 0)
                    primitive = new RectangleBoard
                    {
                        X = center.X,
                        Y = center.Y,
                        Width = width,
                        Height = height,
                        IsFilled = true,
                        layerId = layer,
                        BorderWidth = 0,
                        CornerRadius = 0,
                        rot = rot
                    };
            }
            else if (item is smd)
            {
                var s = item as smd;
                primitive = new Smd
                {
                    x = ToCanvasX(s.x),
                    y = ToCanvasY(s.y),
                    number = s.name,//we must generate this
                    rot = -GetRotFromString(s.rot),
                    Width = s.dx,
                    Height = s.dy,
                    AutoGeneratePasteMask = s.cream == smdCream.yes,
                    AutoGenerateSolderMask = s.stop == smdStop.yes,
                    // SolderMaskExpansion = 0.1
                };

            }
            else if (item is text)
            {
                //ignore the text until we have the right font, but...
                var t = item as text;
                if (t.Value != null && (t.Value.ToLower() == ">value" || t.Value.ToLower() == ">name"))
                    return null;
                var layer = GetFootprintLayerIdMapping(t.layer);

                if (layer > 0)
                    primitive = new TextSingleLineBoard
                    {
                        x = ToCanvasX(t.x),
                        y = ToCanvasY(t.y),
                        Value = t.Value,
                        rot = GetRotFromString(t.rot),
                        layerId = layer,
                        FontSize = t.size,
                    };
            }
            else if (item is wire)
            {
                var l = item as wire;
                var layer = GetFootprintLayerIdMapping(l.layer);

                if (layer > 0)
                {
                    if (l.curve != 0.00d)
                    {
                        var startPoint = new XPoint(ToCanvasX(l.x1), ToCanvasY(l.y1));
                        var endPoint = new XPoint(ToCanvasX(l.x2), ToCanvasY(l.y2));

                        //if it is positive, it's counterclockwise
                        primitive = new ArcBoard
                        {
                            SizeDiameter = GetRadiusFromCurvature(l.curve, startPoint, endPoint),//?
                            IsLargeArc = l.curve > 180.0d || l.curve < -180.0d,
                            SweepDirection = l.curve > 0.0d ? XSweepDirection.Counterclockwise : XSweepDirection.Clockwise,
                            StartPoint = startPoint,
                            EndPoint = endPoint,
                            BorderWidth = l.width,
                            layerId = layer
                        };
                    }
                    else
                    {
                        primitive = new LineBoard
                        {
                            x1 = ToCanvasX(l.x1),
                            y1 = ToCanvasY(l.y1),
                            x2 = ToCanvasX(l.x2),
                            y2 = ToCanvasY(l.y2),
                            width = l.width,
                            layerId = layer
                        };
                    }
                }
            }

            return primitive;
        }

        IList<XPoint> GetOutlineFromPoly(IList<vertex> polyPoints)
        {
            var points = new List<XPoint>();

            if (polyPoints == null || polyPoints.Count == 0)
                return points;

            PathGeometryOutline pathOutline = null;
            var lastCurveRads = 0.0d;
            for (int i = 0; i < polyPoints.Count; i++)
            {
                var p = polyPoints[i];
                var px = new XPoint(ToCanvasX(p.x), ToCanvasY(p.y));

                //add current curve points if any
                if (pathOutline != null)
                {
                    pathOutline.AddArc(px, lastCurveRads);

                    var outlinePoints = pathOutline.GetOutline();
                    for (int j = 1; j < outlinePoints.Count; j++)
                    {
                        points.Add(outlinePoints[j]);
                    }
                }

                if (p.curve != 0.00)
                {
                    lastCurveRads = p.curve * Math.PI / 180.0d;
                    //start a curve
                    pathOutline = new PathGeometryOutline(px);
                }
                else
                {
                    points.Add(px);
                    pathOutline = null;
                }

                //last point is curve; finish to the first point in poly
                if (i == polyPoints.Count - 1 && pathOutline != null)
                {
                    var sp = polyPoints[0];
                    var spx = new XPoint(ToCanvasX(sp.x), ToCanvasY(sp.y));

                    pathOutline.AddArc(spx, lastCurveRads);

                    var outlinePoints = pathOutline.GetOutline();
                    for (int j = 1; j < outlinePoints.Count; j++)
                    {
                        points.Add(outlinePoints[j]);
                    }
                }
            }

            return points;
        }

        Primitive GetFootprintNetPrimitive(object item)
        {
            Primitive primitive = null;

            switch (item)
            {
                case contactref cref:
                    primitive = new PadRef
                    {
                        PadNumber = cref.pad
                    };
                    break;

                case polygon poly:
                    var layer = GetFootprintLayerIdMapping(poly.layer);

                    if (layer > 0)
                        primitive = new PolygonBoard
                        {
                            IsFilled = true,
                            BorderWidth = poly.width,
                            layerId = layer,
                            vertices = GetOutlineFromPoly(poly.vertices).Select(v => new Vertex { x = v.X, y = v.Y }).ToList()
                        };
                    break;

                case via via:
                    primitive = new Via
                    {
                        x = ToCanvasX(via.x),
                        y = ToCanvasY(via.y),
                        drill = via.drill,
                        diameter = via.diameter,
                        startLayer = LayerConstants.SignalTopLayerId,//we could get this from via.extent property
                        endLayer = LayerConstants.SignalBottomLayerId,
                    };
                    break;

                case wire wire:
                    {

                        var l = GetFootprintLayerIdMapping(wire.layer);
                        primitive = new TrackBoard
                        {
                            layerId = l,
                            width = wire.width,
                            Points = new List<Vertex>{
                                new Vertex{ x=ToCanvasX(wire.x1), y=ToCanvasY(wire.y1)},
                                new Vertex{ x=ToCanvasX(wire.x2), y=ToCanvasY(wire.y2)},
                                 }
                        };
                    }
                    break;
            }

            return primitive;
        }

        int GetFootprintLayerIdMapping(int sourceLayerId)
        {
            var fm = footprintLayerMapping.FirstOrDefault(f => f.SourceLayerId == sourceLayerId);
            if (fm != null)
                return fm.DestLayerId;

            return 0;
        }

        //  int pinNumber = 1;
        //int padNumber = 1;

        SchematicPrimitive GetSchematicPrimitive(object item)
        {
            SchematicPrimitive primitive = null;

            if (item is circle)
            {
                var circle = item as circle;
                primitive = new Circle
                {
                    x = ToCanvasX(circle.x),
                    y = ToCanvasY(circle.y),
                    BorderWidth = circle.width,
                    Diameter = circle.radius * 2,
                };
            }
            else if (item is pin)
            {
                var pin = item as pin;
                primitive = new Pin
                {
                    Name = pin.name,
                    Number = pin.name,
                    PinLength = 2.54 * (int)pin.length,
                    Orientation = GetPinOrientation(pin.rot),
                    x = ToCanvasX(pin.x),
                    y = ToCanvasY(pin.y),
                    ShowName = pin.visible != pinVisible.off,
                    ShowNumber = pin.visible == pinVisible.pin
                };
            }
            else if (item is polygon)
            {
                var poly = item as polygon;
                primitive = new Polygon
                {
                    BorderWidth = poly.width,
                    FillColor = "#FF000080",// most of the time, polys are solid fill
                    vertices = poly.vertices?.Select(v => new Vertex
                    {
                        x = ToCanvasX(v.x),
                        y = ToCanvasY(v.y)
                    }).ToList()
                };

            }
            else if (item is rectangle)
            {
                var r = item as rectangle;
                var rr = new XRect(ToCanvas(new XPoint(r.x1, r.y1)), ToCanvas(new XPoint(r.x2, r.y2)));
                var rot = GetRotFromString(r.rot);
                var width = rr.Width;
                var height = rr.Height;

                var center = rr.GetCenter();

                primitive = new Rectangle
                {
                    X = center.X,
                    Y = center.Y,
                    Width = width,
                    Height = height,
                    FillColor = "#FF000080",
                    BorderWidth = 0,
                    RadiusX = 0,
                    RadiusY = 0,
                    Rot = rot
                };
            }
            else if (item is text)
            {
                //ignore the text until we have the right font, but...
                var t = item as text;
                if (t.Value != null && (t.Value.ToLower() == ">value" || t.Value.ToLower() == ">name"))
                    return null;

                //font size is in mm; we convert it to a font size that is 96 dpi
                var dpiSize = 96 * t.size / 25.4;

                primitive = new Text
                {
                    x = ToCanvasX(t.x),
                    y = ToCanvasY(t.y),
                    Value = t.Value,
                    rot = GetRotFromString(t.rot),
                    FontSize = dpiSize,
                    Width = double.NaN
                };
            }
            else if (item is wire)
            {
                var l = item as wire;

                var lineCap = LineCap.Round;

                if (l.cap == wireCap.flat)
                    lineCap = LineCap.Flat;

                if (l.curve != 0.00d)
                {
                    var startPoint = new XPoint(ToCanvasX(l.x1), ToCanvasY(l.y1));
                    var endPoint = new XPoint(ToCanvasX(l.x2), ToCanvasY(l.y2));

                    var w = l.width;
                    if (w == 0)
                        w = 0.2;

                    var radius = GetRadiusFromCurvature(l.curve, startPoint, endPoint);

                    //if it is positive, it's counterclockwise
                    primitive = new Arc
                    {
                        StartPoint = ToCanvas(new XPoint(l.x1, l.y1)),
                        EndPoint = ToCanvas(new XPoint(l.x2, l.y2)),
                        SweepDirection = l.curve > 0.0 ? XSweepDirection.Counterclockwise : XSweepDirection.Clockwise,
                        IsLargeArc = l.curve > 180.0d || l.curve < -180.0d,
                        Size = new XSize(radius, radius),
                        BorderWidth = w,
                        LineCap = lineCap
                    };
                }
                else
                {
                    primitive = new LineSchematic
                    {
                        x1 = ToCanvasX(l.x1),
                        y1 = ToCanvasY(l.y1),
                        x2 = ToCanvasX(l.x2),
                        y2 = ToCanvasY(l.y2),
                        LineCap = lineCap,
                        width = l.width,
                    };
                }

            }
            else if (item is junction)
            {
                var j = item as junction;

                primitive = new Junction
                {
                    x = ToCanvasX(j.x),
                    y = ToCanvasY(j.y)
                };
            }
            else if (item is label)
            {
                var l = item as label;

                primitive = new NetLabel
                {
                    x = ToCanvasX(l.x),
                    y = ToCanvasY(l.y)
                };
            }
            else if (item is pinref)
            {
                var p = item as pinref;

                primitive = new PinRef
                {
                    Pin = p.pin
                };
            }

            return primitive;
        }

        XRect GetRectFromSchematicPrimitive(SchematicPrimitive primitive)
        {
            switch (primitive)
            {
                case Circle c:
                    return new XRect(c.x - 0.5 * c.Diameter, c.y - 0.5 * c.Diameter, 0.5 * c.Diameter, 0.5 * c.Diameter);

                case Pin pin:
                    {
                        var rot = (double)pin.Orientation;
                        var rotMatrix = new XRotateTransform(rot);
                        var translateMatrix = new XTranslateTransform(pin.x, pin.y);

                        var startPoint = new XPoint();
                        var endPoint = new XPoint(pin.PinLength, 0);

                        var localTransform = new XTransformGroup();
                        //local rotation
                        localTransform.Children.Add(rotMatrix);
                        //local translation
                        localTransform.Children.Add(translateMatrix);

                        startPoint = localTransform.Transform(startPoint);
                        endPoint = localTransform.Transform(endPoint);

                        var halfWidth = 0.5 * pin.Width;
                        var w = pin.Width;
                        var r1 = new XRect(startPoint.X - halfWidth, startPoint.Y - halfWidth, w, w);
                        var r2 = new XRect(endPoint.X - halfWidth, endPoint.Y - halfWidth, w, w);
                        r1.Union(r2);

                        return r1;
                    }

                case Polygon poly:
                    {
                        var polyRect = XRect.Empty;

                        foreach (var p in poly.vertices)
                        {
                            polyRect.Union(new XPoint(p.x, p.y));
                        }

                        return polyRect;
                    }

                case Rectangle r:
                    return new XRect(r.X - 0.5 * r.Width, r.Y - 0.5 * r.Height, r.Width, r.Height);

                case Text text:
                    return new XRect(text.x, text.y, 5, 5);

                case LineSchematic line:
                    {
                        var lineRect = new XRect(new XPoint(line.x1, line.y1), new XPoint(line.x2, line.y2));
                        if (lineRect.Width == 0)
                            lineRect.Width = line.width;
                        if (lineRect.Height == 0)
                            lineRect.Height = line.width;

                        return lineRect;
                    }

                case NetWire wire:
                    {
                        var lineRect = XRect.Empty;// new XRect(new XPoint(wire.x1, wire.y1), new XPoint(wire.x2, wire.y2));
                        foreach (var p in wire.Points)
                        {
                            lineRect.Union(new XPoint(p.x, p.y));
                        }
                        if (lineRect.Width == 0)
                            lineRect.Width = wire.Width;
                        if (lineRect.Height == 0)
                            lineRect.Height = wire.Width;

                        return lineRect;
                    }

                case Arc arc:
                    {
                        var arcRect = new XRect(arc.StartPoint, arc.EndPoint);
                        if (arcRect.Width == 0)
                            arcRect.Width = arc.Size.Width;
                        if (arcRect.Height == 0)
                            arcRect.Height = arc.Size.Width;

                        return arcRect;
                    }
                case Junction j:
                    return new XRect(j.x, j.y, 1, 1);

                case NetLabel netLabel:
                    return new XRect(netLabel.x, netLabel.y, 1, 1);
            }

            return XRect.Empty;
        }

        XRect GetRectFromSymbol(Symbol symbol, XPoint position, bool pinsOnly = false)
        {
            var rect = XRect.Empty;

            var pins = symbol.Items.OfType<pin>().ToList();
            var boundItems = (pinsOnly && pins.Count > 0) ? pins.Cast<SchematicPrimitive>() : symbol.Items;
            foreach (var item in boundItems)
            {
                var itemRect = GetRectFromSchematicPrimitive(item);
                rect.Union(itemRect);
            }

            if (pinsOnly)
            {
                rect.X = position.X - rect.Width;
                rect.Y = position.Y - rect.Height;
            }

            return rect;
        }

        XRect GetRectFromSymbol(Symbol symbol)
        {
            var rect = XRect.Empty;

            var xOffset = double.MaxValue;
            var yOffset = double.MaxValue;
            var pins = symbol.Items.OfType<Pin>().ToList();
            var boundItems = (pins.Count > 0) ? pins.Cast<SchematicPrimitive>() : symbol.Items;
            foreach (var item in boundItems)
            {
                var itemRect = GetRectFromSchematicPrimitive(item);

                if (xOffset > itemRect.X)
                    xOffset = itemRect.X;
                if (yOffset > itemRect.Y)
                    yOffset = itemRect.Y;

                rect.Union(itemRect);
            }

            if (rect != XRect.Empty)
            {
                //rect.X = -xOffset - rect.Width * 0.5;
                //rect.Y = -yOffset - rect.Height * 0.5;

                //rect.X = -0.5 * rect.Width;
                //rect.Y = -0.5 * rect.Height;

                rect.X -= 25.4 * 2;
                rect.Y -= 25.4 * 2;
            }

            return rect;
        }

        NetSegmentItem GetSchematicNetPrimitive(object item)
        {
            NetSegmentItem primitive = null;

            if (item is wire)
            {
                var l = item as wire;
                primitive = new NetWire
                {
                    Points = new List<Vertex>
                    {
                        new Vertex{ x = ToCanvasX(l.x1), y = ToCanvasY(l.y1)},
                        new Vertex{ x = ToCanvasX(l.x2), y = ToCanvasY(l.y2)},
                    },
                    Width = l.width,
                };

            }
            else if (item is junction)
            {
                var j = item as junction;

                primitive = new Junction
                {
                    x = ToCanvasX(j.x),
                    y = ToCanvasY(j.y)
                };
            }
            else if (item is label)
            {
                var l = item as label;

                primitive = new NetLabel
                {
                    x = ToCanvasX(l.x),
                    y = ToCanvasY(l.y)
                };
            }
            else if (item is pinref)
            {
                var p = item as pinref;

                primitive = new PinRef
                {
                    Pin = p.pin
                };
            }

            return primitive;
        }

        TopLeftCoordinatesSystem topLeftCS = CoordinateSystems.TopLeft;
        CartezianCoordinatesSystem cartezianCS = CoordinateSystems.Cartezian;
        double ToCanvasX(double x)
        {
            return topLeftCS.ConvertValueFrom(x, Axis.X, cartezianCS);
        }

        double ToCanvasY(double y)
        {
            return topLeftCS.ConvertValueFrom(y, Axis.Y, cartezianCS);
        }

        XPoint ToCanvas(XPoint p)
        {
            var pp = topLeftCS.ConvertValueFrom(p, cartezianCS);
            return new XPoint(pp.X, pp.Y);
        }

        int GetRotFromString(string rot)
        {
            if (string.IsNullOrEmpty(rot))
                return 0;

            rot = rot.ToUpper().Replace("R", "").Replace("M", "");

            var r = 0;
            int.TryParse(rot, out r);
            return r;
        }

        double GetRadiusFromCurvature(double curvature, XPoint startPoint, XPoint endPoint)
        {
            var mp = new XPoint(0.5 * (startPoint.X + endPoint.X), 0.5 * (startPoint.Y + endPoint.Y));
            var angleRad = 0.5 * curvature * Math.PI / 180.0d;
            var middleSegmentLen = (endPoint - mp).Length;

            var radius = middleSegmentLen / Math.Sin(angleRad);

            return Math.Abs(radius);
        }

        pinOrientation GetPinOrientation(string rot)
        {
            var r = (360 - GetRotFromString(rot)) % 360;
            return (pinOrientation)r;
        }

        void ImportProject(string sourceFilePath, string destinationFolder)
        {

            var solutionFolder = destinationFolder;
            var slnName = Path.GetFileName(destinationFolder);
            var projName = Path.GetFileName(destinationFolder);
            var projFolder = Path.Combine(solutionFolder, projName);

            var footprints = new List<Footprint>();
            var symbols = new List<Symbol>();
            var components = new List<ComponentDocument>();

            var parts = new List<Part>();


            var schPath = Path.ChangeExtension(sourceFilePath, ".sch");

            var schDoc = ImportSchematic(projFolder, schPath, symbols, footprints, components, parts);

            ImportBoard(projFolder, sourceFilePath, symbols, footprints, components, parts, schDoc);

            #region Create Output


            var solDoc = new SolutionDocument();


            var projDoc = new ProjectDocument
            {
                OutputType = ProjectOutputType.Library
            };


            solDoc.Children.Add(new SolutionProjectItem { RelativePath = $"{projName}/{projName}.project" });


            Directory.CreateDirectory(projFolder);


            SaveSymbols(projFolder, symbols);

            SaveFootprints(projFolder, footprints);

            SaveComponents(projFolder, components);

            var slnpath = Path.Combine(solutionFolder, $"{slnName}.solution");
            XmlHelper.Save(solDoc, slnpath);

            var projPath = Path.Combine(projFolder, $"{projName}.project");
            XmlHelper.Save(projDoc, projPath);

            #endregion
        }

        private void SaveComponents(string projFolder, List<ComponentDocument> components)
        {
            var compsFolderPath = Path.Combine(projFolder, "Components");
            Directory.CreateDirectory(compsFolderPath);
            foreach (var comp in components)
            {
                var name = ensureFileNameSafe(comp.Name);
                var cmpPath = Path.Combine(compsFolderPath, $"{name}.component");
                XmlHelper.Save(comp, cmpPath);
            }
        }

        private void SaveFootprints(string projFolder, List<Footprint> footprints)
        {
            var footprintsFolderPath = Path.Combine(projFolder, "Footprints");
            Directory.CreateDirectory(footprintsFolderPath);
            foreach (var fpt in footprints)
            {
                var name = ensureFileNameSafe(fpt.Name);
                var fptPath = Path.Combine(footprintsFolderPath, $"{name}.footprint");
                XmlHelper.Save(fpt, fptPath);
            }
        }

        private void SaveSymbols(string projFolder, List<Symbol> symbols)
        {
            var symbolsFolderPath = Path.Combine(projFolder, "Symbols");
            Directory.CreateDirectory(symbolsFolderPath);
            foreach (var smb in symbols)
            {
                var name = ensureFileNameSafe(smb.Name);
                var smbPath = Path.Combine(symbolsFolderPath, $"{name}.symbol");
                XmlHelper.Save(smb, smbPath);
            }
        }

        SchematicDocument ImportSchematic(string projFolder, string schPath, List<Symbol> symbols, List<Footprint> footprints, List<ComponentDocument> components, List<Part> parts)
        {
            var docWidth = 297.0d;
            var docHeight = 210.0d;

            var schFile = XmlHelper.Load<eagle>(schPath);

            var sheets = new List<Sheet>();
            var dictSheetsAndRects = new Dictionary<Sheet, XRect>();

            //schematic
            if (schFile.drawing != null && schFile.drawing.Item != null)
            {
                var sch = schFile.drawing.Item as schematic;

                //libraries
                if (sch != null && sch.libraries != null && sch.libraries.Libraries != null)
                {
                    foreach (var lib in sch.libraries.Libraries)
                    {
                        ExtractLibrary(lib, footprints, symbols, components);
                    }
                }

                //sch parts
                if (sch.parts != null && sch.parts.Parts != null)
                {
                    foreach (var part in sch.parts.Parts)
                    {
                        var cName = GetComponentName(part.deviceset, part.device);
                        var cmp = components.FirstOrDefault(c => c.Name == cName);
                        if (cmp == null)
                            continue;

                        var newPart = new Part
                        {
                            Name = part.name,
                            Id = LibraryItem.GetNextId(),
                            Comment = part.value,
                            ComponentId = cmp.Id,
                            ComponentName = cmp.Name
                        };

                        parts.Add(newPart);
                    }
                }

                //sch sheets
                if (sch.sheets != null && sch.sheets.Sheets != null)
                {


                    foreach (var sheet in sch.sheets.Sheets)
                    {
                        var sheetRect = XRect.Empty;

                        var newSheet = new Sheet();
                        newSheet.Name = "Sheet";
                        sheets.Add(newSheet);

                        //plain items
                        if (sheet.plain != null && sheet.plain.Items != null)
                        {
                            foreach (var item in sheet.plain.Items)
                            {
                                var p = GetSchematicPrimitive(item);
                                if (p != null)
                                {
                                    newSheet.PlainItems.Add(p);

                                    var rect = GetRectFromSchematicPrimitive(p);
                                    sheetRect.Union(rect);
                                }
                            }
                        }

                        //instances
                        if (sheet.instances != null && sheet.instances.Instances != null)
                        {
                            foreach (var instance in sheet.instances.Instances)
                            {
                                var scaleX = 1.0d;
                                if (instance.rot != null && instance.rot.Contains("M"))
                                {
                                    scaleX = -1.0d;
                                }

                                var part = parts.FirstOrDefault(p => p.Name == instance.part);
                                if (part == null)
                                    continue;

                                var cmp = components.FirstOrDefault(c => c.Id == part.ComponentId);
                                var gate = cmp.Gates.FirstOrDefault(g => g.name == instance.gate);

                                if (gate == null)
                                    continue;

                                var newInstance = new Instance
                                {
                                    x = ToCanvasX(instance.x),
                                    y = ToCanvasY(instance.y),
                                    ScaleX = scaleX,
                                    Rot = scaleX * (360 - GetRotFromString(instance.rot)),
                                    Id = LibraryItem.GetNextId(),
                                    PartId = part.Id,
                                    GateId = gate.Id,
                                };

                                var symbol = symbols.FirstOrDefault(s => s.Library == gate.LibraryName && s.Id == gate.symbolId);
                                if (symbol != null)
                                {
                                    FixInstancePosition(newInstance, symbol);

                                    var smbRect = GetRectFromSymbol(symbol, new XPoint(newInstance.x, newInstance.y));
                                    sheetRect.Union(smbRect);
                                }

                                newSheet.Instances.Add(newInstance);
                            }
                        }

                        //nets
                        if (sheet.nets != null && sheet.nets.Nets != null)
                        {
                            foreach (var net in sheet.nets.Nets)
                            {
                                var newNet = new Net
                                {
                                    Name = net.name,
                                    Id = LibraryItem.GetNextId()
                                };
                                newSheet.Nets.Add(newNet);

                                if (net.Segments != null)
                                {
                                    foreach (var seg in net.Segments)
                                    {
                                        if (seg.Items != null)
                                        {
                                            foreach (var item in seg.Items)
                                            {
                                                var primitive = GetSchematicNetPrimitive(item);

                                                if (primitive != null)
                                                {
                                                    if (primitive is PinRef)
                                                    {
                                                        var pinref = primitive as PinRef;
                                                        var pref = (pinref)item;

                                                        var part = parts.FirstOrDefault(p => p.Name == pref.part);
                                                        if (part != null)
                                                        {

                                                            var cmp = components.FirstOrDefault(c => c.Id == part.ComponentId);
                                                            var gate = cmp.Gates.FirstOrDefault(g => g.name == pref.gate);

                                                            if (gate != null)
                                                            {
                                                                var instance = newSheet.Instances.FirstOrDefault(i => i.PartId == part.Id && i.GateId == gate.Id);

                                                                if (instance != null)
                                                                    pinref.PartInstanceId = instance.Id;
                                                            }
                                                        }
                                                    }

                                                    newNet.Items.Add((NetSegmentItem)primitive);

                                                    var rect = GetRectFromSchematicPrimitive(primitive);
                                                    sheetRect.Union(rect);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //buses? (later)

                        //our document size will have the largest between the size of all sheets
                        if (sheetRect.Width > docWidth)
                            docWidth = sheetRect.Width;
                        if (sheetRect.Height > docHeight)
                            docHeight = sheetRect.Height;

                        dictSheetsAndRects.Add(newSheet, sheetRect);
                    }
                }
            }

            TranslateSheets(dictSheetsAndRects);

            var schDoc = new SchematicDocument
            {
                DocumentWidth = docWidth,
                DocumentHeight = docHeight,
                Id = LibraryItem.GetNextId()
            };
            schDoc.Parts = parts;
            schDoc.Sheets = sheets;

            var schFolderPath = Path.Combine(projFolder, "Schematics");
            Directory.CreateDirectory(schFolderPath);
            var schName = ensureFileNameSafe("Schematic");
            var schDocPath = Path.Combine(schFolderPath, $"{schName}.schematic");
            XmlHelper.Save(schDoc, schDocPath);

            return schDoc;
        }

        private void FixInstancePosition(Instance newInstance, Symbol symbol)
        {
            var smbRect = GetRectFromSymbol(symbol);
            var center = smbRect.GetCenter();
            center = SnapHelper.SnapToGrid(center, schematicGridSize);

            center.X = -center.X;
            //center.Y = -center.Y;

            var t = new XTransformGroup();
            t.Children.Add(new XRotateTransform(newInstance.Rot));
            t.Children.Add(new XScaleTransform(newInstance.ScaleX, newInstance.ScaleY));
            center = t.Transform(center);

            //snap from accumulated errors
            center = SnapHelper.SnapToGrid(center, schematicGridSize);

            newInstance.x -= center.X;
            newInstance.y += center.Y;//>?
        }

        const double marginDoc = 2.54 * 4;
        const double schematicGridSize = 1.27d;
        void TranslateSheets(Dictionary<Sheet, XRect> dict)
        {
            foreach (var sheetKVP in dict)
            {
                var sheetrect = sheetKVP.Value;
                var sheet = sheetKVP.Key;
                var dx = 0d;
                var dy = 0d;
                //if (sheetrect.X < 0)
                //    dx = -sheetrect.X;
                //if (sheetrect.Y < 0)
                //    dy = -sheetrect.Y;

                var gridSize = 1.27d;
                dx = SnapHelper.SnapToGrid(-sheetrect.X, gridSize);// + marginDoc;
                dy = SnapHelper.SnapToGrid(-sheetrect.Y, gridSize); //+ marginDoc;



                if (dx == 0.0d && dy == 0.0d)
                    continue;

                foreach (var item in sheet.PlainItems)
                {
                    TranslateSchematicPrimitive(item, dx, dy);
                }
                foreach (var item in sheet.Instances)
                {
                    TranslateSchematicPrimitive(item, dx, dy);
                }
                foreach (var net in sheet.Nets)
                {
                    foreach (var item in net.Items)
                    {
                        TranslateSchematicPrimitive(item, dx, dy);
                    }
                }
            }
        }



        private void TranslateSchematicPrimitive(SchematicPrimitive item, double dx, double dy)
        {
            switch (item)
            {
                case Circle circle:
                    circle.x += dx;
                    circle.y += dy;
                    break;

                case Polygon poly:
                    foreach (var v in poly.vertices)
                    {
                        v.x += dx;
                        v.y += dy;
                    }
                    break;

                case Rectangle rect:
                    rect.X += dx;
                    rect.Y += dy;
                    break;

                case Text txt:
                    txt.x += dx;
                    txt.y += dy;
                    break;

                case Arc arc:
                    arc.StartPoint.Offset(dx, dy);
                    arc.EndPoint.Offset(dx, dy);
                    break;

                case LineSchematic line:
                    line.x1 += dx;
                    line.y1 += dy;
                    line.x2 += dx;
                    line.y2 += dy;
                    break;

                case NetWire netWire:
                    //netWire.x1 += dx;
                    //netWire.y1 += dy;
                    //netWire.x2 += dx;
                    //netWire.y2 += dy;
                    foreach (var v in netWire.Points)
                    {
                        v.x += dx;
                        v.y += dy;
                    }
                    break;

                case Junction j:
                    j.x += dx;
                    j.y += dy;
                    break;

                case NetLabel lbl:
                    lbl.x += dx;
                    lbl.y += dy;
                    break;

                case Instance instance:
                    instance.x += dx;
                    instance.y += dy;
                    break;

            }
        }

        void ImportBoard(string projFolder, string brdPath, List<Symbol> symbols, List<Footprint> footprints, List<ComponentDocument> components, List<Part> parts, SchematicDocument schDoc)
        {
            var brdDoc = new BoardDocument();

            var boardFile = XmlHelper.Load<eagle>(brdPath);
            //board
            if (boardFile.drawing != null && boardFile.drawing.Item != null)
            {
                var brd = boardFile.drawing.Item as board;

                //libraries
                if (brd != null && brd.libraries != null && brd.libraries.Libraries != null)
                {
                    foreach (var lib in brd.libraries.Libraries)
                    {
                        ExtractLibrary(lib, footprints, symbols, components);
                    }
                }

                //plain items
                if (brd.plain != null && brd.plain.Items != null)
                {
                    foreach (var item in brd.plain.Items)
                    {
                        var p = GetFootprintPrimitive(item);
                        if (p != null)
                        {
                            brdDoc.PlainItems.Add(p);
                        }
                    }
                }

                //elements -> parts (footprints)
                if (brd.elements != null && brd.elements.Elements != null)
                {
                    foreach (var element in brd.elements.Elements)
                    {
                        var schPart = parts.FirstOrDefault(p => p.Name == element.name);
                        if (schPart == null)
                            continue;

                        var footprint = footprints.FirstOrDefault(f => f.Name == element.package);
                        if (footprint == null)
                            continue;

                        var placement = FootprintPlacement.Top;
                        if (element.rot != null && element.rot.Contains("M"))
                            placement = FootprintPlacement.Bottom;

                        var newpart = new BoardComponentInstance
                        {
                            Id = LibraryItem.GetNextId(),
                            PartId = schPart.Id,
                            PartName = schPart.Name,
                            ComponentId = schPart.ComponentId,
                            FootprintId = footprint.Id,
                            Placement = placement,
                            rot = GetRotFromString(element.rot),
                            x = ToCanvasX(element.x),
                            y = ToCanvasY(element.y)
                        };
                        brdDoc.Components.Add(newpart);
                    }
                }

                //signals -> nets
                if (brd.signals != null && brd.signals.Signals != null)
                {
                    foreach (var signal in brd.signals.Signals)
                    {
                        var newNet = new BoardNet
                        {
                            Name = signal.name,
                            NetId = LibraryItem.GetNextId(),
                        };
                        brdDoc.Nets.Add(newNet);

                        if (signal.Items != null)
                        {
                            foreach (var item in signal.Items)
                            {
                                var p = GetFootprintNetPrimitive(item);
                                if (p != null)
                                {
                                    if (p is PadRef pref)
                                    {
                                        var brdpart = brdDoc.Components.FirstOrDefault(pp => pp.PartName == ((contactref)item).element);
                                        if (brdpart != null)
                                            pref.FootprintInstanceId = brdpart.Id;

                                        newNet.Pads.Add(pref);
                                    }
                                    else
                                    {
                                        newNet.Items.Add((LayerPrimitive)p);
                                    }

                                }
                            }
                        }
                    }
                }
            }

            brdDoc.SchematicReference.schematicId = schDoc.Id;
            brdDoc.SchematicReference.hintPath = $@"Schematics\Schematic.schematic";
            var brdFolderPath = Path.Combine(projFolder, "Boards");
            Directory.CreateDirectory(brdFolderPath);
            var brdDocPath = Path.Combine(brdFolderPath, "Board.board");
            XmlHelper.Save(brdDoc, brdDocPath);
        }

        string GetComponentName(string deviceSetName, string deviceName)
        {
            var cName = $"{deviceSetName}{deviceName}";
            if (deviceName.ToLower().StartsWith(deviceSetName))
                cName = deviceName;

            return cName;
        }


        class LayerMappingItem
        {
            public int SourceLayerId { get; set; }
            public string SourceLayerName { get; set; }

            public int DestLayerId { get; set; }

            public string DestLayerName { get; set; }
        }
    }
}
