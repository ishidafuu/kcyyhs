using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	[Serializable]
	public class ScoreComponent
	{
		[SerializeField]
		private ScoreComponentInspector scoreComponentInspector;
		[SerializeField]
		public bool IsExistScore_;
		[SerializeField]
		private bool active_;
		[SerializeField]
		public List<TimelineTrack> timelineTracks_;
		[SerializeField]
		public string scoreGuid_;
		[SerializeField]
		public string id_;
		//[SerializeField]
		//public string title_;

		public static Action<OnTrackEvent> Emit;
		public ScoreComponent(string id, string title, List<TimelineTrack> timelineTracks)
		{
			this.IsExistScore_ = true;
			this.active_ = false;
			this.scoreGuid_ = WindowSettings.ID_HEADER_SCORE + Guid.NewGuid().ToString();
			this.id_ = id;
			//this.title_ = title;
			this.timelineTracks_ = new List<TimelineTrack>(timelineTracks);
		}

		public ScoreComponent(Dictionary<string, object> scoreDict, List<TimelineTrack> currentTimelines)
		{
			this.IsExistScore_ = true;
			this.active_ = false;
			this.scoreGuid_ = WindowSettings.ID_HEADER_SCORE + Guid.NewGuid().ToString();
			this.id_ = scoreDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_SCORE_ID] as string;
			//this.title_ = scoreDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_SCORE_TITLE] as string;
			this.timelineTracks_ = currentTimelines;
		}

		public Dictionary<string, object> OutputDict(List<object> timelineList)
		{
			var res = new Dictionary<string, object>
				{ { TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_SCORE_ID, this.id_ },
					//{TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_SCORE_TITLE, this.title_},
					{ TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_SCORE_TIMELINES, timelineList }
				};

			return res;
		}

		public MotionScore CreateMotionScoreObject()
		{
			List<MotionTimeline> timelineTracks = new List<MotionTimeline>();
			foreach (var item in this.timelineTracks_)
			{
				timelineTracks.Add(item.OutputTimelineObject());
			}
			MotionScore res = MotionScore.CreateMotionScore(this.id_, timelineTracks);
			//MotionScore res = new MotionScore(this.id, this.title, timelineTracks);
			return res;
		}

		public AniScript CreateAniScriptObject()
		{
			List<AniFrame> aniFrameList = new List<AniFrame>();
			foreach (var item in this.timelineTracks_)
			{
				item.OutputAniScript(aniFrameList);
			}
			AniScript res = AniScript.CreateAniScript(this.id_, aniFrameList);

			//MotionScore res = new MotionScore(this.id, this.title, timelineTracks);
			return res;
		}

		public bool IsActive()
		{
			return active_;
		}

		public void SetActive()
		{
			active_ = true;
			ApplyDataToInspector();
		}

		public void SetDeactive()
		{
			active_ = false;
		}

		public void DrawTimelines(ScoreComponent auto, float yOffsetPos, float xScrollIndex, float trackWidth)
		{
			var yIndex = yOffsetPos;

			for (var windowIndex = 0; windowIndex < timelineTracks_.Count; windowIndex++)
			{
				var timelineTrack = timelineTracks_[windowIndex];
				if (!timelineTrack.IsExistTimeline_)continue;

				var trackHeight = timelineTrack.DrawTimelineTrack(yOffsetPos, xScrollIndex, yIndex, trackWidth);

				// set next y index.
				yIndex = yIndex + trackHeight + WindowSettings.TIMELINE_SPAN;
			}
		}

		public float TimelinesTotalHeight()
		{
			var totalHeight = 0f;
			foreach (var timelineTrack in timelineTracks_)
			{
				totalHeight += timelineTrack.Height();
			}
			return totalHeight;
		}

		public List<TimelineTrack> TimelinesByIds(List<string> timelineIds)
		{
			var results = new List<TimelineTrack>();
			foreach (var timelineTrack in timelineTracks_)
			{
				if (timelineIds.Contains(timelineTrack.timelineId_))
				{
					results.Add(timelineTrack);
				}
			}
			return results;
		}

		public TimelineTrack TimelineById(string timelineId)
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				if (timelineTrack.timelineId_ == timelineId)
				{
					return timelineTrack;
				}
			}
			return null;
		}

		public TackPoint TackById(string tackId)
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				var tacks = timelineTrack.tackPoints_;
				foreach (var tack in tacks)
				{
					if (tack.tackId_ == tackId)
					{
						return tack;
					}
				}
			}
			return null;
		}

		public List<TackPoint> TimelinesByType(TimelineType timelineType)
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				if (timelineTrack.timelineType_ == (int)timelineType)
				{
					return timelineTrack.tackPoints_;
				}
			}
			return null;
		}

		public void SelectAboveObjectById(string currentActiveObjectId)
		{
			if (ARIMotionScoreWindow.IsTimelineId(currentActiveObjectId))
			{
				var candidateTimelines = timelineTracks_.Where(timeline => timeline.IsExistTimeline_).OrderBy(timeline => timeline.GetIndex()).ToList();
				var currentTimelineIndex = candidateTimelines.FindIndex(timeline => timeline.timelineId_ == currentActiveObjectId);

				if (0 < currentTimelineIndex)
				{
					var targetTimeline = timelineTracks_[currentTimelineIndex - 1];
					Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, targetTimeline.timelineId_));
					return;
				}

				return;
			}

			if (ARIMotionScoreWindow.IsTackId(currentActiveObjectId))
			{
				/*
					select another timeline's same position tack.
				*/
				var currentActiveTack = TackById(currentActiveObjectId);

				var currentActiveTackStart = currentActiveTack.start_;
				var currentTimelineId = currentActiveTack.parentTimelineId_;

				var aboveTimeline = AboveTimeline(currentTimelineId);
				if (aboveTimeline != null)
				{
					var nextActiveTacks = aboveTimeline.TacksByStart(currentActiveTackStart);
					if (nextActiveTacks.Any())
					{
						var targetTack = nextActiveTacks[0];
						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, targetTack.tackId_));
					}
					else
					{
						// no tack found, select timeline itself.
						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, aboveTimeline.timelineId_));
					}
				}
				return;
			}
		}

		public void SelectBelowObjectById(string currentActiveObjectId)
		{
			if (ARIMotionScoreWindow.IsTimelineId(currentActiveObjectId))
			{
				var cursoredTimelineIndex = timelineTracks_.FindIndex(timeline => timeline.timelineId_ == currentActiveObjectId);
				if (cursoredTimelineIndex < timelineTracks_.Count - 1)
				{
					var targetTimelineIndex = cursoredTimelineIndex + 1;
					var targetTimeline = timelineTracks_[targetTimelineIndex];
					Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, targetTimeline.timelineId_));
				}
				return;
			}

			if (ARIMotionScoreWindow.IsTackId(currentActiveObjectId))
			{
				/*
					select another timeline's same position tack.
				*/
				var currentActiveTack = TackById(currentActiveObjectId);

				var currentActiveTackStart = currentActiveTack.start_;
				var currentTimelineId = currentActiveTack.parentTimelineId_;

				var belowTimeline = BelowTimeline(currentTimelineId);
				if (belowTimeline != null)
				{
					var nextActiveTacks = belowTimeline.TacksByStart(currentActiveTackStart);
					if (nextActiveTacks.Any())
					{
						var targetTack = nextActiveTacks[0];
						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, targetTack.tackId_));
					}
					else
					{
						// no tack found, select timeline itself.
						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, belowTimeline.timelineId_));
					}
				}
				return;
			}
		}

		private TimelineTrack AboveTimeline(string baseTimelineId)
		{
			var baseIndex = timelineTracks_.FindIndex(timeline => timeline.timelineId_ == baseTimelineId);
			if (0 < baseIndex)return timelineTracks_[baseIndex - 1];
			return null;
		}

		private TimelineTrack BelowTimeline(string baseTimelineId)
		{
			var baseIndex = timelineTracks_.FindIndex(timeline => timeline.timelineId_ == baseTimelineId);
			if (baseIndex < timelineTracks_.Count - 1)return timelineTracks_[baseIndex + 1];
			return null;
		}

		public void SelectPreviousTackOfTimelines(string currentActiveObjectId)
		{
			if (ARIMotionScoreWindow.IsTackId(currentActiveObjectId))
			{
				foreach (var timelineTrack in timelineTracks_)
				{
					timelineTrack.SelectPreviousTackOf(currentActiveObjectId);
				}
			}
		}

		public void SelectNextTackOfTimelines(string currentActiveObjectId)
		{
			if (ARIMotionScoreWindow.IsTimelineId(currentActiveObjectId))
			{
				foreach (var timelineTrack in timelineTracks_)
				{
					if (timelineTrack.timelineId_ == currentActiveObjectId)
					{
						timelineTrack.SelectDefaultTackOrSelectTimeline();
					}
				}
				return;
			}

			if (ARIMotionScoreWindow.IsTackId(currentActiveObjectId))
			{
				foreach (var timelineTrack in timelineTracks_)
				{
					timelineTrack.SelectNextTackOf(currentActiveObjectId);
				}
			}
		}

		public bool IsActiveTimelineOrContainsActiveObject(int index)
		{
			if (index < timelineTracks_.Count)
			{
				var currentTimeline = timelineTracks_[index];
				if (currentTimeline.IsActive())return true;
				return currentTimeline.ContainsActiveTack();
			}
			return false;
		}

		public int GetStartFrameById(string objectId)
		{
			if (ARIMotionScoreWindow.IsTimelineId(objectId))
			{
				return -1;
			}

			if (ARIMotionScoreWindow.IsTackId(objectId))
			{
				var targetContainedTimelineIndex = GetTackContainedTimelineIndex(objectId);
				if (0 <= targetContainedTimelineIndex)
				{
					var foundStartFrame = timelineTracks_[targetContainedTimelineIndex].GetStartFrameById(objectId);
					if (0 <= foundStartFrame)return foundStartFrame;
				}
			}

			return -1;
		}

		public void SelectTackAtFrame(int frameCount)
		{
			if (timelineTracks_.Any())
			{
				var firstTimelineTrack = timelineTracks_[0];
				var nextActiveTacks = firstTimelineTrack.TacksByStart(frameCount);
				if (nextActiveTacks.Any())
				{
					var targetTack = nextActiveTacks[0];
					Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, targetTack.tackId_));
				}
				else
				{
					// no tack found, select timeline itself.
					Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, firstTimelineTrack.timelineId_));
				}
			}
		}

		public void DeactivateAllObjects()
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				timelineTrack.SetDeactive();
				timelineTrack.DeactivateTacks();
			}
		}

		public void SetMovingTackToTimelimes(string tackId)
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				if (timelineTrack.ContainsTackById(tackId))
				{
					timelineTrack.SetMovingTack(tackId);
				}
			}
		}

		/**
			set active to active objects, and set deactive to all other objects.
			affect to records of Undo/Redo.
		*/
		public void ActivateObjectsAndDeactivateOthers(List<string> activeObjectIds)
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				if (activeObjectIds.Contains(timelineTrack.timelineId_))
				{
					timelineTrack.SetActive();
				}
				else
				{
					timelineTrack.SetDeactive();
				}

				timelineTrack.ActivateTacks(activeObjectIds);
			}
		}

		public int GetTackContainedTimelineIndex(string tackId)
		{
			return timelineTracks_.FindIndex(timelineTrack => timelineTrack.ContainsTackById(tackId));
		}

		//public void AddNewTackToTimeline(string timelineId, int frame, TackPoint newTackPoint)
		//{
		//	var targetTimeline = TimelinesByIds(new List<string> { timelineId })[0];
		//	targetTimeline.AddNewTackToEmptyFrame(frame, targetTimeline.timelineType_, newTackPoint);
		//}

		public TackPoint NewTackToTimeline(string timelineId, int frame)
		{
			var targetTimeline = TimelinesByIds(new List<string> { timelineId })[0];
			return targetTimeline.NewTackToEmptyFrame(frame, targetTimeline.timelineType_);
		}

		public void PasteTackToTimeline(string timelineId, int frame, TackPoint clipTack)
		{
			if (clipTack != null)
			{
				var targetTimeline = TimelinesByIds(new List<string> { timelineId })[0];

				//targetTimeline.PasteTackToEmptyFrame(frame, clipTack);
				if (targetTimeline.timelineType_ == clipTack.timelineType_)
				{
					targetTimeline.PasteTackToEmptyFrame(frame, clipTack);
					//targetTimeline.AddNewTackToEmptyFrame(frame, clipTack);
				}
			}
		}

		public void DeleteObjectById(string deletedObjectId, bool isCancel)
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				if (ARIMotionScoreWindow.IsTimelineId(deletedObjectId))
				{
					if (timelineTrack.timelineId_ == deletedObjectId)
					{
						timelineTrack.Deleted(isCancel);
					}
				}
				if (ARIMotionScoreWindow.IsTackId(deletedObjectId))
				{
					timelineTrack.DeleteTackById(deletedObjectId, isCancel);
				}
			}
		}

		public TackPoint GetTackById(string selectObjectId)
		{
			TackPoint res = null;
			foreach (var timelineTrack in timelineTracks_)
			{
				if (ARIMotionScoreWindow.IsTackId(selectObjectId))
				{
					res = timelineTrack.GetTackById(selectObjectId);
					//見つかったら抜けないとNULLで上書きしてしまう
					if (res != null)break;
				}
			}

			return res;
		}

		public bool HasAnyValidTimeline()
		{
			if (timelineTracks_.Any())return true;
			return false;
		}

		public int GetIndexOfTimelineById(string timelineId)
		{
			return timelineTracks_.FindIndex(timeline => timeline.timelineId_ == timelineId);
		}

		public void ApplyDataToInspector()
		{
			if (scoreComponentInspector == null)scoreComponentInspector = ScriptableObject.CreateInstance("ScoreComponentInspector")as ScoreComponentInspector;
			scoreComponentInspector.UpdateTimelineTrack(this);
			//scoreComponentInspector.score.title = title;
		}

		public void BeforeSave()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_SCORE_BEFORESAVE, this.scoreGuid_));
		}

		public void Save()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_SCORE_SAVE, this.scoreGuid_));
		}

		public TimelineTrack GetActiveTimeline()
		{
			var res = timelineTracks_.Where(t => t.IsActive()).FirstOrDefault();
			if (res == null)throw new Exception("no active TimelineTrack found.");
			return res;
		}

		public TackPoint GetActiveTackPoint()
		{
			var timeline = timelineTracks_.Where(t => t.IsHaveActiveTackPoint()).FirstOrDefault();
			if (timeline == null)throw new Exception("no active TimelineTrack found.");

			return timeline.GetActiveTackPoint();
		}

		//選択フレームとその一つ前のタック（座標）
		public List<TackPoint> GetSelectedFrameAndPrevPos(int selectedFrame)
		{
			//Posのタイムライン
			var timeline = timelineTracks_
				.Where(t => t.IsExistTimeline_)
				.Where(t => t.timelineType_ == (int)TimelineType.TL_POS)
				.FirstOrDefault();
			return timeline.GetSelectedFrameAndPrev(selectedFrame);
		}

		//選択フレームに一番近い最後のタック（トランスフォーム）
		public MotionTransform GetLatestTransform(int selectedFrame)
		{
			//Transformのタイムライン
			var timeline = timelineTracks_
				.Where(t => t.IsExistTimeline_)
				.Where(t => t.timelineType_ == (int)TimelineType.TL_TRANSFORM)
				.FirstOrDefault();

			if (timeline == null)return new MotionTransform();

			var latestTack = timeline.GetLatestTransform(selectedFrame);

			if (latestTack == null)return new MotionTransform();

			return latestTack.motionData_.mTransform;
		}

		//選択フレームまで全てのタック（位置移動）
		public List<TackPoint> GetUntilSelectedFrameMove(int selectedFrame)
		{
			//Posのタイムライン
			var timeline = timelineTracks_
				.Where(t => t.IsExistTimeline_)
				.Where(t => t.timelineType_ == (int)TimelineType.TL_MOVE)
				.FirstOrDefault();

			if (timeline == null)return null;

			return timeline.GetUntilSelectedFrame(selectedFrame);
		}

		//選択フレームとその一つ前のタック（敵座標）
		public List<TackPoint> GetSelectedFrameAndPrevHold(int selectedFrame)
		{
			//Posのタイムライン
			var timeline = timelineTracks_
				.Where(t => t.IsExistTimeline_)
				.Where(t => t.timelineType_ == (int)TimelineType.TL_HOLD)
				.FirstOrDefault();

			if (timeline == null)return null;

			return timeline.GetSelectedFrameAndPrev(selectedFrame);
		}

		//選択フレームのタック（投げ座標）
		public TackPoint GetSelectedFrame(int selectedFrame, TimelineType timelineType)
		{
			//Posのタイムライン
			var timeline = timelineTracks_
				.Where(t => t.IsExistTimeline_)
				.Where(t => t.timelineType_ == (int)timelineType)
				.FirstOrDefault();

			if (timeline == null)return null;

			return timeline.GetSelectedFrame(selectedFrame);
		}

		public void SetScoreInspector()
		{
			Selection.activeObject = scoreComponentInspector; //インスペクタをコレに変える
		}

		public bool IsActiveTimeline()
		{
			var res = timelineTracks_.Where(t => t.IsActive()).FirstOrDefault();
			return (res != null);
		}

		public bool IsActiveTackPoint()
		{
			var timeline = timelineTracks_.Where(t => t.IsHaveActiveTackPoint()).FirstOrDefault();
			if (timeline == null)return false;

			var tackPoint = timeline.GetActiveTackPoint();
			return (tackPoint != null);
		}

		public void SqueezeTack()
		{
			foreach (var timelineTrack in timelineTracks_)
			{
				timelineTrack.SqueezeTack();
			}
		}
		public int LastFrame()
		{
			//IsExistTack_（存在する）とIsActive（選択中）を間違えないように

			//Posのタイムライン
			var timeline = timelineTracks_.Where(t => t.IsExistTimeline_)
				.Where(t => t.timelineType_ == (int)TimelineType.TL_POS)
				.FirstOrDefault();

			var lastTack = timeline.tackPoints_
				.Where(t => t.IsExistTack_)
				.OrderBy(t => t.start_)
				.LastOrDefault();

			return lastTack.start_ + lastTack.span_;
		}
	}
}