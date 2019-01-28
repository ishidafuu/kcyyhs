using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
    public class TimelineTrackSelectTack
    {
        TimelineTrackView view;
        TimelineTrackModel model;
        TimelineTrackViewModel viewModel;
        TimelineTrackInspector timelineTrackInspector;

        public void SetModels(TimelineTrackView view, TimelineTrackModel model, TimelineTrackViewModel viewModel, TimelineTrackInspector timelineTrackInspector)
        {
            this.view = view;
            this.timelineTrackInspector = timelineTrackInspector;
            this.model = model;
            this.viewModel = viewModel;
        }

        public void SelectPreviousTackOf(string tackId)
        {
            var cursoredTackIndex = model.tackPoints_.FindIndex(tack => tack.GetTackId() == tackId);

            if (cursoredTackIndex == 0)
            {
                TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, model.timelineId_));
                return;
            }

            var currentExistTacks = model.tackPoints_.Where(tack => tack.GetIsExistTack()).OrderByDescending(tack => tack.GetStart()).ToList();
            var currentTackIndex = currentExistTacks.FindIndex(tack => tack.GetTackId() == tackId);

            if (0 <= currentTackIndex && currentTackIndex < currentExistTacks.Count - 1)
            {
                var nextTack = currentExistTacks[currentTackIndex + 1];
                TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, nextTack.GetTackId()));
            }
        }

        public void SelectNextTackOf(string tackId)
        {
            var currentExistTacks = model.tackPoints_.Where(tack => tack.GetIsExistTack()).OrderBy(tack => tack.GetStart()).ToList();
            var currentTackIndex = currentExistTacks.FindIndex(tack => tack.GetTackId() == tackId);

            if (0 <= currentTackIndex && currentTackIndex < currentExistTacks.Count - 1)
            {
                var nextTack = currentExistTacks[currentTackIndex + 1];
                TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, nextTack.GetTackId()));
            }
        }

        public void SelectDefaultTackOrSelectTimeline()
        {
            if (model.tackPoints_.Any())
            {
                var firstTackPoint = model.tackPoints_[0];
                TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, firstTackPoint.GetTackId()));
                return;
            }

            TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, model.timelineId_));
        }

    }
}