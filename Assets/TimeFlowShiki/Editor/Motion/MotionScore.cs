using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

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

		public static MotionScore CreateMotionScore(string id,  List<MotionTimeline> timelineTracks)
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
			if (timeline == null) return null;

			return timeline.GetSelectedFrameAndPrevPos(selectedFrame);
		}

		public MotionTransform GetLatestTransform(int selectedFrame)
		{
			//Transformのタイムライン
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_TRANSFORM)
				.FirstOrDefault();

			if (timeline == null) return new MotionTransform();

			var latestTack = timeline.GetLatestTransform(selectedFrame);

			if (latestTack == null) return new MotionTransform();

			return latestTack.data;
		}

		////選択フレームのタック
		//public MotionTackAtari GetSelectedFrameAtari(int selectedFrame)
		//{
		//	var timeline = timelineTracks
		//		.Where(t => t.timelineType == (int)TimelineType.TL_ATARI)
		//		.FirstOrDefault();

		//	if (timeline == null) return null;

		//	return timeline.GetSelectedFrameAtari(selectedFrame);
		//}
		////選択フレームのタック
		//public MotionTackThrow GetSelectedFrameThrow(int selectedFrame)
		//{
		//	var timeline = timelineTracks
		//		.Where(t => t.timelineType == (int)TimelineType.TL_THROW)
		//		.FirstOrDefault();

		//	if (timeline == null) return null;

		//	return timeline.GetSelectedFrameThrow(selectedFrame);
		//}
		//選択フレームのタック
		public MotionTackMove GetSelectedFrameMove(int selectedFrame)
		{
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_MOVE)
				.FirstOrDefault();

			if (timeline == null) return null;

			return timeline.GetSelectedFrameMove(selectedFrame);
		}
		//選択フレームのタック
		public MotionTackColor GetSelectedFrameColor(int selectedFrame)
		{
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_COLOR)
				.FirstOrDefault();

			if (timeline == null) return null;

			return timeline.GetSelectedFrameColor(selectedFrame);
		}
		//選択フレームのタック
		public MotionTackEffect GetSelectedFrameEffect(int selectedFrame)
		{
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_EFFECT)
				.FirstOrDefault();

			if (timeline == null) return null;

			return timeline.GetSelectedFrameEffect(selectedFrame);
		}
		//選択フレームのタック
		public MotionTackPassive GetSelectedFramePassive(int selectedFrame)
		{
			var timeline = timelineTracks
				.Where(t => t.timelineType == (int)TimelineType.TL_PASSIVE)
				.FirstOrDefault();

			if (timeline == null) return null;

			return timeline.GetSelectedFramePassive(selectedFrame);
		}

	}

	[Serializable]
	public class MotionTimeline
	{
		public int timelineType;

		//各種タイムライン
		public List<MotionTackPos> motionPos = new List<MotionTackPos>();
		public List<MotionTackTransform> motionTransform = new List<MotionTackTransform>();
		public List<MotionTackMove> motionMove = new List<MotionTackMove>();
		public List<MotionTackColor> motionColor = new List<MotionTackColor>();
		public List<MotionTackEffect> motionEffect = new List<MotionTackEffect>();
		public List<MotionTackPassive> motionPassive = new List<MotionTackPassive>();

		public MotionTimeline(int timelineType, List<MotionTackPos> motionPos)
		{
			this.timelineType = timelineType;
			this.motionPos = motionPos;
		}

		public MotionTimeline(int timelineType, List<MotionTackMove> motionMove)
		{
			this.timelineType = timelineType;
			this.motionMove = motionMove;
		}

		public MotionTimeline(int timelineType, List<MotionTackTransform> motionTransform)
		{
			this.timelineType = timelineType;
			this.motionTransform = motionTransform;
		}

		//public MotionTimeline(int timelineType, List<MotionTackAtari> motionAtari)
		//{
		//	this.timelineType = timelineType;
		//	this.motionAtari = motionAtari;
		//}

		//public MotionTimeline(int timelineType, List<MotionTackHold> motionHold)
		//{
		//	this.timelineType = timelineType;
		//	this.motionHold = motionHold;
		//}

		//public MotionTimeline(int timelineType, List<MotionTackThrow> motionThrow)
		//{
		//	this.timelineType = timelineType;
		//	this.motionThrow = motionThrow;
		//}

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

		public MotionTimeline(int timelineType, List<MotionTackPassive> motionPassive)
		{
			this.timelineType = timelineType;
			this.motionPassive = motionPassive;
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
			if (nextTack != null) res.Add(nextTack);
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

		////現在のフレームのタック
		//public MotionTackAtari GetSelectedFrameAtari(int selectedFrame)
		//{
		//	var res = motionAtari
		//		.Where(t => (t.start <= selectedFrame))
		//		.Where(t => ((t.start + t.span) > selectedFrame))
		//		.FirstOrDefault();

		//	return res;
		//}

		////現在のフレームのタック
		//public MotionTackThrow GetSelectedFrameThrow(int selectedFrame)
		//{
		//	var res = motionThrow
		//		.Where(t => (t.start <= selectedFrame))
		//		.Where(t => ((t.start + t.span) > selectedFrame))
		//		.FirstOrDefault();

		//	return res;
		//}

		//現在のフレームのタック
		public MotionTackMove GetSelectedFrameMove(int selectedFrame)
		{
			var res = motionMove
				.Where(t => (t.start <= selectedFrame))
				.Where(t => ((t.start + t.span) > selectedFrame))
				.FirstOrDefault();

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

		//現在のフレームのタック
		public MotionTackPassive GetSelectedFramePassive(int selectedFrame)
		{
			var res = motionPassive
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
	public class MotionTackMove : MotionTack
	{
		public MotionMove data;

		public MotionTackMove(int start, int span, MotionMove data) : base(start, span)
		{
			this.data = data;
		}
	}

	//[Serializable]
	//public class MotionTackAtari : MotionTack
	//{
	//	public MotionAtari data;

	//	public MotionTackAtari(int start, int span, MotionAtari data) : base(start, span)
	//	{
	//		this.data = data;
	//	}
	//}

	//[Serializable]
	//public class MotionTackHold : MotionTack
	//{
	//	public MotionHold data;

	//	public MotionTackHold(int start, int span, MotionHold data) : base(start, span)
	//	{
	//		this.data = data;
	//	}
	//}

	//[Serializable]
	//public class MotionTackThrow : MotionTack
	//{
	//	public MotionThrow data;

	//	public MotionTackThrow(int start, int span, MotionThrow data) : base(start, span)
	//	{
	//		this.data = data;
	//	}
	//}

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

	[Serializable]
	public class MotionTackPassive : MotionTack
	{
		public MotionPassive data;

		public MotionTackPassive(int start, int span, MotionPassive data) : base(start, span)
		{
			this.data = data;
		}
	}
}