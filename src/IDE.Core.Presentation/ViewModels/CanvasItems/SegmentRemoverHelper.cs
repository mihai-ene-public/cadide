using System;
using IDE.Core.Interfaces;
using IDE.Core.Types.Media;
using System.Collections.Generic;
using System.Linq;
using IDE.Core.Presentation.Utilities;
using System.Threading.Tasks;
using IDE.Core.Routing.PathFinding;
using IDE.Core.Storage;

namespace IDE.Core.Designers;

public abstract class SegmentRemoverHelper<T> : ISegmentRemoverHelper where T : ISegmentedPolylineSelectableCanvasItem, new()
{
    protected readonly IDispatcherHelper _dispatcher;

    public SegmentRemoverHelper(IDispatcherHelper dispatcher)
    {
        _dispatcher = dispatcher;
    }

    protected abstract T CreateAnotherItem(T canvasItem);

    protected abstract void RunRemoveBehavior(T canvasItem, IList<T> newTracks, ICanvasDesignerFileViewModel canvasModel);

    protected abstract object GetSignal(T canvasItem);
    protected abstract void SetSignal(T canvasItem, object signal);
    protected virtual void SetSignalOther(object canvasItem, object signal)
    {

    }
    protected virtual object GetSignalOther(object canvasItem)
    {
        return null;
    }

    protected virtual IList<ISelectableItem> GetOtherAffectedItems() { return new List<ISelectableItem>(); }

    public void RemoveSelectedSegments(ICanvasDesignerFileViewModel canvasModel, ISegmentedPolylineSelectableCanvasItem segmentedItem)
    {
        var canvasItem = (T)segmentedItem;
        if (segmentedItem.HasSelectedSegments())
        {
            var originalPoints = segmentedItem.Points.ToList();
            var originalSignal = GetSignal(canvasItem);

            //consider we have to tracks and rebuild points
            //if we have a track from the points then we build another item
            var track1Points = new List<XPoint>();
            for (int pIndex = 0; pIndex <= segmentedItem.SelectedSegmentStart; pIndex++)
            {
                track1Points.Add(segmentedItem.Points[pIndex]);
            }

            var track2Points = new List<XPoint>();
            for (int pIndex = segmentedItem.SelectedSegmentEnd + 1; pIndex < segmentedItem.Points.Count; pIndex++)
            {
                track2Points.Add(segmentedItem.Points[pIndex]);
            }

            var newTracks = new List<T>();

            if (track1Points.Count > 1)
            {
                var track1 = CreateAnotherItem(canvasItem);
                track1.Points = track1Points;
                newTracks.Add(track1);
            }

            if (track2Points.Count > 1)
            {
                var track2 = CreateAnotherItem(canvasItem);
                track2.Points = track2Points;
                newTracks.Add(track2);
            }

            RunRemoveBehavior(canvasItem, newTracks, canvasModel);
            var newSignals = new List<object>();
            foreach (var track in newTracks)
            {
                newSignals.Add(GetSignal(track));
            }

            canvasModel.ClearSelectedItems();
            canvasModel.UpdateSelection();

            //this is an workaround for a refresh issue
            //it happens that when you delete a segment in the middle the first portion is not visible; only the second part of the segment is
            //it exists on the canvas since if you save then reload all segments are where they're supposed to be
            _dispatcher.RunOnDispatcher(async () =>
            {
                canvasModel.RemoveItem(segmentedItem);

                foreach (ISelectableItem track in newTracks)
                {
                    canvasModel.AddItem(track);
                    await Task.Delay(10);
                }

            });

            var currentNewTracks = newTracks.ToList();
            var currentOtherAffectedItems = GetOtherAffectedItems().ToList();
            var currentOtherNewSignals = currentOtherAffectedItems.Select(s => GetSignalOther(s)).ToList();

            canvasModel.RegisterUndoActionExecuted(undo: o =>
            {
                canvasModel.RemoveItems(currentNewTracks.Cast<ISelectableItem>());

                segmentedItem.Points = originalPoints;
                SetSignal(canvasItem, originalSignal);
                foreach (var affectedItem in currentOtherAffectedItems)
                {
                    SetSignalOther(affectedItem, originalSignal);
                }

                canvasModel.AddItem(segmentedItem);
                return null;
            },
            redo: o =>
            {
                canvasModel.RemoveItem(segmentedItem);

                for (int i = 0; i < currentNewTracks.Count; i++)
                {
                    SetSignal(currentNewTracks[i], newSignals[i]);
                }
                for (int i = 0; i < currentOtherAffectedItems.Count; i++)
                {
                    SetSignalOther(currentOtherAffectedItems[i], currentOtherNewSignals[i]);
                }

                canvasModel.AddItems(currentNewTracks.Cast<ISelectableItem>());
                return null;
            }, null);
        }
    }
}
