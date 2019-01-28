using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assertions;

namespace NKKD.EDIT
{

	[Serializable]
	public class MotionScore
	{
		public List<MotionTimeline> timelineTracks;
		public string id;
		public void Setup(string id, List<MotionTimeline> timelineTracks)
		{
			this.id = id;
			this.timelineTracks = timelineTracks;
		}

		public static MotionScore CreateMotionScore(string id, List<MotionTimeline> timelineTracks)
		{
			MotionScore res = new MotionScore();
			res.Setup(id, timelineTracks);
			return res;
		}

		public List<MotionTackPos> GetSelectedFrameAndPrevPos(int selectedFrame)
		{
			//トランスフォームのタイムライン
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_POS)
				.FirstOrDefault();
			if (timeline == null)return null;

			return timeline.GetSelectedFrameAndPrevPos(selectedFrame);
		}

		public MotionTransform GetLatestTransform(int selectedFrame)
		{
			//Transformのタイムライン
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_TRANSFORM)
				.FirstOrDefault();

			if (timeline == null)return new MotionTransform();

			var latestTack = timeline.GetLatestTransform(selectedFrame);

			if (latestTack == null)return new MotionTransform();

			return latestTack.data;
		}
		//選択フレームのタック
		public MotionTackColor GetSelectedFrameColor(int selectedFrame)
		{
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_COLOR)
				.FirstOrDefault();

			if (timeline == null)return null;

			return timeline.GetSelectedFrameColor(selectedFrame);
		}
		//選択フレームのタック
		public MotionTackEffect GetSelectedFrameEffect(int selectedFrame)
		{
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_EFFECT)
				.FirstOrDefault();

			if (timeline == null)return null;

			return timeline.GetSelectedFrameEffect(selectedFrame);
		}

	}

	[Serializable]
	public class MotionTimeline
	{
		public int timelineType;

		//各種タイムライン
		public List<MotionTackPos> motionPos = new List<MotionTackPos>();
		public List<MotionTackTransform> motionTransform = new List<MotionTackTransform>();
		public List<MotionTackColor> motionColor = new List<MotionTackColor>();
		public List<MotionTackEffect> motionEffect = new List<MotionTackEffect>();

		public MotionTimeline(int timelineType, List<MotionTackPos> motionPos)
		{
			this.timelineType = timelineType;
			this.motionPos = motionPos;
		}

		public MotionTimeline(int timelineType, List<MotionTackTransform> motionTransform)
		{
			this.timelineType = timelineType;
			this.motionTransform = motionTransform;
		}

		public MotionTimeline(int timelineType, List<MotionTackColor> motionColor)
		{
			this.timelineType = timelineType;
			this.motionColor = motionColor;
		}

		public MotionTimeline(int timelineType, List<MotionTackEffect> motionEvent)
		{
			this.timelineType = timelineType;
			this.motionEffect = motionEvent;
		}

		//現在のフレームのタックと、一つ前のタック
		public List<MotionTackPos> GetSelectedFrameAndPrevPos(int selectedFrame)
		{
			List<MotionTackPos> res = new List<MotionTackPos>();
			var selectedTack = motionPos
				.Where(t => (t.start <= selectedFrame) && ((t.start + t.span) > selectedFrame))
				.OrderBy(t => t.start)
				.FirstOrDefault();
			if (selectedTack == null)
			{
				return res;
			}
			res.Add(selectedTack);
			var nextTack = motionPos
				.Where(t => t.start < selectedTack.start)
				.OrderBy(t => t.start)
				.LastOrDefault();
			if (nextTack != null)res.Add(nextTack);
			return res;
		}

		//選択フレームに最も近い最後尾のタック
		public MotionTackTransform GetLatestTransform(int selectedFrame)
		{
			var res = motionTransform
				.Where(t => (t.start <= selectedFrame))
				.OrderBy(t => t.start)
				.LastOrDefault();

			return res;
		}

		//現在のフレームのタック
		public MotionTackColor GetSelectedFrameColor(int selectedFrame)
		{
			var res = motionColor
				.Where(t => (t.start <= selectedFrame))
				.Where(t => ((t.start + t.span) > selectedFrame))
				.FirstOrDefault();

			return res;
		}

		//現在のフレームのタック
		public MotionTackEffect GetSelectedFrameEffect(int selectedFrame)
		{
			var res = motionEffect
				.Where(t => (t.start <= selectedFrame))
				.Where(t => ((t.start + t.span) > selectedFrame))
				.FirstOrDefault();

			return res;
		}

	}

	//各種タイムライン
	[Serializable]
	public class MotionTack
	{
		public int start;
		public int span;
		public MotionTack()
		{
			this.start = 0;
			this.span = 1;
		}
		public MotionTack(int start, int span)
		{
			this.start = start;
			this.span = span;
		}
	}

	[Serializable]
	public class MotionTackPos : MotionTack
	{
		public MotionPos data;

		public MotionTackPos(int start, int span, MotionPos data) : base(start, span)
		{
			this.data = data;
		}
		public MotionTackPos()
		{
			this.data = new MotionPos();
		}
	}

	[Serializable]
	public class MotionTackTransform : MotionTack
	{
		public MotionTransform data;

		public MotionTackTransform(int start, int span, MotionTransform data) : base(start, span)
		{
			this.data = data;
		}
		public MotionTackTransform()
		{
			this.data = new MotionTransform();
		}
	}

	[Serializable]
	public class MotionTackColor : MotionTack
	{
		public MotionColor data;

		public MotionTackColor(int start, int span, MotionColor data) : base(start, span)
		{
			this.data = data;
		}
	}

	[Serializable]
	public class MotionTackEffect : MotionTack
	{
		public MotionEffect data;

		public MotionTackEffect(int start, int span, MotionEffect data) : base(start, span)
		{
			this.data = data;
		}
	}
}