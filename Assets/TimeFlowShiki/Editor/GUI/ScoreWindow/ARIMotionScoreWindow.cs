using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MiniJSONForTimeFlowShiki;
using System.Reflection;

namespace NKKD.EDIT {
	public partial class ARIMotionScoreWindow : EditorWindow {
		enum enAutoScroll {
			Stop,
			Play,
			LoopPlay,
		}
		struct ManipulateTargets {
			public List<string> activeObjectIds;
			public ManipulateTargets(List<string> activeObjectIds) {
				this.activeObjectIds = activeObjectIds;
			}
		}
		struct ManipulateEvents {
			public bool keyLeft;
			public bool keyRight;
			public bool keyUp;
			public bool keyDown;
			public bool keyNum;
			//public int keyNumValue;
			//public void SetKeyNum(KeyCode keycode)
			//{
			//	keyNum = true;
			//	switch (keycode)
			//	{
			//		case KeyCode.Keypad1: keyNumValue = 1; break;
			//		case KeyCode.Keypad2: keyNumValue = 2; break;
			//		case KeyCode.Keypad3: keyNumValue = 3; break;
			//		case KeyCode.Keypad4: keyNumValue = 4; break;
			//		case KeyCode.Keypad5: keyNumValue = 5; break;
			//		case KeyCode.Keypad6: keyNumValue = 6; break;
			//		case KeyCode.Keypad7: keyNumValue = 7; break;
			//		case KeyCode.Keypad8: keyNumValue = 8; break;
			//		case KeyCode.Keypad9: keyNumValue = 9; break;
			//		case KeyCode.Keypad0: keyNumValue = 10; break;
			//	}

			//}
		}

		public List<ScoreComponent> scores_;// = new List<ScoreComponent>();
		public static Action<OnTrackEvent> ParentEmit;
		public ARIMotionMainWindow parent_;
		public bool isMovePos_ = true;
		ManipulateTargets manipulateTargets = new ManipulateTargets(new List<string>());
		float selectedPos_;
		int selectedFrame_;
		float cursorPos_;
		float scrollPos_;
		GUIStyle activeFrameLabelStyle_;
		GUIStyle activeConditionValueLabelStyle_;
		int drawCounter = 0;
		ManipulateEvents manipulateEvents_ = new ManipulateEvents();
		List<OnTrackEvent> eventStacks_ = new List<OnTrackEvent>();
		TackPoint clipTack_ = null;
		enAutoScroll autoScroll_ = enAutoScroll.Stop;
		double playStartTime_;
		int lastFrame_ = 0;
		float scrollSpeed_ = 1.0f;


		//[MenuItem("Window/TimeFlowShiki")]
		public static ARIMotionScoreWindow ShowEditor(ARIMotionMainWindow parent) {
			ARIMotionScoreWindow window = EditorWindow.GetWindow<ARIMotionScoreWindow>();
			window.parent_ = parent;
			window.scores_ = parent.scores_;
			window.Show();
			return window;
		}

		public void OnEnable() {
			InitializeResources();
			ScoreComponent.Emit = Emit;
			TimelineTrack.Emit = Emit;
			TackPoint.Emit = Emit;
			InitializeScoreView();

		}
		public void ApplyDataToInspector() {
			foreach (var score in scores_) score.ApplyDataToInspector();
		}

		public bool HasValidScore() {
			if (scores_.Any()) {
				foreach (var score in scores_) {
					if (score.IsExistScore_) return true;
				}
			}
			return false;
		}

		public static bool IsTimelineId(string activeObjectId) {
			if (activeObjectId.StartsWith(WindowSettings.ID_HEADER_TIMELINE)) return true;
			return false;
		}

		public static bool IsTackId(string activeObjectId) {
			if (activeObjectId.StartsWith(WindowSettings.ID_HEADER_TACK)) return true;
			return false;
		}



		public int GetSelectedFrame() {
			return selectedFrame_;
		}

		public void SetZeroFrame() {
			selectedPos_ = 0;
			selectedFrame_ = 0;
		}


		void InitializeScoreView() {
			this.titleContent = new GUIContent("TimelineKit");
			this.wantsMouseMove = true;
			this.minSize = new Vector2(600f, 300f);
			this.scrollPos_ = 0;
		}

