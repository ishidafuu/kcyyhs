using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class ScoreComponentInspector : ScriptableObject
	{
		public ScoreComponent score;
		public void UpdateTimelineTrack(ScoreComponent score)
		{
			this.score = score;
		}
	}

	[CustomEditor(typeof(ScoreComponentInspector))]
	public class ScoreComponentInspectorGUI : Editor
	{
		public override void OnInspectorGUI()
		{
			var insp = (ScoreComponentInspector)target;
			var score = insp.score;
		}
	}
}