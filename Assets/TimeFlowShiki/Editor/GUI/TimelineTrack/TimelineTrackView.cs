using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class TimelineTrackView
	{
		TimelineTrackModel model = new TimelineTrackModel();
		TimelineTrackViewModel viewModel = new TimelineTrackViewModel();

		public void SetModels(TimelineTrackModel model, TimelineTrackViewModel viewModel)
		{
			this.model = model;
			this.viewModel = viewModel;
		}

		public static int GetFrameOnTimelineFromAbsolutePosX(float frameSourceX)
		{
			return (int)(frameSourceX / WindowSettings.TACK_FRAME_WIDTH);
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

		public void InitializeTextResource()
		{
			viewModel.timelineConditionTypeLabelStyle_ = new GUIStyle();
			viewModel.timelineConditionTypeLabelStyle_.normal.textColor = Color.white;
			viewModel.timelineConditionTypeLabelStyle_.fontSize = 16;
			viewModel.timelineConditionTypeLabelStyle_.alignment = TextAnchor.MiddleCenter;

			viewModel.timelineConditionTypeLabelSmallStyle_ = new GUIStyle();
			viewModel.timelineConditionTypeLabelSmallStyle_.normal.textColor = Color.white;
			viewModel.timelineConditionTypeLabelSmallStyle_.fontSize = 10;
			viewModel.timelineConditionTypeLabelSmallStyle_.alignment = TextAnchor.MiddleCenter;
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
				foreach (var tackPoint in model.tackPoints_)
				{
					var isUnderEvent = viewModel.movingTackIds_.Contains(tackPoint.GetTackId());
					if (!viewModel.movingTackIds_.Any())isUnderEvent = true;

					// draw tackPoint on the frame.
					tackPoint.view.DrawTack(limitRect, model.timelineId_, timelineScrollX, timelineBodyY, isUnderEvent);
					index++;
				}
			}
		}

		public void ApplyTextureToTacks(int texIndex)
		{
			viewModel.timelineBaseTexture_ = GetTimelineTexture(texIndex);
			foreach (var tackPoint in model.tackPoints_)
			{
				tackPoint.InitializeTackTexture(viewModel.timelineBaseTexture_);
			}
		}

		public static Texture2D GetTimelineTexture(int textureIndex)
		{
			var color = WindowSettings.RESOURCE_COLORS_SOURCES[textureIndex % WindowSettings.RESOURCE_COLORS_SOURCES.Count];
			var colorTex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			colorTex.SetPixel(0, 0, color);
			colorTex.Apply();

			return colorTex;
		}

		public float DrawTimelineTrack(float headWall, float timelineScrollX, float yOffsetPos, float width)
		{
			viewModel.timelineScrollX_ = timelineScrollX;

			viewModel.trackRect_.width = width;
			viewModel.trackRect_.y = yOffsetPos;

			if (viewModel.trackRect_.y < headWall)viewModel.trackRect_.y = headWall;

			if (viewModel.timelineBaseTexture_ == null)ApplyTextureToTacks(model.index_);

			viewModel.trackRect_ = GUI.Window(model.index_, viewModel.trackRect_, WindowEventCallback, string.Empty, "AnimationKeyframeBackground");
			return viewModel.trackRect_.height;
		}

		void WindowEventCallback(int id)
		{
			// draw bg from header to footer.
			{
				if (model.active_)
				{
					var headerBGActiveRect = new Rect(0f, 0f, viewModel.trackRect_.width, WindowSettings.TIMELINE_HEIGHT);
					GUI.DrawTexture(headerBGActiveRect, WindowSettings.activeTackBaseTex);

					var headerBGRect = new Rect(1f, 1f, viewModel.trackRect_.width - 1f, WindowSettings.TIMELINE_HEIGHT - 2f);
					GUI.DrawTexture(headerBGRect, WindowSettings.timelineHeaderTex);
				}
				else
				{
					var headerBGRect = new Rect(0f, 0f, viewModel.trackRect_.width, WindowSettings.TIMELINE_HEIGHT);
					GUI.DrawTexture(headerBGRect, WindowSettings.timelineHeaderTex);
				}
			}

			var timelineBodyY = WindowSettings.TIMELINE_HEADER_HEIGHT;

			// timeline condition type box.	
			var conditionBGRect = new Rect(1f, timelineBodyY, WindowSettings.TIMELINE_CONDITIONBOX_WIDTH - 1f, WindowSettings.TACK_HEIGHT - 1f);
			if (model.active_)
			{
				var conditionBGRectInActive = new Rect(1f, timelineBodyY, WindowSettings.TIMELINE_CONDITIONBOX_WIDTH - 1f, WindowSettings.TACK_HEIGHT - 1f);
				GUI.DrawTexture(conditionBGRectInActive, viewModel.timelineBaseTexture_);
			}
			else
			{
				GUI.DrawTexture(conditionBGRect, viewModel.timelineBaseTexture_);
			}

			string timelineTypeName = ((TimelineType)model.timelineType_).ToString();

			if ((viewModel.timelineConditionTypeLabelStyle_ == null)
				|| (viewModel.timelineConditionTypeLabelSmallStyle_ == null))
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
				viewModel.timelineConditionTypeLabelSmallStyle_
			);

			var frameRegionWidth = viewModel.trackRect_.width - WindowSettings.TIMELINE_CONDITIONBOX_SPAN;

			// draw frame back texture & TackPoint datas on frame.
			GUI.BeginGroup(new Rect(WindowSettings.TIMELINE_CONDITIONBOX_SPAN, timelineBodyY, viewModel.trackRect_.width, WindowSettings.TACK_HEIGHT));
			{
				DrawFrameRegion(viewModel.timelineScrollX_, 0f, frameRegionWidth);
			}
			GUI.EndGroup();

			var useEvent = false;

			// mouse manipulation.
			switch (Event.current.type)
			{

				case EventType.ContextClick:
					{
						ShowContextMenu(viewModel.timelineScrollX_);
						useEvent = true;
						break;
					}

					// clicked.
				case EventType.MouseUp:
					{

						// is right clicked
						if (Event.current.button == 1)
						{
							ShowContextMenu(viewModel.timelineScrollX_);
							useEvent = true;
							break;
						}

						TimelineTrack.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, model.timelineId_));
						useEvent = true;
						break;
					}
			}

			// constraints.
			viewModel.trackRect_.x = 0;
			if (viewModel.trackRect_.y < 0)viewModel.trackRect_.y = 0;

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
							TimelineTrack.Emit(new OnTrackEvent(eventType, model.timelineId_, targetFrame));
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
						foreach (var tackPoint in model.tackPoints_)
						{
							if (tackPoint.ContainsFrame(frame))
							{
								if (!tackPoint.GetIsExistTack())return true;
								return false;
							}
						}
						return true;
					}
				case OnTrackEvent.EventType.EVENT_TIMELINE_DELETE:
					{
						return true;
						//return (model.timelineType_ != (int)(TimelineType.TL_POS));
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

	}
}