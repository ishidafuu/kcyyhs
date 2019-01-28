using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class TackPointInspector : ScriptableObject
	{
		public TackPoint tackPoint;

		public void UpdateTackPoint(TackPoint tackPoint)
		{
			this.tackPoint = tackPoint;

		}
	}

	[CustomEditor(typeof(TackPointInspector))]
	public class TackPointInspectorGUI : Editor
	{
		TackPointInspectorPos pos = new TackPointInspectorPos();
		TackPointInspectorTransform transform = new TackPointInspectorTransform();
		TackPointInspectorEffect effect = new TackPointInspectorEffect();
		TackPointInspectorAni ani = new TackPointInspectorAni();

		public override void OnInspectorGUI()
		{
			var insp = (TackPointInspector)target;

			var tackPoint = insp.tackPoint;

			DrawTackSpan(tackPoint);

			switch ((TimelineType)tackPoint.GetTimelineType())
			{
				case TimelineType.TL_POS:
					pos.Draw(tackPoint);
					break;
				case TimelineType.TL_TRANSFORM:
					transform.Draw(tackPoint);
					break;
				case TimelineType.TL_COLOR:
					ani.Draw(tackPoint);
					break;
				case TimelineType.TL_EFFECT:
					effect.Draw(tackPoint);
					break;
				default:
					Debug.LogError(tackPoint.GetTimelineType().ToString());
					break;
			}

			UndoKey();

		}

		private void UndoKey()
		{
			if (Event.current.type != EventType.KeyDown)return;

			if (Event.current.keyCode == KeyCode.Z)
			{
				ARIMotionMainWindow.tackCmd_.Undo();
				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
			else if (Event.current.keyCode == KeyCode.Y)
			{
				ARIMotionMainWindow.tackCmd_.Redo();
				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}

		}

		private Vector2 RoundPosVector(Vector2 pos)
		{
			return new Vector2((int)pos.x, (int)pos.y);
		}

		private void DrawTackSpan(TackPoint tackPoint)
		{
			var start = tackPoint.GetStart();
			GUILayout.Label("start : " + start);
			var span = tackPoint.GetSpan();
			var end = start + span - 1;
			GUILayout.Label("end : " + end);
			GUILayout.Label("span : " + span);
		}

		private List<string> GetAllMotionList()
		{
			return ARIMotionMainWindow.fileList_;
		}

		private int GetMotionIndex(string motionId)
		{
			int res = 0;
			var item = GetAllMotionList()
				.Select((x, i) => new { x, i })
				.Where(xi => xi.x == motionId)
				.FirstOrDefault();

			if (item != null)res = item.i;

			return res;
			//var motionId = EditorGUILayout.Popup("motionId", selectedIndex, JMMotionMainWindow.fileList_.ToArray());
		}

	}
}