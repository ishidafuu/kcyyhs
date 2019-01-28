using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	[Serializable]
	public class TimelineTrack
	{
		public static Action<OnTrackEvent> Emit;

		TimelineTrackInspector timelineTrackInspector_;

		[SerializeField]
		TimelineTrackModel model = new TimelineTrackModel();
		TimelineTrackViewModel viewModel = new TimelineTrackViewModel();

		TimelineTrackView view = new TimelineTrackView();
		TimelineTrackTack tack = new TimelineTrackTack();
		TimelineTrackSelectTack select = new TimelineTrackSelectTack();
		// TimelineTrackCreateTack create = new TimelineTrackCreateTack();

		public void SetModels()
		{
			view.SetModels(model, viewModel);
			tack.SetModels(view, model, viewModel, timelineTrackInspector_);
			select.SetModels(view, model, viewModel, timelineTrackInspector_);
			// create.SetModels(view, model, viewModel, timelineTrackInspector_);
		}

		public TimelineTrack()
		{
			SetModels();
			model.IsExistTimeline_ = true;
			var defaultHeight = (WindowSettings.TIMELINE_HEIGHT);
			viewModel.trackRect_ = new Rect(0, 0, 10, defaultHeight);
		}

		public TimelineTrack(int index, int timelineType, List<TackPoint> tackPoints)
		{
			SetModels();
			view.InitializeTextResource();

			model.IsExistTimeline_ = true;
			model.timelineId_ = WindowSettings.ID_HEADER_TIMELINE + Guid.NewGuid().ToString();
			model.index_ = index;
			model.timelineType_ = timelineType;
			model.tackPoints_ = new List<TackPoint>(tackPoints);

			// set initial track rect.
			var defaultHeight = (WindowSettings.TIMELINE_HEIGHT);
			viewModel.trackRect_ = new Rect(0, 0, 10, defaultHeight);
			view.ApplyTextureToTacks(index);
		}

		public TimelineTrack(int index, Dictionary<string, object> scoreTimelineDict, List<TackPoint> currentTacks)
		{
			SetModels();
			view.InitializeTextResource();

			model.IsExistTimeline_ = true;
			model.timelineId_ = WindowSettings.ID_HEADER_TIMELINE + Guid.NewGuid().ToString();
			model.index_ = index;
			model.timelineType_ = Convert.ToInt32(scoreTimelineDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TIMELINE_TYPE]);
			model.tackPoints_ = currentTacks;

			// set initial track rect.
			var defaultHeight = (WindowSettings.TIMELINE_HEIGHT);
			viewModel.trackRect_ = new Rect(0, 0, 10, defaultHeight);

			view.ApplyTextureToTacks(index);
		}

		public float Height()
		{
			return viewModel.trackRect_.height;
		}

		public int GetIndex()
		{
			return model.index_;
		}
		public void SetActive()
		{
			model.active_ = true;
			model.haveActiveTack_ = false;
			ApplyDataToInspector();
			Selection.activeObject = timelineTrackInspector_;
		}
		public void SetDeactive()
		{
			model.active_ = false;
			model.haveActiveTack_ = false;
		}

		public bool IsActive()
		{
			return model.active_;
		}

		public bool IsHaveActiveTackPoint()
		{
			return model.haveActiveTack_;
		}

		public bool ContainsActiveTack()
		{
			foreach (var tackPoint in model.tackPoints_)
			{
				if (tackPoint.IsActive())return true;
			}
			return false;
		}

		public bool ContainsTackById(string tackId)
		{
			foreach (var tackPoint in model.tackPoints_)
			{
				if (tackId == tackPoint.GetTackId())return true;
			}
			return false;
		}

		public void BeforeSave()
		{
			TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TIMELINE_BEFORESAVE, model.timelineId_));
		}

		public void Save()
		{
			TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TIMELINE_SAVE, model.timelineId_));
		}

		public void Deleted(bool isCancel)
		{
			model.IsExistTimeline_ = isCancel;
		}
		public void ApplyDataToInspector()
		{
			if (timelineTrackInspector_ == null)
				timelineTrackInspector_ = ScriptableObject.CreateInstance("TimelineTrackInspector")as TimelineTrackInspector;

			timelineTrackInspector_.UpdateTimelineTrack(this);

			foreach (var tackPoint in model.tackPoints_)
			{
				tackPoint.ApplyDataToInspector();
			}
		}
	}
}