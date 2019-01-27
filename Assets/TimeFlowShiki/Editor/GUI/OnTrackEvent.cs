using UnityEngine;

namespace NKKD.EDIT
{
	public class OnTrackEvent
	{
		public enum EventType : int
		{
			EVENT_NONE,
			//EVENT_CREATEOBJECT,
			EVENT_MAIN_ADDSCORE,

			EVENT_SCORE_BEFORESAVE,
			EVENT_SCORE_SAVE,
			EVENT_SCORE_ADDTIMELINE_POS,
			EVENT_SCORE_ADDTIMELINE_TRANSFORM,
			EVENT_SCORE_ADDTIMELINE_MOVE,
			EVENT_SCORE_ADDTIMELINE_COLOR,
			EVENT_SCORE_ADDTIMELINE_EFFECT,
			EVENT_SCORE_ADDTIMELINE_PASSIVE,

			EVENT_TIMELINE_ADDTACK,
			EVENT_TIMELINE_PASTETACK,
			EVENT_TIMELINE_DELETE,
			EVENT_TIMELINE_BEFORESAVE,
			EVENT_TIMELINE_SAVE,

			EVENT_TACK_MOVING,
			EVENT_TACK_MOVED,
			EVENT_TACK_MOVED_AFTER,
			EVENT_TACK_DELETED,
			EVENT_TACK_COPY,
			EVENT_TACK_BEFORESAVE,
			EVENT_TACK_SAVE,
			EVENT_TACK_CHANGE,

			EVENT_OBJECT_SELECTED,
			EVENT_UNSELECTED,

			EVENT_PARTS_MOVED,
			EVENT_PARTS_COPY,
			EVENT_PARTS_PASTE,
		}

		public readonly EventType eventType;
		public readonly string activeObjectId;
		public readonly int frame;

		public OnTrackEvent(OnTrackEvent.EventType eventType, string activeObjectId, int frame = -1)
		{
			this.eventType = eventType;
			this.activeObjectId = activeObjectId;
			this.frame = frame;
		}

		public OnTrackEvent Copy()
		{
			return new OnTrackEvent(this.eventType, this.activeObjectId, this.frame);
		}
	}
}