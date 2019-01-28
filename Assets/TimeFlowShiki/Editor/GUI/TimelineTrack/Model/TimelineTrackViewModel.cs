using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class TimelineTrackViewModel
	{
		public Rect trackRect_;
		public Texture2D timelineBaseTexture_;
		public float timelineScrollX_;
		public GUIStyle timelineConditionTypeLabelStyle_ = null;
		public GUIStyle timelineConditionTypeLabelSmallStyle_ = null;
		public List<string> movingTackIds_ = new List<string>();
	}
}