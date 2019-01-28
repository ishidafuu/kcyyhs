using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class SubWindowInspector : ScriptableObject
	{
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
			DrawTackSpan(tackPoint);
		}

		private void UpdateTackTitle(TackPoint tackPoint)
		{

		}
		private void DrawTackSpan(TackPoint tackPoint)
		{
			var start = tackPoint.GetStart();
			GUILayout.Label("start:" + start);
			var span = tackPoint.GetSpan();
			var end = start + span - 1;
			GUILayout.Label("end:" + end);
			GUILayout.Label("span:" + span);
		}
	}
}