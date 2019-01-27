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

		[SerializeField]
		TimelineTrackInspector timelineTrackInspector_;

		[CustomEditor(typeof(TimelineTrackInspector))]
		public class TimelineTrackInspectorGUI : Editor
		{
			public override void OnInspectorGUI()
			{
				var insp = (TimelineTrackInspector)target;

				var timelineTrack = insp.timelineTrack;
				UpdateTimelineTrackTitle(timelineTrack);
			}
			void UpdateTimelineTrackTitle(TimelineTrack timelineTrack)
			{
				//var newTitle = EditorGUILayout.TextField("title", timelineTrack.title_);
				//var charManager = EditorGUILayout.ObjectField("charManager", timelineTrack.charManager, typeof(JMCharManager),true);

				TimelineType timelineType = (TimelineType)timelineTrack.timelineType_;

				EditorGUILayout.LabelField("type : " + timelineType.ToString());
				//if (newTitle != timelineTrack.title_)
				//{
				//	timelineTrack.BeforeSave();
				//	timelineTrack.title_ = newTitle;
				//	timelineTrack.Save();
				//}

				//if (charManager != timelineTrack.charManager)
				//{
				//	timelineTrack.charManager = charManager as JMCharManager;
				//}
			}
		}

		[SerializeField]
		int index_;
		[SerializeField]
		public bool IsExistTimeline_;
		[SerializeField]
		public bool active_;
		[SerializeField]
		public string timelineId_;
		[SerializeField]
		public bool haveActiveTack_;

		[SerializeField]
		public List<TackPoint> tackPoints_ = new List<TackPoint>();
		[SerializeField]
		public int timelineType_;

		Rect trackRect_;
		Texture2D timelineBaseTexture_;

		float timelineScrollX_;

		GUIStyle timelineConditionTypeLabelStyle_ = null;
		GUIStyle timelineConditionTypeLabelSmallStyle_ = null;

		List<string> movingTackIds_ = new List<string>();

		public TimelineTrack()
		{
			//InitializeTextResource();
			this.IsExistTimeline_ = true;
			// set initial track rect.
			var defaultHeight = (WindowSettings.TIMELINE_HEIGHT);
			trackRect_ = new Rect(0, 0, 10, defaultHeight);
		}

		//
		public TimelineTrack(int index, int timelineType, List<TackPoint> tackPoints)
		{
			InitializeTextResource();

			this.IsExistTimeline_ = true;

			this.timelineId_ = WindowSettings.ID_HEADER_TIMELINE + Guid.NewGuid().ToString();
			this.index_ = index;

			this.timelineType_ = timelineType;
			this.tackPoints_ = new List<TackPoint>(tackPoints);

			// set initial track rect.
			var defaultHeight = (WindowSettings.TIMELINE_HEIGHT);
			trackRect_ = new Rect(0, 0, 10, defaultHeight);

			ApplyTextureToTacks(index);
		}

		public TimelineTrack(int index, Dictionary<string, object> scoreTimelineDict, List<TackPoint> currentTacks)
		{
			InitializeTextResource();

			this.IsExistTimeline_ = true;

			this.timelineId_ = WindowSettings.ID_HEADER_TIMELINE + Guid.NewGuid().ToString();
			this.index_ = index;

			this.timelineType_ = Convert.ToInt32(scoreTimelineDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TIMELINE_TYPE]);
			this.tackPoints_ = currentTacks;

			// set initial track rect.
			var defaultHeight = (WindowSettings.TIMELINE_HEIGHT);
			trackRect_ = new Rect(0, 0, 10, defaultHeight);

			ApplyTextureToTacks(index);
		}

		public Dictionary<string, object> OutputDict(List<object> tackList)
		{
			var res = new Dictionary<string, object>
				{ { TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TIMELINE_TYPE, this.timelineType_ },
					{ TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TIMELINE_TACKS, tackList }
				};

			return res;
		}

		public MotionTimeline OutputTimelineObject()
		{
			MotionTimeline res = null;
			switch ((TimelineType)timelineType_)
			{
				case TimelineType.TL_POS:
					res = new MotionTimeline(timelineType_, ConvertMotionPos());
					break;
				case TimelineType.TL_TRANSFORM:
					res = new MotionTimeline(timelineType_, ConvertMotionTransform());
					break;
				case TimelineType.TL_MOVE:
					res = new MotionTimeline(timelineType_, ConvertMotionMove());
					break;
					//case TimelineType.TL_ATARI: res = new MotionTimeline(timelineType_, ConvertMotionAtari()); break;
					//case TimelineType.TL_HOLD: res = new MotionTimeline(timelineType_, ConvertMotionHold()); break;
					//case TimelineType.TL_THROW: res = new MotionTimeline(timelineType_, ConvertMotionThrow()); break;
				case TimelineType.TL_COLOR:
					res = new MotionTimeline(timelineType_, ConvertMotionColor());
					break;
				case TimelineType.TL_EFFECT:
					res = new MotionTimeline(timelineType_, ConvertMotionEffect());
					break;
				case TimelineType.TL_PASSIVE:
					res = new MotionTimeline(timelineType_, ConvertMotionPassive());
					break;
			}
			return res;
		}

		public void OutputAniScript(List<AniFrame> aniScriptFrames)
		{
			switch ((TimelineType)timelineType_)
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
					//case TimelineType.TL_TRANSFORM: res = new MotionTimeline(timelineType_, ConvertMotionTransform()); break;
					//case TimelineType.TL_MOVE: res = new MotionTimeline(timelineType_, ConvertMotionMove()); break;
					////case TimelineType.TL_ATARI: res = new MotionTimeline(timelineType_, ConvertMotionAtari()); break;
					////case TimelineType.TL_HOLD: res = new MotionTimeline(timelineType_, ConvertMotionHold()); break;
					////case TimelineType.TL_THROW: res = new MotionTimeline(timelineType_, ConvertMotionThrow()); break;
					//case TimelineType.TL_COLOR: res = new MotionTimeline(timelineType_, ConvertMotionColor()); break;
					//case TimelineType.TL_EFFECT: res = new MotionTimeline(timelineType_, ConvertMotionEffect()); break;
					//case TimelineType.TL_PASSIVE: res = new MotionTimeline(timelineType_, ConvertMotionPassive()); break;
			}

		}

		List<MotionPosState> GetIntermediatePos()
		{
			List<MotionPosState> res = new List<MotionPosState>();
			int nowFrame = 0;
			for (int i = 0; i < tackPoints_.Count; i++)
			{
				var nowTack = tackPoints_[i];
				TackPoint prevTack = (i > 0)
					? tackPoints_[i - 1]
					: new TackPoint();

				for (int i2 = 0; i2 < nowTack.span_; i2++)
				{
					var intermediate = MotionPos.MakeIntermediate2(
						prevTack.motionData_.mPos,
						nowTack.motionData_.mPos,
						nowTack.start_, nowTack.span_, nowFrame);
					res.Add(intermediate);
					nowFrame++;
				}

			}

			return res;
		}

		public List<MotionTackPos> ConvertMotionPos()
		{
			List<MotionTackPos> res = new List<MotionTackPos>();
			foreach (var item in tackPoints_)res.Add(item.OutputMotionTackPos());
			return res;
		}
		public List<MotionTackTransform> ConvertMotionTransform()
		{
			List<MotionTackTransform> res = new List<MotionTackTransform>();
			foreach (var item in tackPoints_)res.Add(item.OutputMotionTackTransform());
			return res;
		}
		public List<MotionTackMove> ConvertMotionMove()
		{
			List<MotionTackMove> res = new List<MotionTackMove>();
			foreach (var item in tackPoints_)res.Add(item.OutputMotionTackMove());
			return res;
		}
		public List<MotionTackColor> ConvertMotionColor()
		{
			List<MotionTackColor> res = new List<MotionTackColor>();
			foreach (var item in tackPoints_)res.Add(item.OutputMotionTackColor());
			return res;
		}
		public List<MotionTackEffect> ConvertMotionEffect()
		{
			List<MotionTackEffect> res = new List<MotionTackEffect>();
			foreach (var item in tackPoints_)res.Add(item.OutputMotionTackEffect());
			return res;
		}
		public List<MotionTackPassive> ConvertMotionPassive()
		{
			List<MotionTackPassive> res = new List<MotionTackPassive>();
			foreach (var item in tackPoints_)res.Add(item.OutputMotionTackPassive());
			return res;
		}

		void InitializeTextResource()
		{
			timelineConditionTypeLabelStyle_ = new GUIStyle();
			timelineConditionTypeLabelStyle_.normal.textColor = Color.white;
			timelineConditionTypeLabelStyle_.fontSize = 16;
			timelineConditionTypeLabelStyle_.alignment = TextAnchor.MiddleCenter;

			timelineConditionTypeLabelSmallStyle_ = new GUIStyle();
			timelineConditionTypeLabelSmallStyle_.normal.textColor = Color.white;
			timelineConditionTypeLabelSmallStyle_.fontSize = 10;
			timelineConditionTypeLabelSmallStyle_.alignment = TextAnchor.MiddleCenter;
		}

		public void BeforeSave()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TIMELINE_BEFORESAVE, this.timelineId_));
		}

		public void Save()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TIMELINE_SAVE, this.timelineId_));
		}

		public void ApplyTextureToTacks(int texIndex)
		{
			timelineBaseTexture_ = GetTimelineTexture(texIndex);
			foreach (var tackPoint in tackPoints_)tackPoint.InitializeTackTexture(timelineBaseTexture_);
		}

		public static Texture2D GetTimelineTexture(int textureIndex)
		{
			var color = WindowSettings.RESOURCE_COLORS_SOURCES[textureIndex % WindowSettings.RESOURCE_COLORS_SOURCES.Count];
			var colorTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			colorTex.SetPixel(0, 0, color);
			colorTex.Apply();

			return colorTex;
		}

		public float Height()
		{
			return trackRect_.height;
		}

		public int GetIndex()
		{
			return index_;
		}

		public void SetActive()
		{
			active_ = true;
			haveActiveTack_ = false;
			ApplyDataToInspector();
			Selection.activeObject = timelineTrackInspector_; //�C���X�y�N�^��R���ɕς���
		}

		public void SetDeactive()
		{
			active_ = false;
			haveActiveTack_ = false;
		}

		public bool IsActive()
		{
			return active_;
		}

		public bool ContainsActiveTack()
		{
			foreach (var tackPoint in tackPoints_)
			{
				if (tackPoint.IsActive())return true;
			}
			return false;
		}

		public int GetStartFrameById(string objectId)
		{
			foreach (var tackPoint in tackPoints_)
			{
				if (tackPoint.tackId_ == objectId)return tackPoint.start_;
			}
			return -1;
		}

		public void SetTimelineY(float additional)
		{
			trackRect_.y = trackRect_.y + additional;
		}

		public float DrawTimelineTrack(float headWall, float timelineScrollX, float yOffsetPos, float width)
		{
			this.timelineScrollX_ = timelineScrollX;

			trackRect_.width = width;
			trackRect_.y = yOffsetPos;

			if (trackRect_.y < headWall)trackRect_.y = headWall;

			if (timelineBaseTexture_ == null)ApplyTextureToTacks(index_);

			trackRect_ = GUI.Window(index_, trackRect_, WindowEventCallback, string.Empty, "AnimationKeyframeBackground");
			return trackRect_.height;
		}

		public float GetY()
		{
			return trackRect_.y;
		}

		public float GetHeight()
		{
			return trackRect_.height;
		}

		void WindowEventCallback(int id)
		{
			// draw bg from header to footer.
			{
				if (active_)
				{
					var headerBGActiveRect = new Rect(0f, 0f, trackRect_.width, WindowSettings.TIMELINE_HEIGHT);
					GUI.DrawTexture(headerBGActiveRect, WindowSettings.activeTackBaseTex);

					var headerBGRect = new Rect(1f, 1f, trackRect_.width - 1f, WindowSettings.TIMELINE_HEIGHT - 2f);
					GUI.DrawTexture(headerBGRect, WindowSettings.timelineHeaderTex);
				}
				else
				{
					var headerBGRect = new Rect(0f, 0f, trackRect_.width, WindowSettings.TIMELINE_HEIGHT);
					GUI.DrawTexture(headerBGRect, WindowSettings.timelineHeaderTex);
				}
			}

			var timelineBodyY = WindowSettings.TIMELINE_HEADER_HEIGHT;

			// timeline condition type box.	
			var conditionBGRect = new Rect(1f, timelineBodyY, WindowSettings.TIMELINE_CONDITIONBOX_WIDTH - 1f, WindowSettings.TACK_HEIGHT - 1f);
			if (active_)
			{
				var conditionBGRectInActive = new Rect(1f, timelineBodyY, WindowSettings.TIMELINE_CONDITIONBOX_WIDTH - 1f, WindowSettings.TACK_HEIGHT - 1f);
				GUI.DrawTexture(conditionBGRectInActive, timelineBaseTexture_);
			}
			else
			{
				GUI.DrawTexture(conditionBGRect, timelineBaseTexture_);
			}

			string timelineTypeName = ((TimelineType)timelineType_).ToString();

			if ((timelineConditionTypeLabelStyle_ == null)
				|| (timelineConditionTypeLabelSmallStyle_ == null))
			{

				InitializeTextResource();
			}

			GUI.Label(
				new Rect(
					0f,
					WindowSettings.TIMELINE_HEADER_HEIGHT - 1f,
					WindowSettings.TIMELINE_CONDITIONBOX_WIDTH,
					WindowSettings.TACK_HEIGHT
				),
				timelineTypeName,
				timelineConditionTypeLabelSmallStyle_
			);

			var frameRegionWidth = trackRect_.width - WindowSettings.TIMELINE_CONDITIONBOX_SPAN;

			// draw frame back texture & TackPoint datas on frame.
			GUI.BeginGroup(new Rect(WindowSettings.TIMELINE_CONDITIONBOX_SPAN, timelineBodyY, trackRect_.width, WindowSettings.TACK_HEIGHT));
			{
				DrawFrameRegion(timelineScrollX_, 0f, frameRegionWidth);
			}
			GUI.EndGroup();

			var useEvent = false;

			// mouse manipulation.
			switch (Event.current.type)
			{

				case EventType.ContextClick:
					{
						ShowContextMenu(timelineScrollX_);
						useEvent = true;
						break;
					}

					// clicked.
				case EventType.MouseUp:
					{

						// is right clicked
						if (Event.current.button == 1)
						{
							ShowContextMenu(timelineScrollX_);
							useEvent = true;
							break;
						}

						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, this.timelineId_));
						useEvent = true;
						break;
					}
			}

			// constraints.
			trackRect_.x = 0;
			if (trackRect_.y < 0)trackRect_.y = 0;

			GUI.DragWindow();
			if (useEvent)Event.current.Use();
		}

		void ShowContextMenu(float scrollX)
		{
			var targetFrame = GetFrameOnTimelineFromLocalMousePos(Event.current.mousePosition, scrollX);
			var menu = new GenericMenu();

			var menuItems = new Dictionary<string, OnTrackEvent.EventType>
				{ { "Add New Tack", OnTrackEvent.EventType.EVENT_TIMELINE_ADDTACK },
					{ "Paste Tack", OnTrackEvent.EventType.EVENT_TIMELINE_PASTETACK },
					{ "Delete This Timeline", OnTrackEvent.EventType.EVENT_TIMELINE_DELETE },
				};

			foreach (var key in menuItems.Keys)
			{
				var eventType = menuItems[key];
				var enable = IsEnableEvent(eventType, targetFrame);
				if (enable)
				{
					menu.AddItem(
						new GUIContent(key),
						false,
						() =>
						{
							Emit(new OnTrackEvent(eventType, this.timelineId_, targetFrame));
						}
					);
				}
				else
				{
					menu.AddDisabledItem(new GUIContent(key));
				}
			}
			menu.ShowAsContext();
		}

		bool IsEnableEvent(OnTrackEvent.EventType eventType, int frame)
		{
			switch (eventType)
			{
				case OnTrackEvent.EventType.EVENT_TIMELINE_ADDTACK:
					{
						foreach (var tackPoint in tackPoints_)
						{
							if (tackPoint.ContainsFrame(frame))
							{
								if (!tackPoint.IsExistTack_)return true;
								return false;
							}
						}
						return true;
					}
				case OnTrackEvent.EventType.EVENT_TIMELINE_DELETE:
					{
						return true;
						//return (timelineType_ != (int)(TimelineType.TL_POS));
					}

				default:
					{
						// Debug.LogError("unhandled eventType IsEnableEvent:" + eventType);
						return true;
					}
			}
		}

		int GetFrameOnTimelineFromLocalMousePos(Vector2 localMousePos, float scrollX)
		{
			var frameSourceX = localMousePos.x + Math.Abs(scrollX) - WindowSettings.TIMELINE_CONDITIONBOX_SPAN;
			return GetFrameOnTimelineFromAbsolutePosX(frameSourceX);
		}

		public static int GetFrameOnTimelineFromAbsolutePosX(float frameSourceX)
		{
			return (int)(frameSourceX / WindowSettings.TACK_FRAME_WIDTH);
		}

		void DrawFrameRegion(float timelineScrollX, float timelineBodyY, float frameRegionWidth)
		{
			var limitRect = new Rect(0, 0, frameRegionWidth, WindowSettings.TACK_HEIGHT);

			// draw frame background.
			{
				DrawFrameBG(timelineScrollX, timelineBodyY, frameRegionWidth, WindowSettings.TACK_FRAME_HEIGHT, false);
			}

			// draw tack points & label on this track in range.
			{
				var index = 0;
				foreach (var tackPoint in tackPoints_)
				{
					var isUnderEvent = movingTackIds_.Contains(tackPoint.tackId_);
					if (!movingTackIds_.Any())isUnderEvent = true;

					// draw tackPoint on the frame.
					tackPoint.DrawTack(limitRect, this.timelineId_, timelineScrollX, timelineBodyY, isUnderEvent);
					index++;
				}
			}
		}

		public static void DrawFrameBG(float timelineScrollX, float timelineBodyY, float frameRegionWidth, float frameRegionHeight, bool showFrameCount)
		{
			var yOffset = timelineBodyY;

			// show 0 count.
			if (showFrameCount)
			{
				if (0 < WindowSettings.TACK_FRAME_WIDTH + timelineScrollX)GUI.Label(new Rect(timelineScrollX + 3, 0, 20, WindowSettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), "0");
				yOffset = yOffset + WindowSettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT;
			}

			// draw 1st 1 frame.
			if (0 < WindowSettings.TACK_FRAME_WIDTH + timelineScrollX)
			{
				GUI.DrawTexture(new Rect(timelineScrollX, yOffset, WindowSettings.TACK_5FRAME_WIDTH, frameRegionHeight), WindowSettings.frameTex);
			}

			var repeatCount = (frameRegionWidth - timelineScrollX) / WindowSettings.TACK_5FRAME_WIDTH;
			for (var i = 0; i < repeatCount; i++)
			{
				var xPos = WindowSettings.TACK_FRAME_WIDTH + timelineScrollX + (i * WindowSettings.TACK_5FRAME_WIDTH);
				if (xPos + WindowSettings.TACK_5FRAME_WIDTH < 0)continue;

				if (showFrameCount)
				{
					var frameCountStr = ((i + 1) * 5).ToString();
					var span = 0;
					if (2 < frameCountStr.Length)span = ((frameCountStr.Length - 2) * 8) / 2;
					GUI.Label(new Rect(xPos + (WindowSettings.TACK_FRAME_WIDTH * 4) - span, 0, frameCountStr.Length * 10, WindowSettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), frameCountStr);
				}
				var frameRect = new Rect(xPos, yOffset, WindowSettings.TACK_5FRAME_WIDTH, frameRegionHeight);
				GUI.DrawTexture(frameRect, WindowSettings.frameTex);
			}
		}

		//��O�̃^�b�N�擾
		public void SelectPreviousTackOf(string tackId)
		{
			var cursoredTackIndex = tackPoints_.FindIndex(tack => tack.tackId_ == tackId);

			if (cursoredTackIndex == 0)
			{
				Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, this.timelineId_));
				return;
			}

			var currentExistTacks = tackPoints_.Where(tack => tack.IsExistTack_).OrderByDescending(tack => tack.start_).ToList();
			var currentTackIndex = currentExistTacks.FindIndex(tack => tack.tackId_ == tackId);

			if (0 <= currentTackIndex && currentTackIndex < currentExistTacks.Count - 1)
			{
				var nextTack = currentExistTacks[currentTackIndex + 1];
				Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, nextTack.tackId_));
			}
		}

		//����Ƃ̃^�b�N�擾
		public void SelectNextTackOf(string tackId)
		{
			var currentExistTacks = tackPoints_.Where(tack => tack.IsExistTack_).OrderBy(tack => tack.start_).ToList();
			var currentTackIndex = currentExistTacks.FindIndex(tack => tack.tackId_ == tackId);

			if (0 <= currentTackIndex && currentTackIndex < currentExistTacks.Count - 1)
			{
				var nextTack = currentExistTacks[currentTackIndex + 1];
				Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, nextTack.tackId_));
			}
		}

		public void SelectDefaultTackOrSelectTimeline()
		{
			if (tackPoints_.Any())
			{
				var firstTackPoint = tackPoints_[0];
				Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, firstTackPoint.tackId_));
				return;
			}

			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, this.timelineId_));
		}

		public void ActivateTacks(List<string> activeTackIds)
		{
			haveActiveTack_ = false;
			foreach (var tackPoint in tackPoints_)
			{
				if (activeTackIds.Contains(tackPoint.tackId_))
				{
					tackPoint.SetActive();
					haveActiveTack_ = true;
				}
				else
				{
					tackPoint.SetDeactive();
				}
			}
		}

		public void DeactivateTacks()
		{
			haveActiveTack_ = false;
			foreach (var tackPoint in tackPoints_)
			{
				tackPoint.SetDeactive();
			}
		}

		//ID����v����^�b�N�擾
		public List<TackPoint> TacksByIds(List<string> tackIds)
		{
			var results = new List<TackPoint>();
			foreach (var tackPoint in tackPoints_)
			{
				if (tackIds.Contains(tackPoint.tackId_))
				{
					results.Add(tackPoint);
				}
			}
			return results;
		}

		//�X�^�[�g�ʒu���߂��^�b�N�擾
		public List<TackPoint> TacksByStart(int startPos)
		{
			var startIndex = tackPoints_.FindIndex(tack => startPos <= tack.start_);
			if (0 <= startIndex)
			{
				// if index - 1 tack contains startPos, return it.
				if (0 < startIndex && (startPos <= tackPoints_[startIndex - 1].start_ + tackPoints_[startIndex - 1].span_ - 1))
				{
					return new List<TackPoint> { tackPoints_[startIndex - 1] };
				}
				return new List<TackPoint> { tackPoints_[startIndex] };
			}

			// no candidate found in area, but if any tack exists, select the last of it. 
			if (tackPoints_.Any())
			{
				return new List<TackPoint> { tackPoints_[tackPoints_.Count - 1] };
			}
			return new List<TackPoint>();
		}

		public bool ContainsTackById(string tackId)
		{
			foreach (var tackPoint in tackPoints_)
			{
				if (tackId == tackPoint.tackId_)return true;
			}
			return false;
		}

		public void Deleted(bool isCancel)
		{
			IsExistTimeline_ = isCancel;
		}

		//�^�b�N�̈ړ��㏈��
		public void UpdateByTackMoved(string tackId)
		{
			movingTackIds_.Clear();

			//�ړ��^�b�N
			var movedTack = TacksByIds(new List<string> { tackId })[0];

			movedTack.ApplyDataToInspector();

			//���X����^�b�N��Ɉړ��O�����������A���̖����Ɉړ��^�b�N�̃X�^�[�g��␳
			foreach (var targetTack in tackPoints_)
			{

				if (targetTack.tackId_ == tackId)continue; //�������g
				if (!targetTack.IsExistTack_)continue; //�����^�b�N

				// not contained case.
				if (targetTack.start_ + (targetTack.span_ - 1) < movedTack.start_)continue; //���S�ɑO��
				//if (movedTack.start_ + (movedTack.span_ - 1) < targetTack.start_) continue;//���S�Ɍ��

				//���X����^�b�N��Ɉړ��O�����������A���̖����Ɉړ��^�b�N�̃X�^�[�g��␳
				if ((movedTack.start_ > targetTack.start_)
					&& (movedTack.start_ < (targetTack.start_ + targetTack.span_)))
				{
					//movedTack.UpdatePos(targetTack.start_ + targetTack.span_, movedTack.span_);

					string id = MethodBase.GetCurrentMethod().Name;

					var newStart = targetTack.start_ + targetTack.span_;
					var lastStart = movedTack.start_;
					var newSpan = movedTack.span_;

					Action action = () =>
					{
						movedTack.UpdatePos(newStart, newSpan);
					};

					Action undo = () =>
					{
						movedTack.UpdatePos(lastStart, newSpan);
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));
					//Debug.Log("movedTackaaaa");
					break;
				}
				else
				{
					//Debug.Log("movedTackbbbb");
				}
			}
			string id2 = MethodBase.GetCurrentMethod().Name;

			List<Action> cmdDo = new List<Action>();
			List<Action> cmdUndo = new List<Action>();

			//�O���ړ���
			if (movedTack.start_ < movedTack.GetLastStart())
			{
				foreach (var targetTack in tackPoints_)
				{

					if (targetTack.tackId_ == tackId)continue; //�������g
					if (!targetTack.IsExistTack_)continue; //�����^�b�N
					if (targetTack.start_ + (targetTack.span_ - 1) < movedTack.start_)continue; //���S�ɑO��

					//�ړ��^�b�N�̈ړ��O�X�^�[�g���O�ɂ���A�ړ�����ɂ������͈ړ��^�b�N�̃X�p�����������Ɉړ�
					if ((targetTack.start_ < movedTack.GetLastStart())
						&& (movedTack.start_ <= targetTack.start_))
					{
						var tag = targetTack; //Do�̓R�R����Ȃ��̂ŕʂ̕ϐ��ɏ����ʂ��Ȃ��ƃ_��
						var newStart = targetTack.start_ + movedTack.span_;
						var lastStart = targetTack.start_;
						var newSpan = targetTack.span_;

						//Debug.Log("bbbb");
						cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
						cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));

						continue;
					}
					else
					{
						//Debug.Log("gggg");
					}
					//���������A��邱�Ƃ͏�L�ʒu����ւ������邽�ߏo���Ȃ�
				}
			}
			else if (movedTack.start_ > movedTack.GetLastStart()) //����ړ���
			{
				foreach (var targetTack in tackPoints_)
				{

					if (targetTack.tackId_ == tackId)continue; //�������g
					if (!targetTack.IsExistTack_)continue; //�����^�b�N

					//���X�ړ��^�b�N�̌��
					if (movedTack.GetLastStart() < targetTack.start_)
					{
						//���X�ړ��^�b�N�̌��ɂ����āA���݊��S�ɑO�ɂ����͈ړ��^�b�N�̃X�p���������O���ړ�
						if (targetTack.start_ + (targetTack.span_ - 1) < movedTack.start_)
						{
							var tag = targetTack;
							var newStart = targetTack.start_ - movedTack.span_;
							var lastStart = targetTack.start_;
							var newSpan = targetTack.span_;
							//Debug.Log("cccc");
							cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
							cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));
							continue;
						}
						else if (movedTack.start_ <= targetTack.start_)
						{
							//���X�ړ��^�b�N�̌��ɂ����āA���݈ړ��^�b�N�̃X�^�[�g�ʒu���X�^�[�g�ʒu������̂�͈̂ړ�����������ړ�
							//Debug.Log("dddd");
							var tag = targetTack;
							var newStart = targetTack.start_ + (movedTack.start_ - movedTack.GetLastStart());
							var lastStart = targetTack.start_;
							var newSpan = targetTack.span_;
							cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
							cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));

							continue;
						}
						else
						{
							//Debug.Log("asdf");
						}
					}
				}
			}
			else //�X�p�����΂�//�X�p����ύX��������������̃^�b�N��ړ�
			{
				foreach (var targetTack in tackPoints_)
				{

					if (targetTack.tackId_ == tackId)continue; //�������g
					if (!targetTack.IsExistTack_)continue; //�����^�b�N

					//���X�ړ��^�b�N�̌��
					if (movedTack.GetLastStart() < targetTack.start_)
					{
						//var newStartPos = targetTack.start_ + (movedTack.span_ - movedTack.GetLastSpan());
						//targetTack.UpdatePos(newStartPos, targetTack.span_);
						//Debug.Log("eeee");
						var tag = targetTack;
						var newStart = targetTack.start_ + (movedTack.span_ - movedTack.GetLastSpan());
						var lastStart = targetTack.start_;
						var newSpan = targetTack.span_;
						cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
						cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));

						continue;
					}
					else
					{
						//Debug.Log("ffff");
					}
				}
			}

			if (cmdDo.Any())
			{
				ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id2,
					() => { foreach (var cmd in cmdDo)cmd(); },
					() => { foreach (var cmd in cmdUndo)cmd(); }));
			}

			//�g�����X�t�H�[���^�C�����C���͌��Ԃ𖄂߂�
			SqueezeTack();
		}

		//���Ԃ𖄂߂�(�g�����X�t�H�[���^�C�����C���̂�)
		public void SqueezeTack()
		{
			//�g�����X�t�H�[���^�C�����C���͌��Ԃ𖄂߂�
			if ((TimelineType)timelineType_ == TimelineType.TL_POS)
			{
				var sortedTacks = tackPoints_.OrderBy(t => t.start_);
				int nextStart = 0;
				foreach (var targetTack in sortedTacks)
				{
					if (!targetTack.IsExistTack_)continue; //�����^�b�N
					targetTack.UpdatePos(nextStart, targetTack.span_);
					nextStart = (targetTack.start_ + targetTack.span_);
				}
			}
		}

		public void SetMovingTack(string tackId)
		{
			movingTackIds_ = new List<string> { tackId };
		}

		public TackPoint NewTackToEmptyFrame(int frame, int timelineType)
		{
			var newTackPoint = new TackPoint(
				tackPoints_.Count,
				frame,
				WindowSettings.DEFAULT_TACK_SPAN,
				timelineType
			);

			return newTackPoint;
		}

		public void AddNewTackToEmptyFrame(int frame, TackPoint newTackPoint)
		{
			//var newTackPoint = new TackPoint(
			//	tackPoints_.Count,
			//	frame,
			//	WindowSettings.DEFAULT_TACK_SPAN,
			//	timelineType
			//);
			tackPoints_.Add(newTackPoint);

			ApplyTextureToTacks(index_);
		}

		public void PasteTackToEmptyFrame(int frame, TackPoint clipTack)
		{

			var newTackPoint = new TackPoint(
				tackPoints_.Count,
				frame,
				clipTack
			);
			tackPoints_.Add(newTackPoint);

			ApplyTextureToTacks(index_);
		}

		public void DeleteTackById(string tackId, bool isCancel)
		{
			var deletedTackIndex = tackPoints_.FindIndex(tack => tack.tackId_ == tackId);
			if (deletedTackIndex == -1)
			{
				//Debug.Log("(deletedTackIndex == -1) ");
				return;
			}
			else
			{
				//Debug.Log("tackPoints_[deletedTackIndex].Deleted(isCancel); ");
			}
			tackPoints_[deletedTackIndex].Deleted(isCancel);
		}

		public TackPoint GetTackById(string tackId)
		{
			var selectTackIndex = tackPoints_.FindIndex(tack => tack.tackId_ == tackId);
			if (selectTackIndex == -1)return null;
			return tackPoints_[selectTackIndex];
		}

		public void ApplyDataToInspector()
		{
			if (timelineTrackInspector_ == null)timelineTrackInspector_ = ScriptableObject.CreateInstance("TimelineTrackInspector")as TimelineTrackInspector;

			timelineTrackInspector_.UpdateTimelineTrack(this);

			foreach (var tackPoint in tackPoints_)
			{
				tackPoint.ApplyDataToInspector();
			}
		}

		public TackPoint GetActiveTackPoint()
		{
			var res = tackPoints_.Where(t => t.IsActive()).FirstOrDefault();
			if (res == null)throw new Exception("no active TackPoint found.");
			return res;
		}

		//���݂̃t���[���̃^�b�N�ƁA��O�̃^�b�N
		public List<TackPoint> GetSelectedFrameAndPrev(int selectedFrame)
		{
			List<TackPoint> res = new List<TackPoint>();
			var selectedTack = tackPoints_
				.Where(t => t.IsExistTack_)
				.Where(t => (t.start_ <= selectedFrame))
				.Where(t => ((t.start_ + t.span_) > selectedFrame))
				.OrderBy(t => t.start_)
				.FirstOrDefault();

			if (selectedTack == null)return res;

			res.Add(selectedTack);
			var nextTack = tackPoints_
				.Where(t => t.IsExistTack_)
				.Where(t => t.start_ < selectedTack.start_)
				.OrderBy(t => t.start_)
				.LastOrDefault();

			if (nextTack != null)res.Add(nextTack);

			return res;
		}

		//�I��t���[���ɍł�߂��Ō���̃^�b�N
		public TackPoint GetLatestTransform(int selectedFrame)
		{
			var res = tackPoints_
				.Where(t => t.IsExistTack_)
				.Where(t => (t.start_ <= selectedFrame))
				.OrderBy(t => t.start_)
				.LastOrDefault();

			return res;
		}

		//���݂̃t���[���܂ł̑S�Ẵ^�b�N
		public List<TackPoint> GetUntilSelectedFrame(int selectedFrame)
		{
			List<TackPoint> res = new List<TackPoint>();
			var untilTacks = tackPoints_
				.Where(t => t.IsExistTack_)
				.Where(t => (t.start_ <= selectedFrame))
				//.Where(t => ((t.start_ + t.span_) > selectedFrame))
				.OrderBy(t => t.start_);

			if (untilTacks == null)return res;

			foreach (var item in untilTacks)res.Add(item);

			return res;
		}

		//���݂̃t���[���̃^�b�N
		public TackPoint GetSelectedFrame(int selectedFrame)
		{
			var res = tackPoints_
				.Where(t => t.IsExistTack_)
				.Where(t => (t.start_ <= selectedFrame))
				.Where(t => ((t.start_ + t.span_) > selectedFrame))
				.FirstOrDefault();

			return res;
		}

		public bool IsHaveActiveTackPoint()
		{
			return haveActiveTack_;
		}

	}
}