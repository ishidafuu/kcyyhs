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
			//UpdateTimelineTrackTitle(score);
		}

		//private void UpdateTimelineTrackTitle(ScoreComponent score)
		//{
		//	//���C���X�y�N�^�̕\��
		//	var newTitle = EditorGUILayout.TextField("title", score.title_);
		//	//var charManager = EditorGUILayout.ObjectField("charManager", timelineTrack.charManager, typeof(JMCharManager),true);
		//	//EditorGUILayout.LabelField("type : " + score.timelineType.ToString());
		//	if (newTitle != score.title_)
		//	{
		//		score.BeforeSave();
		//		//score.title_ = newTitle;
		//		score.Save();
		//	}
		//}
	}
}