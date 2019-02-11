using UnityEngine;
using System.Collections.Generic;

namespace NKKD.EDIT {
	public class TimelineTrackInspector : ScriptableObject {
		public TimelineTrack timelineTrack;

		public void UpdateTimelineTrack (TimelineTrack timelineTrack) {
			this.timelineTrack = timelineTrack;
		}
	}
}