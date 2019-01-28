using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	[Serializable]
	public class TimelineTrackModel
	{
		[SerializeField]
		public int index_;
		[SerializeField]
		public bool IsExistTimeline_;
		[SerializeField]
		public bool active_;
		[SerializeField]
		public string timelineId_;
		[SerializeField]
		public bool haveActiveTack_;
		[SerializeField]
		public List<TackPoint> tackPoints_ = new List<TackPoint>();
		[SerializeField]
		public int timelineType_;
	}
}