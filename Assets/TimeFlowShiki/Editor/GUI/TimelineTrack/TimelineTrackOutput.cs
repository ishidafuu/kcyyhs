using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class TimelineTrackOutput
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

		public Dictionary<string, object> OutputDict(List<object> tackList)
		{
			var res = new Dictionary<string, object>
				{ { TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TIMELINE_TYPE, model.timelineType_ },
					{ TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TIMELINE_TACKS, tackList }
				};

			return res;
		}

		public MotionTimeline OutputTimelineObject()
		{
			MotionTimeline res = null;
			switch ((TimelineType)model.timelineType_)
			{
				case TimelineType.TL_POS:
					res = new MotionTimeline(model.timelineType_, ConvertMotionPos());
					break;
				case TimelineType.TL_TRANSFORM:
					res = new MotionTimeline(model.timelineType_, ConvertMotionTransform());
					break;
				case TimelineType.TL_COLOR:
					res = new MotionTimeline(model.timelineType_, ConvertMotionColor());
					break;
				case TimelineType.TL_EFFECT:
					res = new MotionTimeline(model.timelineType_, ConvertMotionEffect());
					break;
			}
			return res;
		}

		public void OutputAniScript(List<AniFrame> aniScriptFrames)
		{
			switch ((TimelineType)model.timelineType_)
			{
				case TimelineType.TL_POS:
					{
						List<MotionPosState> posList = GetIntermediatePos();
						foreach (var item in posList)
						{
							aniScriptFrames.Add(item.OutputFrame());
						}
					}
					break;
					//case TimelineType.TL_TRANSFORM: res = new MotionTimeline(model.timelineType_, ConvertMotionTransform()); break;
					//case TimelineType.TL_MOVE: res = new MotionTimeline(model.timelineType_, ConvertMotionMove()); break;
					////case TimelineType.TL_ATARI: res = new MotionTimeline(model.timelineType_, ConvertMotionAtari()); break;
					////case TimelineType.TL_HOLD: res = new MotionTimeline(model.timelineType_, ConvertMotionHold()); break;
					////case TimelineType.TL_THROW: res = new MotionTimeline(model.timelineType_, ConvertMotionThrow()); break;
					//case TimelineType.TL_COLOR: res = new MotionTimeline(model.timelineType_, ConvertMotionColor()); break;
					//case TimelineType.TL_EFFECT: res = new MotionTimeline(model.timelineType_, ConvertMotionEffect()); break;
					//case TimelineType.TL_PASSIVE: res = new MotionTimeline(model.timelineType_, ConvertMotionPassive()); break;
			}

		}

		List<MotionPosState> GetIntermediatePos()
		{
			List<MotionPosState> res = new List<MotionPosState>();
			int nowFrame = 0;
			for (int i = 0; i < model.tackPoints_.Count; i++)
			{
				var nowTack = model.tackPoints_[i];
				TackPoint prevTack = (i > 0)
					? model.tackPoints_[i - 1]
					: new TackPoint();

				for (int i2 = 0; i2 < nowTack.GetSpan(); i2++)
				{

					var intermediate = MotionPos.MakeIntermediate2(
						prevTack.GetMotionData().mPos,
						nowTack.GetMotionData().mPos,
						nowTack.GetStart(), nowTack.GetSpan(), nowFrame);
					res.Add(intermediate);
					nowFrame++;
				}

			}

			return res;
		}

		public List<MotionTackPos> ConvertMotionPos()
		{
			List<MotionTackPos> res = new List<MotionTackPos>();
			foreach (var item in model.tackPoints_)res.Add(item.output.OutputMotionTackPos());
			return res;
		}
		public List<MotionTackTransform> ConvertMotionTransform()
		{
			List<MotionTackTransform> res = new List<MotionTackTransform>();
			foreach (var item in model.tackPoints_)res.Add(item.output.OutputMotionTackTransform());
			return res;
		}
		public List<MotionTackColor> ConvertMotionColor()
		{
			List<MotionTackColor> res = new List<MotionTackColor>();
			foreach (var item in model.tackPoints_)res.Add(item.output.OutputMotionTackColor());
			return res;
		}
		public List<MotionTackEffect> ConvertMotionEffect()
		{
			List<MotionTackEffect> res = new List<MotionTackEffect>();
			foreach (var item in model.tackPoints_)res.Add(item.output.OutputMotionTackEffect());
			return res;
		}

	}
}