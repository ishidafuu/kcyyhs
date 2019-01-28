using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
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
}