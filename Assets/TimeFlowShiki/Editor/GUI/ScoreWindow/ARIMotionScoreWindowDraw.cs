using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MiniJSONForTimeFlowShiki;
using System.Reflection;

namespace NKKD.EDIT
{
	public partial class ARIMotionScoreWindow : EditorWindow
	{
		//スコア、タイムライン、タックの描画
		void DrawAutoConponent(float viewWidth) {
			var xScrollIndex = -scrollPos_;
			float yOffsetPos = 16;

			//描画
			// draw header.
			var inspectorRect = DrawConditionInspector(xScrollIndex, yOffsetPos, viewWidth);



			yOffsetPos = inspectorRect.y + inspectorRect.height + 24f;

			if (HasValidScore()) {
				var activeAuto = GetActiveScore();
				// draw timelines
				DrawTimelines(activeAuto, yOffsetPos, xScrollIndex, viewWidth);

				yOffsetPos += activeAuto.TimelinesTotalHeight();

				// draw tick
				DrawTick();
			}
		}


		Rect DrawConditionInspector(float xScrollIndex, float yIndex, float inspectorWidth) {
			var width = inspectorWidth - WindowSettings.TIMELINE_CONDITIONBOX_SPAN;
			var height = yIndex;

			var assumedHeight = height
				 + WindowSettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT
				 + WindowSettings.CONDITION_INSPECTOR_FRAMELINE_HEIGHT
				 + AssumeConditionLineHeight();

			GUI.BeginGroup(new Rect(WindowSettings.TIMELINE_CONDITIONBOX_SPAN, height, width, assumedHeight));
			{
				var internalHeight = 0f;

				// count & frame in header.
				{
					TimelineTrack.DrawFrameBG(xScrollIndex, internalHeight, width, WindowSettings.CONDITION_INSPECTOR_FRAMELINE_HEIGHT, true);
					internalHeight = internalHeight + WindowSettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT + WindowSettings.CONDITION_INSPECTOR_FRAMELINE_HEIGHT;
				}
				if (HasValidScore()) {
					var currentScore = GetActiveScore();
					var timelines = currentScore.timelineTracks_;
					foreach (var timeline in timelines) {
						if (!timeline.IsExistTimeline_) continue;
						internalHeight = internalHeight + WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;

						DrawConditionLine(0, xScrollIndex, timeline, internalHeight);
						internalHeight = internalHeight + WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT;
					}

					if (timelines.Any()) {
						// add footer.
						internalHeight = internalHeight + WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;
					}
				}
			}
			GUI.EndGroup();

			return new Rect(0, 0, inspectorWidth, assumedHeight);
		}

		void DrawTimelines(ScoreComponent activeAuto, float yOffsetPos, float xScrollIndex, float viewWidth) {
			BeginWindows();
			activeAuto.DrawTimelines(activeAuto, yOffsetPos, xScrollIndex, viewWidth);
			EndWindows();
		}

		void DrawTick() {
			GUI.BeginGroup(new Rect(WindowSettings.TIMELINE_CONDITIONBOX_SPAN, 0f, position.width - WindowSettings.TIMELINE_CONDITIONBOX_SPAN, position.height));
			{
				// tick
				GUI.DrawTexture(new Rect(cursorPos_ + (WindowSettings.TACK_FRAME_WIDTH / 2f) - 1f, 0f, 3f, position.height), WindowSettings.tickTex);

				// draw frame count.
				if (selectedFrame_ == 0) {
					GUI.Label(new Rect(cursorPos_ + 5f, 1f, 10f, WindowSettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), "0", activeFrameLabelStyle_);
				}
				else {
					var span = 0;
					var selectedFrameStr = selectedFrame_.ToString();
					if (2 < selectedFrameStr.Length) span = ((selectedFrameStr.Length - 2) * 8) / 2;
					GUI.Label(new Rect(cursorPos_ + 2 - span, 1f, selectedFrameStr.Length * 10, WindowSettings.CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT), selectedFrameStr, activeFrameLabelStyle_);
				}
			}
			GUI.EndGroup();
		}

		float AssumeConditionLineHeight() {
			var height = 0f;

			if (HasValidScore()) {
				var currentScore = GetActiveScore();
				var timelines = currentScore.timelineTracks_;
				for (var i = 0; i < timelines.Count; i++) {
					if (!timelines[i].IsExistTimeline_) continue;
					height = height + WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;
					height = height + WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT;
				}

				if (timelines.Any()) {
					// add footer.
					height = height + WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_SPAN;
				}
			}

			return height;
		}

		void DrawConditionLine(float xOffset, float xScrollIndex, TimelineTrack timeline, float yOffset) {
			foreach (var tack in timeline.tackPoints_) {
				if (!tack.IsExistTack_) continue;

				var start = tack.start_;
				var span = tack.span_;

				var startPos = xOffset + xScrollIndex + (start * WindowSettings.TACK_FRAME_WIDTH);
				var length = span * WindowSettings.TACK_FRAME_WIDTH;
				var tex = tack.GetColorTex();

				// draw background.
				if (tack.IsActive()) {
					var condtionLineBgRect = new Rect(startPos, yOffset - 1, length, WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT + 2);
					GUI.DrawTexture(condtionLineBgRect, WindowSettings.conditionLineBgTex);
				}
				else {
					var condtionLineBgRect = new Rect(startPos, yOffset + 1, length, WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT - 2);
					GUI.DrawTexture(condtionLineBgRect, WindowSettings.conditionLineBgTex);
				}

				// fill color.
				var condtionLineRect = new Rect(startPos + 1, yOffset, length - 2, WindowSettings.CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT);
				GUI.DrawTexture(condtionLineRect, tex);
			}

		}

	}

}
