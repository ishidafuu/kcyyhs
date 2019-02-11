using UnityEngine;
using UnityEditor;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace NKKD.EDIT {
	public class SubWindowInspector : ScriptableObject {
		public string title;
		public TackPoint tackPoint;
		public void UpdateSubWindow(TackPoint tackPoint)
		{
			this.tackPoint = tackPoint;
		}
	}

	[CustomEditor(typeof(SubWindowInspector))]
	public class SubWindowInspectorGUI : Editor
	{
		public override void OnInspectorGUI()
		{
			var insp = (SubWindowInspector)target;
			var tackPoint = insp.tackPoint;
			//UpdateTackTitle(tackPoint);

			DrawTackSpan(tackPoint);
		}

		private void UpdateTackTitle(TackPoint tackPoint)
		{
			//var newTitle = EditorGUILayout.TextField("title", tackPoint.title_);
			//if (newTitle != tackPoint.title_)
			//{
			//	tackPoint.BeforeSave();
			//	tackPoint.title_ = newTitle;
			//	tackPoint.Save();
			//}
		}
		private void DrawTackSpan(TackPoint tackPoint)
		{
			var start = tackPoint.start_;
			GUILayout.Label("start:" + start);
			var span = tackPoint.span_;
			var end = start + span - 1;
			GUILayout.Label("end:" + end);
			GUILayout.Label("span:" + span);
		}
	}
}