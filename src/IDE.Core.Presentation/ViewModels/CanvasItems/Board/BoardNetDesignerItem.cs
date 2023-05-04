using IDE.Core.Interfaces;
using IDE.Core.Presentation.Utilities;
using IDE.Core.Spatial2D;
using IDE.Core.Storage;
using IDE.Core.Types.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace IDE.Core.Designers
{
    /*
NetSegmentDesignerItem (item belonging to a net): junction, label, netwire inherit from this
PinRefDesignerItem: NetSegmentDesignerItem (a ref item to a pin; added to the canvas but not displayed
NetDesignerItem (net)

BOARD
SignalPrimitiveDesignerItem (item belonging to a signal): poly, trace, via
PadRefDesignerItem:SignalPrimitiveDesignerItem (a ref item to a pad; added to the canvas but not displayed)
SignalDesignerItem (signal)
*/

    /// <summary>
    /// Net on a board
    /// </summary>
    public class BoardNetDesignerItem : BaseViewModel, IBoardNetDesignerItem
    {
        public BoardNetDesignerItem(IBoardDesigner board)
        {
            thisBoard = board;
        }

        private readonly IBoardDesigner thisBoard;


        /// <summary>
        ///id of the net.Id from schematic
        /// </summary>
        public string Id { get; set; }

        string name;

        /// <summary>
        /// net name from schematic
        /// </summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// the net class id this net belongs to
        /// </summary>
        public string ClassId { get; set; }

        bool isHighlighted;

        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set
            {
                isHighlighted = value;
                OnPropertyChanged(nameof(IsHighlighted));
            }
        }

        public bool IsNamed()
        {
            return name != null && !name.StartsWith("Net");
        }

        /// <summary>
        /// in this net all pads must be connected together (net list)
        /// </summary>
        public IList<IPadCanvasItem> Pads { get; set; } = new ObservableCollection<IPadCanvasItem>();

        /// <summary>
        /// canvas items that belong to this net (except pads
        /// </summary>
        public IList<ISelectableItem> Items { get; set; } = new ObservableCollection<ISelectableItem>();

        IEnumerable<IPadCanvasItem> GetPads()
        {
            return Pads.Where(p => p != null).DistinctBy(p => new { p.FootprintInstanceId, p.Number });
        }

        public IList<ISignalPrimitiveCanvasItem> GetAllNetItems()
        {
            return Items.Cast<ISignalPrimitiveCanvasItem>().Union(GetPads()).ToList();
        }

        public void HighlightNet(bool newHighlight)
        {
            if (IsHighlighted == newHighlight)
                return;

            IsHighlighted = newHighlight;

            thisBoard.OnPropertyChanged(nameof(thisBoard.HasHighlightedNets));
            thisBoard.OnHighlightChanged(thisBoard, EventArgs.Empty);
        }

        public override string ToString()
        {
            return Name;
        }

        /* void Optimize() - we need to use the optimization on placement or moving endpoints
         * an endpoint should stop at a pad or via; currently it creates a single line
        public void Optimize()
        {
            //we optimize net tracks

            //net tracks grouped by layer
            var trackGroups = Items.OfType<TrackBoardCanvasItem>()
                                       .Where(t => t.IsPlaced)
                                       .GroupBy(t => t.Layer);

            foreach (var tg in trackGroups)
            {
                var optimizedTracks = new List<TrackHelper>();
                var optimizationOccured = false;

#if DEBUG
                foreach (var track in tg)
                {
                    Debug.WriteLine(track.ToString());
                }
#endif

                var trackIndex = -1;

                foreach (var track in tg)
                {
                    trackIndex++;

                    if (optimizedTracks.Count > 0)
                    {
                        var merged = false;
                        foreach (var optTrack in optimizedTracks)
                        {
                            if (track.Width != optTrack.Width)
                                continue;

                            if (XPoinExtensions.PointsAreAlmostEqual(optTrack.EndPoint, track.StartPoint)
                                || XPoinExtensions.PointsAreAlmostEqual(optTrack.EndPoint, track.EndPoint))
                            {
                                optTrack.AppendPoints(track.Points);
                                merged = true;
                                break;
                            }
                            else if(XPoinExtensions.PointsAreAlmostEqual(optTrack.StartPoint, track.StartPoint)
                                || XPoinExtensions.PointsAreAlmostEqual(optTrack.StartPoint, track.EndPoint))
                            {
                                optTrack.PrependPoints(track.Points);
                                merged = true;
                                break;
                            }
                        }

                        if (!merged)
                        {
                            //new track
                            var newTrack = new TrackHelper(thisBoard);
                            newTrack.Width = track.Width;
                            newTrack.AppendPoints(track.Points);
                            optimizedTracks.Add(newTrack);
                        }
                        else
                        {
                            optimizationOccured |= true;
                        }
                    }
                    else
                    {
                        var newTrack = new TrackHelper(thisBoard);
                        newTrack.Width = track.Width;
                        newTrack.AppendPoints(track.Points);
                        optimizedTracks.Add(newTrack);
                    }
                }

                if (optimizationOccured)
                {
                    //remove items from canvas
                    var toRemove = tg.ToList();
                    var toAdd = new List<TrackBoardCanvasItem>();


                    var layer = tg.Key;

                    //add the new items on the same layer
                    foreach (var optTrack in optimizedTracks)
                    {
                        optTrack.Simplify();

                        if (optTrack.trackPoints.Count > 1)
                        {
                            var track = new TrackBoardCanvasItem()
                            {
                                LayerDocument = thisBoard,
                                //ParentObject = trackItem.ParentObject,
                                //Layer = layer,
                                Signal = this,
                                Width = optTrack.Width,
                                IsPlaced = true,
                            };
                            track.Points.Clear();
                            track.Points.AddRange(optTrack.trackPoints);

                            // if (track.Points.Count > 0)
                            toAdd.Add(track);
                        }

                    }

                    dispatcher.RunOnDispatcher(() =>
                    {
                        canvas.RemoveItems(toRemove);
                        toAdd.ForEach(t => t.Layer = layer);
                        canvas.AddItems(toAdd);
                    });
                }
            }
        }
        */

        /*RemoveLoops() - a method that is not working
         * we plan to implement loop removal on placement or moving endpoints
        public void RemoveLoops()
        {
            var removedTracks = false;

            //the track to keep is the last added track
            var trackToKeep = Items.OfType<TrackBoardCanvasItem>().Where(t => t.IsPlaced).LastOrDefault();
            TrackBoardCanvasItem trackToRemove = null;

            if (trackToKeep != null)
            {
                var trackList = Items.OfType<TrackBoardCanvasItem>()
                                    .Where(t => t != trackToKeep && t.Layer == trackToKeep.Layer && t.IsPlaced)
                                    .ToList();

                foreach (var track in trackList)
                {
                    if ((XPoinExtensions.PointsAreAlmostEqual(trackToKeep.StartPoint, track.StartPoint)
                        || XPoinExtensions.PointsAreAlmostEqual(trackToKeep.StartPoint, track.EndPoint))
                            && (XPoinExtensions.PointsAreAlmostEqual(trackToKeep.EndPoint, track.StartPoint)
                        || XPoinExtensions.PointsAreAlmostEqual(trackToKeep.EndPoint, track.EndPoint)))
                    {
                        trackToRemove = track;
                        break;
                    }
                }
            }
            else
            {
            }

            if (trackToRemove != null)
            {
                canvas.RemoveItem(trackToRemove);
                removedTracks = true;
            }

            if (removedTracks)
            {
                Optimize();
            }
        }
        */
    }




}