		void InitializeResources() {
			WindowSettings.tickTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TICK, typeof(Texture2D)) as Texture2D;
			WindowSettings.timelineHeaderTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TRACK_HEADER_BG, typeof(Texture2D)) as Texture2D;
			WindowSettings.conditionLineBgTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_CONDITIONLINE_BG, typeof(Texture2D)) as Texture2D;
			WindowSettings.frameTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TRACK_FRAME_BG, typeof(Texture2D)) as Texture2D;
			WindowSettings.whitePointTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TACK_WHITEPOINT, typeof(Texture2D)) as Texture2D;
			WindowSettings.grayPointTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TACK_GRAYPOINT, typeof(Texture2D)) as Texture2D;
			WindowSettings.whitePointSingleTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TACK_WHITEPOINT_SINGLE, typeof(Texture2D)) as Texture2D;
			WindowSettings.grayPointSingleTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TACK_GRAYPOINT_SINGLE, typeof(Texture2D)) as Texture2D;
			WindowSettings.activeTackBaseTex = AssetDatabase.LoadAssetAtPath(WindowSettings.RESOURCE_TACK_ACTIVE_BASE, typeof(Texture2D)) as Texture2D;

			activeFrameLabelStyle_ = new GUIStyle();
			activeFrameLabelStyle_.normal.textColor = Color.white;

			activeConditionValueLabelStyle_ = new GUIStyle();
			activeConditionValueLabelStyle_.fontSize = 11;
			activeConditionValueLabelStyle_.normal.textColor = Color.white;
		}



		void Update() {
			AutoScrolling();

			drawCounter++;

			if (drawCounter % 5 != 0) return;
			if (10000 < drawCounter) drawCounter = 0;

			var consumed = false;
			// emit events.
			if (manipulateEvents_.keyLeft) {
				//SelectPreviousTack();
				ChangeStart(true);
				consumed = true;
			}
			if (manipulateEvents_.keyRight) {
				//SelectNextTack();
				ChangeStart(false);
				consumed = true;
			}

			if (manipulateEvents_.keyUp) {
				//SelectAheadObject();
				ChangeStartAll(true);
				consumed = true;
			}
			if (manipulateEvents_.keyDown) {
				//SelectBelowObject();
				ChangeStartAll(false);
				consumed = true;
			}

			// renew.
			if (consumed) manipulateEvents_ = new ManipulateEvents();
		}

		void OnGUI() {
			//親閉じたら閉じる
			if (parent_ == null) {
				Close();
				return;
			}

			var viewWidth = this.position.width;
			var viewHeight = this.position.height;

			GUI.BeginGroup(new Rect(0, 0, viewWidth, viewHeight));
			{
				AutoScrollButtons();
				DrawAutoConponent(viewWidth);
				//if (autoScroll_ == enAutoScroll.Stop)
				HandlingMouseEvent();
			}
			GUI.EndGroup();

		}
		void AutoScrolling() {
			if ((autoScroll_ == enAutoScroll.Play)
				|| (autoScroll_ == enAutoScroll.LoopPlay)) {
				int frameCount = (int)((EditorApplication.timeSinceStartup - playStartTime_) * ARIMotionMainWindow.FPS * scrollSpeed_);
				if (frameCount >= lastFrame_) {

					if (autoScroll_ == enAutoScroll.Play) {
						autoScroll_ = enAutoScroll.Stop;//停止
					}
					else if (autoScroll_ == enAutoScroll.LoopPlay) {
						frameCount = 0;
						playStartTime_ = EditorApplication.timeSinceStartup;//振り出し
					}
				}

				selectedPos_ = frameCount * WindowSettings.TACK_FRAME_WIDTH;
				selectedFrame_ = frameCount;
				parent_.RepaintAllWindow();
			}
		}

		void SelectPreviousTack() {
			if (!HasValidScore()) return;

			var score = GetActiveScore();

			if (manipulateTargets.activeObjectIds.Any()) {
				if (manipulateTargets.activeObjectIds.Count == 1) {
					score.SelectPreviousTackOfTimelines(manipulateTargets.activeObjectIds[0]);
				}
				else {
					// select multiple objects.
				}
			}

			if (!manipulateTargets.activeObjectIds.Any()) return;

			var currentSelectedFrame = score.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
			if (0 <= currentSelectedFrame) {
				FocusToFrame(currentSelectedFrame);
			}
		}

		void SelectNextTack() {
			if (!HasValidScore()) return;

			var score = GetActiveScore();
			if (manipulateTargets.activeObjectIds.Any()) {
				if (manipulateTargets.activeObjectIds.Count == 1) {
					score.SelectNextTackOfTimelines(manipulateTargets.activeObjectIds[0]);
				}
				else {
					// select multiple objects.
				}
			}

			if (!manipulateTargets.activeObjectIds.Any()) return;

			var currentSelectedFrame = score.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
			if (0 <= currentSelectedFrame) {
				FocusToFrame(currentSelectedFrame);
			}
		}

		//オブジェクトの選択
		void SelectAheadObject() {
			if (!HasValidScore()) return;

			var score = GetActiveScore();

			// if selecting object is top, select tick. unselect all objects.
			if (score.IsActiveTimelineOrContainsActiveObject(0)) {
				Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_UNSELECTED, null));
				return;
			}

			if (!manipulateTargets.activeObjectIds.Any()) return;
			score.SelectAboveObjectById(manipulateTargets.activeObjectIds[0]);

			var currentSelectedFrame = score.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
			if (0 <= currentSelectedFrame) {
				FocusToFrame(currentSelectedFrame);
			}
		}

		void SelectBelowObject() {
			if (!HasValidScore()) return;

			var score = GetActiveScore();

			if (manipulateTargets.activeObjectIds.Any()) {
				score.SelectBelowObjectById(manipulateTargets.activeObjectIds[0]);
				var currentSelectedFrame = score.GetStartFrameById(manipulateTargets.activeObjectIds[0]);
				if (0 <= currentSelectedFrame) {
					FocusToFrame(currentSelectedFrame);
				}
				return;
			}

			score.SelectTackAtFrame(selectedFrame_);
		}

		//キー入力によるスパンの変更
		void ChangeSpan(int newSpan) {
			if (!HasValidScore()) return;

			var score = GetActiveScore();
			if (manipulateTargets.activeObjectIds.Any()) {
				if (manipulateTargets.activeObjectIds.Count == 1) {
					var tack = score.TackById(manipulateTargets.activeObjectIds[0]);

					//スパン変更はポジションのみ
					//if (tack.timelineType_ == (int)TimelineType.TL_POS)
					{
						Undo.RecordObject(this, "ChangeSpan");
						string id = MethodBase.GetCurrentMethod().Name;
						int lastSpan = tack.span_;

						Action action = () => {
							tack.UpdatePos(tack.start_, newSpan);
							score.SqueezeTack();
						};

						Action undo = () => {
							tack.UpdatePos(tack.start_, lastSpan);
							score.SqueezeTack();
						};

						ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));

						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_SAVE, null));
						parent_.RepaintAllWindow();
					}

				}
				else {
					// select multiple objects.
				}
			}
		}

		//スタート位置の変更
		void ChangeStart(bool isForwerd) {
			if (!HasValidScore()) return;

			var score = GetActiveScore();
			if (manipulateTargets.activeObjectIds.Any()) {
				if (manipulateTargets.activeObjectIds.Count == 1) {
					var tack = score.TackById(manipulateTargets.activeObjectIds[0]);

					if (tack == null) return;

					if ((TimelineType)tack.timelineType_ == TimelineType.TL_POS) return;

					Undo.RecordObject(this, "ChangeSpan");
					int newStart = (isForwerd)
						? tack.start_ - 1
						: tack.start_ + 1;
					int lastStart = tack.start_;

					string id = MethodBase.GetCurrentMethod().Name;
					//int lastSpan = tack.span_;

					Action action = () => {
						tack.UpdatePos(newStart, tack.span_);
						score.SqueezeTack();
					};

					Action undo = () => {
						tack.UpdatePos(lastStart, tack.span_);
						score.SqueezeTack();
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));


					Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_SAVE, null));
					parent_.RepaintAllWindow();


				}
				else {
					// select multiple objects.
				}
			}
		}

		//キー入力によるスパンの変更
		void ChangeStartAll(bool isForwerd) {
			if (!HasValidScore()) return;

			var score = GetActiveScore();
			if (manipulateTargets.activeObjectIds.Any()) {
				if (manipulateTargets.activeObjectIds.Count == 1) {
					var activeId = manipulateTargets.activeObjectIds[0];
					List<TackPoint> tackPoints = null;
					if (IsTimelineId(activeId)) {
						TimelineTrack timelineTrack = score.TimelineById(activeId);
						tackPoints = timelineTrack.tackPoints_;
					}
					else if (IsTackId(activeId)) {
						var tack = score.TackById(activeId);
						tackPoints = score.TimelinesByType((TimelineType)tack.timelineType_);
					}

					if (tackPoints != null) {
						Undo.RecordObject(this, "ChangeSpan");

						List<Action> cmdDo = new List<Action>();
						List<Action> cmdUndo = new List<Action>();
						string id = MethodBase.GetCurrentMethod().Name;

						foreach (var item in tackPoints) {
							TackPoint tack = item;
							int newStart = (isForwerd)
								? item.start_ - 1
								: item.start_ + 1;

							int lastStart = item.start_;


							Action action = () => {
								tack.UpdatePos(newStart, tack.span_);
								score.SqueezeTack();
							};

							Action undo = () => {
								tack.UpdatePos(lastStart, tack.span_);
								score.SqueezeTack();
							};

							//コマンドPos
							cmdDo.Add(action);
							cmdUndo.Add(undo);
						}

						cmdDo.Add(() => score.SqueezeTack());
						cmdUndo.Add(() => score.SqueezeTack());

						if (cmdDo.Any()) {
							ARIMotionMainWindow.tackCmd_.Do(new MotionCommand(id,
								() => { foreach (var cmd in cmdDo) cmd(); },
								() => { foreach (var cmd in cmdUndo) cmd(); }));
						}

						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_SAVE, null));
						parent_.RepaintAllWindow();
					}
				}
				else {
					// select multiple objects.
				}
			}
		}

		//オート再生ボタン
		void AutoScrollButtons() {
			const int MAXWIDTH = 72;
			EditorGUILayout.BeginVertical(GUI.skin.box);
			if (GUILayout.Button("停止", GUILayout.Height(16), GUILayout.MaxWidth(MAXWIDTH))) {
				autoScroll_ = enAutoScroll.Stop;
			}
			if (GUILayout.Button("再生", GUILayout.Height(16), GUILayout.MaxWidth(MAXWIDTH))) {
				autoScroll_ = enAutoScroll.Play;
				PlayStartSetup();
			}
			if (GUILayout.Button("ループ再生", GUILayout.Height(16), GUILayout.MaxWidth(MAXWIDTH))) {
				autoScroll_ = enAutoScroll.LoopPlay;
				PlayStartSetup();
			}
			//再生速度
			scrollSpeed_ = GUILayout.HorizontalSlider(scrollSpeed_, 0.1f, 1.0f, GUILayout.MaxWidth(MAXWIDTH));

			isMovePos_ = GUILayout.Toggle(isMovePos_, "位置移動反映");
			EditorGUILayout.EndVertical();
		}

		void PlayStartSetup() {
			playStartTime_ = EditorApplication.timeSinceStartup;
			lastFrame_ = GetActiveScore().LastFrame();
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_UNSELECTED, null));
		}

		//マウス入力イベント設定
		void HandlingMouseEvent() {
			var repaint = false;
			var useEvent = false;
			switch (Event.current.type) {
				// mouse event handling.
				case EventType.MouseDown: {
					var touchedFrameCount = TimelineTrack.GetFrameOnTimelineFromAbsolutePosX(scrollPos_ + (Event.current.mousePosition.x - WindowSettings.TIMELINE_CONDITIONBOX_SPAN));
					if (touchedFrameCount < 0) touchedFrameCount = 0;
					selectedPos_ = touchedFrameCount * WindowSettings.TACK_FRAME_WIDTH;
					selectedFrame_ = touchedFrameCount;
					repaint = true;

					Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_UNSELECTED, null));

					useEvent = true;
					break;
				}
				case EventType.ContextClick: {
					ShowContextMenu();
					useEvent = true;
					break;
				}
				case EventType.MouseUp: {

					// right click.
					if (Event.current.button == 1) {
						ShowContextMenu();
					}
					else {
						var touchedFrameCount = TimelineTrack.GetFrameOnTimelineFromAbsolutePosX(scrollPos_ + (Event.current.mousePosition.x - WindowSettings.TIMELINE_CONDITIONBOX_SPAN));
						if (touchedFrameCount < 0) touchedFrameCount = 0;
						selectedPos_ = touchedFrameCount * WindowSettings.TACK_FRAME_WIDTH;
						selectedFrame_ = touchedFrameCount;
						repaint = true;
					}

					useEvent = true;

					break;
				}
				case EventType.MouseDrag: {
					var pos = scrollPos_ + (Event.current.mousePosition.x - WindowSettings.TIMELINE_CONDITIONBOX_SPAN);
					if (pos < 0) pos = 0;
					selectedPos_ = pos - ((WindowSettings.TACK_FRAME_WIDTH / 2f) - 1f);
					selectedFrame_ = TimelineTrack.GetFrameOnTimelineFromAbsolutePosX(pos);

					FocusToFrame(selectedFrame_);

					repaint = true;
					useEvent = true;
					break;
				}

				// scroll event handling.
				case EventType.ScrollWheel: {
					if (0 != Event.current.delta.x) {
						scrollPos_ = scrollPos_ + (Event.current.delta.x * 2);
						if (scrollPos_ < 0) scrollPos_ = 0;

						repaint = true;
					}
					useEvent = true;
					break;
				}

				// key event handling.
				case EventType.KeyDown: {
					switch (Event.current.keyCode) {
						case KeyCode.LeftArrow: {
							if (manipulateTargets.activeObjectIds.Count == 0) {

								selectedFrame_ = selectedFrame_ - 1;
								if (selectedFrame_ < 0) selectedFrame_ = 0;
								selectedPos_ = selectedFrame_ * WindowSettings.TACK_FRAME_WIDTH;
								repaint = true;

								FocusToFrame(selectedFrame_);
							}
							manipulateEvents_.keyLeft = true;
							useEvent = true;
							break;
						}
						case KeyCode.RightArrow: {
							if (manipulateTargets.activeObjectIds.Count == 0) {
								selectedFrame_ = selectedFrame_ + 1;
								selectedPos_ = selectedFrame_ * WindowSettings.TACK_FRAME_WIDTH;
								repaint = true;

								FocusToFrame(selectedFrame_);
							}
							manipulateEvents_.keyRight = true;
							useEvent = true;
							break;
						}
						case KeyCode.UpArrow: {
							manipulateEvents_.keyUp = true;
							useEvent = true;
							break;
						}
						case KeyCode.DownArrow: {
							manipulateEvents_.keyDown = true;
							useEvent = true;
							break;
						}
						case KeyCode.S:
							parent_.SaveData2(true);
							break;
						case KeyCode.L:
							parent_.ReloadSavedData();
							break;
						case KeyCode.Z:
							ARIMotionMainWindow.scoreCmd_.Undo();//Undo
							repaint = true;
							break;
						case KeyCode.Y:
							ARIMotionMainWindow.scoreCmd_.Redo();//Redo
							repaint = true;
							break;
							//case KeyCode.Keypad1:
							//case KeyCode.Keypad2:
							//case KeyCode.Keypad3:
							//case KeyCode.Keypad4:
							//case KeyCode.Keypad5:
							//case KeyCode.Keypad6:
							//case KeyCode.Keypad7:
							//case KeyCode.Keypad8:
							//case KeyCode.Keypad9:
							//case KeyCode.Keypad0:
							//	{
							//		manipulateEvents_.SetKeyNum(Event.current.keyCode);
							//		useEvent = true;
							//		break;
							//	}
					}
					break;
				}
			}

			// update cursor pos
			cursorPos_ = selectedPos_ - scrollPos_;

			if (repaint) {
				//HandleUtility.Repaint();
				parent_.RepaintAllWindow();
				//repaint = false;
			}

			if (eventStacks_.Any()) {
				foreach (var onTrackEvent in eventStacks_) EmitAfterDraw(onTrackEvent);
				eventStacks_.Clear();
				parent_.NeedSave();
			}

			if (useEvent) Event.current.Use();
		}

		//右クリックメニュー
		void ShowContextMenu() {
			var nearestTimelineIndex = 0;// fixed. should change by mouse position.

			var menu = new GenericMenu();

			if (HasValidScore()) {
				var currentScore = GetActiveScore();
				var scoreId = currentScore.scoreGuid_;

				//各種タイムライン
				var menuItems = new Dictionary<string, OnTrackEvent.EventType>{
					{"Add New Pos", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_POS},
					{"Add New Transform", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_TRANSFORM},
					{"Add New Move", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_MOVE},
					//{"Add New Atari", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_ATARI},
					//{"Add New Hold", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_HOLD},
					//{"Add New Throw", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_THROW},
					{"Add New Color", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_COLOR},
					{"Add New Effect", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_EFFECT},
					{"Add New Passive", OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_PASSIVE},
				};

				foreach (var key in menuItems.Keys) {
					var eventType = menuItems[key];
					var enable = IsEnableEvent(eventType);
					if (enable) {
						menu.AddItem(
						new GUIContent(key),
						false,
						() => {
							Emit(new OnTrackEvent(eventType, scoreId, nearestTimelineIndex));
						}
					);
					}
					else {
						menu.AddDisabledItem(new GUIContent(key));
					}
				}
			}

			menu.ShowAsContext();
		}

		//右クリックメニューの可不可
		bool IsEnableEvent(OnTrackEvent.EventType eventType) {
			int timelineType = 0;

			//既に存在する種類のタイムラインは作成できないように
			//各種タイムライン
			switch (eventType) {
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_POS:
					timelineType = (int)TimelineType.TL_POS; break;
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_TRANSFORM:
					timelineType = (int)TimelineType.TL_TRANSFORM; break;
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_MOVE:
					timelineType = (int)TimelineType.TL_MOVE; break;
				//case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_ATARI:
				//	timelineType = (int)TimelineType.TL_ATARI; break;
				//case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_HOLD:
				//	timelineType = (int)TimelineType.TL_HOLD; break;
				//case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_THROW:
				//	timelineType = (int)TimelineType.TL_THROW; break;
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_COLOR:
					timelineType = (int)TimelineType.TL_COLOR; break;
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_EFFECT:
					timelineType = (int)TimelineType.TL_EFFECT; break;
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_PASSIVE:
					timelineType = (int)TimelineType.TL_PASSIVE; break;
			}

			return !GetActiveScore().timelineTracks_
				.Where(t => t.IsExistTimeline_)
				.Where(t => t.timelineType_ == timelineType)
				.Any();
		}

		void Emit(OnTrackEvent onTrackEvent) {
			var type = onTrackEvent.eventType;
			// tack events.
			switch (type) {
				case OnTrackEvent.EventType.EVENT_UNSELECTED: {
					manipulateTargets = new ManipulateTargets(new List<string>());

					Undo.RecordObject(this, "Unselect");

					var activeAuto = GetActiveScore();
					activeAuto.DeactivateAllObjects();
					activeAuto.SetScoreInspector();
					parent_.RepaintAllWindow();
					return;
				}
				case OnTrackEvent.EventType.EVENT_OBJECT_SELECTED: {
					manipulateTargets = new ManipulateTargets(new List<string> { onTrackEvent.activeObjectId });

					var activeAuto = GetActiveScore();

					Undo.RecordObject(this, "Select");
					activeAuto.ActivateObjectsAndDeactivateOthers(manipulateTargets.activeObjectIds);
					parent_.RepaintAllWindow();
					return;
				}

				//各種タイムライン
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_POS:
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_TRANSFORM:
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_MOVE:
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_COLOR:
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_EFFECT:
				case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_PASSIVE: {
					int timelineType = 0;
					//各種タイムライン
					switch (type) {
						case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_POS:
							timelineType = (int)TimelineType.TL_POS; break;
						case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_TRANSFORM:
							timelineType = (int)TimelineType.TL_TRANSFORM; break;
						case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_MOVE:
							timelineType = (int)TimelineType.TL_MOVE; break;
						case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_COLOR:
							timelineType = (int)TimelineType.TL_COLOR; break;
						case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_EFFECT:
							timelineType = (int)TimelineType.TL_EFFECT; break;
						case OnTrackEvent.EventType.EVENT_SCORE_ADDTIMELINE_PASSIVE:
							timelineType = (int)TimelineType.TL_PASSIVE; break;
					}

					var activeAuto = GetActiveScore();
					var tackPoints = new List<TackPoint>();
					var newTimeline = new TimelineTrack(activeAuto.timelineTracks_.Count, timelineType, tackPoints);
					var newTimelineId = newTimeline.timelineId_;
					Undo.RecordObject(this, "Add Timeline");

					//activeAuto.timelineTracks.Add(newTimeline);

					string id = MethodBase.GetCurrentMethod().Name;

					Action action = () => {
						activeAuto.timelineTracks_.Add(newTimeline);
					};

					Action undo = () => {
						activeAuto.DeleteObjectById(newTimelineId, false);
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));

					parent_.NeedSave();
					return;
				}

				case OnTrackEvent.EventType.EVENT_SCORE_BEFORESAVE: {
					Undo.RecordObject(this, "Update Score Title");
					return;
				}

				case OnTrackEvent.EventType.EVENT_SCORE_SAVE: {
					parent_.NeedSave();
					return;
				}

				/*
					timeline events.
				*/
				case OnTrackEvent.EventType.EVENT_TIMELINE_ADDTACK: {
					eventStacks_.Add(onTrackEvent.Copy());
					return;
				}


				case OnTrackEvent.EventType.EVENT_TIMELINE_PASTETACK: {
					eventStacks_.Add(onTrackEvent.Copy());
					return;
				}

				case OnTrackEvent.EventType.EVENT_TIMELINE_DELETE: {
					var targetTimelineId = onTrackEvent.activeObjectId;
					var activeAuto = GetActiveScore();


					Undo.RecordObject(this, "Delete Timeline");

					string id = MethodBase.GetCurrentMethod().Name;

					Action action = () => {
						activeAuto.DeleteObjectById(targetTimelineId, false);
					};

					Action undo = () => {
						activeAuto.DeleteObjectById(targetTimelineId, true);
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));


					//activeAuto.DeleteObjectById(targetTimelineId);
					parent_.NeedSave();
					return;
				}
				case OnTrackEvent.EventType.EVENT_TIMELINE_BEFORESAVE: {
					Undo.RecordObject(this, "Update Timeline Title");
					return;
				}

				case OnTrackEvent.EventType.EVENT_TIMELINE_SAVE: {
					parent_.NeedSave();
					return;
				}


				/*
					tack events.
				*/
				case OnTrackEvent.EventType.EVENT_TACK_MOVING: {
					var movingTackId = onTrackEvent.activeObjectId;

					var activeAuto = GetActiveScore();

					activeAuto.SetMovingTackToTimelimes(movingTackId);
					break;
				}
				case OnTrackEvent.EventType.EVENT_TACK_MOVED: {

					Undo.RecordObject(this, "Move Tack");

					return;
				}
				case OnTrackEvent.EventType.EVENT_TACK_MOVED_AFTER: {
					var targetTackId = onTrackEvent.activeObjectId;

					var activeAuto = GetActiveScore();
					var activeTimelineIndex = activeAuto.GetTackContainedTimelineIndex(targetTackId);
					if (0 <= activeTimelineIndex) {
						//タックの移動後処理
						activeAuto.timelineTracks_[activeTimelineIndex].UpdateByTackMoved(targetTackId);

						//Repaint();
						parent_.NeedSave();
					}
					return;
				}
				case OnTrackEvent.EventType.EVENT_TACK_DELETED: {
					var targetTackId = onTrackEvent.activeObjectId;
					var activeAuto = GetActiveScore();

					Undo.RecordObject(this, "Delete Tack");


					string id = MethodBase.GetCurrentMethod().Name;

					Action action = () => {
						activeAuto.DeleteObjectById(targetTackId, false);
						activeAuto.SqueezeTack();
					};

					Action undo = () => {
						activeAuto.DeleteObjectById(targetTackId, true);
						activeAuto.SqueezeTack();
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));

					//Repaint();
					parent_.NeedSave();
					return;
				}

				case OnTrackEvent.EventType.EVENT_TACK_COPY: {
					var targetTackId = onTrackEvent.activeObjectId;
					var activeAuto = GetActiveScore();
					clipTack_ = activeAuto.GetTackById(targetTackId);
					return;
				}


				case OnTrackEvent.EventType.EVENT_TACK_BEFORESAVE: {
					Undo.RecordObject(this, "Update Tack Title");
					return;
				}

				case OnTrackEvent.EventType.EVENT_TACK_SAVE: {
					parent_.NeedSave();
					return;
				}


				case OnTrackEvent.EventType.EVENT_TACK_CHANGE: {
					parent_.NeedSave();
					parent_.subWindow_.SetupPartsData(false);//サブウインドウにも反映させる
					parent_.RepaintAllWindow();
					return;
				}


				default: {
					//親に投げる
					ParentEmit(onTrackEvent);
					//Debug.LogError("no match type:" + type);
					break;
				}
			}
		}

		ScoreComponent GetActiveScore() {
			return parent_.GetActiveScore();
		}

		void EmitAfterDraw(OnTrackEvent onTrackEvent) {
			var type = onTrackEvent.eventType;
			switch (type) {
				case OnTrackEvent.EventType.EVENT_TIMELINE_ADDTACK: {
					var targetTimelineId = onTrackEvent.activeObjectId;
					var targetFramePos = onTrackEvent.frame;

					var activeAuto = GetActiveScore();

					Undo.RecordObject(this, "Add Tack");

					string id = MethodBase.GetCurrentMethod().Name;

					TackPoint newTackPoint = activeAuto.NewTackToTimeline(targetTimelineId, targetFramePos);
					var delId = newTackPoint.tackId_;
					Action action = () => {
						activeAuto.PasteTackToTimeline(targetTimelineId, targetFramePos, newTackPoint);
						activeAuto.SqueezeTack();
					};

					Action undo = () => {
						activeAuto.DeleteObjectById(delId, false);
						activeAuto.SqueezeTack();
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));


					return;
				}

				case OnTrackEvent.EventType.EVENT_TIMELINE_PASTETACK: {

					var targetTimelineId = onTrackEvent.activeObjectId;
					var targetFramePos = onTrackEvent.frame;
					var activeAuto = GetActiveScore();

					Undo.RecordObject(this, "Paste Tack");

					//activeAuto.PasteTackToTimeline(targetTimelineId, targetFramePos, clipTack_);
					//activeAuto.SqueezeTack();

					string id = MethodBase.GetCurrentMethod().Name;

					TackPoint newTackPoint = clipTack_;

					Action action = () => {
						activeAuto.PasteTackToTimeline(targetTimelineId, targetFramePos, newTackPoint);
						activeAuto.SqueezeTack();
					};

					Action undo = () => {
						activeAuto.DeleteObjectById(newTackPoint.tackId_, false);
						activeAuto.SqueezeTack();
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));

					return;
				}
			}
		}

		void FocusToFrame(int focusTargetFrame) {
			var leftFrame = (int)Math.Round(scrollPos_ / WindowSettings.TACK_FRAME_WIDTH);
			var rightFrame = (int)(((scrollPos_ + (position.width - WindowSettings.TIMELINE_CONDITIONBOX_SPAN)) / WindowSettings.TACK_FRAME_WIDTH) - 1);

			// left edge of view - leftFrame - rightFrame - right edge of view

			if (focusTargetFrame < leftFrame) {
				scrollPos_ = scrollPos_ - ((leftFrame - focusTargetFrame) * WindowSettings.TACK_FRAME_WIDTH);
				return;
			}

			if (rightFrame < focusTargetFrame) {
				scrollPos_ = scrollPos_ + ((focusTargetFrame - rightFrame) * WindowSettings.TACK_FRAME_WIDTH);
				return;
			}
		}
		string GetScriptableObjectFilePath() {
			return "Assets/" + TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_PATH + "TimeFlow.asset";
			//return Path.Combine(Application.dataPath, TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_PATH + "TimeFlow.asset");
		}


	}

}
