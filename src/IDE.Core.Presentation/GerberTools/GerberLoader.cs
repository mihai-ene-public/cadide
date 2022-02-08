using GerberLibrary;
using GerberLibrary.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDE.Core.Presentation.GerberTools
{
    //currently this is just an example on how to use the loading of the gerber files
    //we will do this right when we actually implement the gerber viewer
    public class GerberLoader
    {

        public void LoadGerberFiles(IList<string> files)
        {
            var parsedGerbers = GetParsedGerbers(files);

            var outlineBoundingBox = GetOutlineBoundingBox(parsedGerbers);

        }

        List<ParsedGerber> GetParsedGerbers(IList<string> files)
        {
            var listParsedGerbers = new List<ParsedGerber>();

            foreach (var file in files)
            {
                var fileType = GerberLibrary.Gerber.FindFileType(file);
                var forcezerowidth = true;
                var precombinepolygons = false;
                ParsedGerber parsedGerber = null;
                var boardState = new GerberParserState() { PreCombinePolygons = precombinepolygons };

                switch (fileType)
                {
                    case BoardFileType.Drill:
                        {
                            parsedGerber = PolyLineSet.LoadExcellonDrillFile(file);
                            parsedGerber.Side = BoardSide.Both;
                            parsedGerber.Layer = BoardLayer.Drill;

                            break;
                        }

                    case BoardFileType.Gerber:
                        {
                            var boardSide = BoardSide.Unknown;
                            var boardLayer = BoardLayer.Unknown;
                            GerberLibrary.Gerber.DetermineBoardSideAndLayer(file, out boardSide, out boardLayer);

                            if (boardLayer == BoardLayer.Outline || boardLayer == BoardLayer.Mill)
                            {
                                forcezerowidth = true;
                                precombinepolygons = true;
                            }
                            boardState.PreCombinePolygons = precombinepolygons;
                            if (boardLayer == BoardLayer.Silk)
                            {
                                boardState.IgnoreZeroWidth = true;
                            }
                            parsedGerber = PolyLineSet.LoadGerberFile(file, forcezerowidth, false, boardState);
                            parsedGerber.Side = boardState.Side;
                            parsedGerber.Layer = boardState.Layer;
                            if (boardLayer == BoardLayer.Outline)
                            {
                                parsedGerber.FixPolygonWindings();
                            }

                            break;
                        }
                }

                parsedGerber.CalcPathBounds();

                listParsedGerbers.Add(parsedGerber);
            }

            return listParsedGerbers;
        }

        Bounds GetOutlineBoundingBox(List<ParsedGerber> parsedGerbers)
        {
            var bounds = new Bounds();
            foreach (var parsedGerber in parsedGerbers)
            {
                if (parsedGerber.Layer == BoardLayer.Mill || parsedGerber.Layer == BoardLayer.Outline)
                {
                    bounds.AddBox(parsedGerber.BoundingBox);
                }
            }
            return bounds;
        }
    }
}
