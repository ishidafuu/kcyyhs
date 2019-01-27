using System.Collections.Generic;
using UnityEngine;

namespace NKKD.EDIT
{
	public class WindowSettings
	{
		public const float CONDITION_INSPECTOR_BOX_WIDTH = 80f;

		public const float CONDITION_INSPECTOR_HEADER_HEIGHT = 40f;
		public const float CONDITION_INSPECTOR_FRAMECOUNT_HEIGHT = 15f;
		public const float CONDITION_INSPECTOR_FRAMELINE_HEIGHT = 18f;

		public const float CONDITION_INSPECTOR_CONDITIONLINE_SPAN = 7f;
		public const float CONDITION_INSPECTOR_CONDITIONLINE_HEIGHT = 4f;

		public const float ROUTINE_HEIGHT_DEFAULT = 64f;

		public const float TACK_5FRAME_WIDTH = 80f;
		public const float TACK_FRAME_WIDTH = TACK_5FRAME_WIDTH / 5;
		public const float TACK_FRAME_HEIGHT = 32f;
		public const float TACK_HEIGHT = TACK_FRAME_HEIGHT + ROUTINE_HEIGHT_DEFAULT;
		public const float TACK_POINT_SIZE = 13f;

		public const float TIMELINE_HEADER_HEIGHT = 1f;
		public const float TIMELINE_HEIGHT = TACK_HEIGHT + TIMELINE_HEADER_HEIGHT;
		public const float TIMELINE_CONDITIONBOX_WIDTH = CONDITION_INSPECTOR_BOX_WIDTH;
		public const float TIMELINE_CONDITIONBOX_SPAN = TIMELINE_CONDITIONBOX_WIDTH + 5f;

		public const float TIMELINE_SPAN = 10f;

		public const string ID_HEADER_SCORE = "S_";
		public const string ID_HEADER_TIMELINE = "TL_";
		public const string ID_HEADER_TACK = "TA_";

		public const int BEHAVE_FRAME_MOVE_RATIO = 4;

		// defaults
		public const string DEFAULT_SCORE_ID = "New Score";
		public const string DEFAULT_SCORE_INFO = "generated by TimeFlowShiki.";

		//public const string DEFAULT_TIMELINE_NAME = "New Timeline";

		//public const string DEFAULT_TACK_NAME = "New Tack";
		public const int DEFAULT_TACK_SPAN = 4;

		public const char UNDO_CACHED_TACK_SEPARATER = ':';

		public const string RESOURCE_BASEPATH = "Assets/TimeFlowShiki/Editor/GUI/Res/";

		public static List<Color> RESOURCE_COLORS_SOURCES = new List<Color>
			{
				new Color(0.000f, 0.318f, 0.604f, 1.000f),
				new Color(0.620f, 0.137f, 0.494f, 1.000f),
				new Color(0.929f, 0.043f, 0.443f, 1.000f),
				new Color(0.929f, 0.102f, 0.231f, 1.000f),
				new Color(0.953f, 0.424f, 0.133f, 1.000f),
				new Color(1.000f, 0.812f, 0.012f, 1.000f),
				new Color(0.843f, 0.867f, 0.137f, 1.000f),
				new Color(0.306f, 0.690f, 0.286f, 1.000f),
				new Color(0.051f, 0.592f, 0.286f, 1.000f),
				new Color(0.000f, 0.620f, 0.620f, 1.000f),
				new Color(0.004f, 0.631f, 0.773f, 1.000f),
				new Color(0.039f, 0.553f, 0.804f, 1.000f),
			};

		public const string RESOURCE_TICK = RESOURCE_BASEPATH + "tick.png";
		public const string RESOURCE_CONDITIONLINE_BG = RESOURCE_BASEPATH + "conditionLineBg.png";

		public const string RESOURCE_TRACK_HEADER_BG = RESOURCE_BASEPATH + "headerBg.png";
		public const string RESOURCE_TRACK_CONDITION_BG = RESOURCE_BASEPATH + "bg.png";
		public const string RESOURCE_TRACK_FRAME_BG = RESOURCE_BASEPATH + "5frame.png";

		public const string RESOURCE_TACK_WHITEPOINT = RESOURCE_BASEPATH + "whitePoint.png";
		public const string RESOURCE_TACK_GRAYPOINT = RESOURCE_BASEPATH + "grayPoint.png";
		public const string RESOURCE_TACK_WHITEPOINT_SINGLE = RESOURCE_BASEPATH + "whitePointSingle.png";
		public const string RESOURCE_TACK_GRAYPOINT_SINGLE = RESOURCE_BASEPATH + "grayPointSingle.png";

		public const string RESOURCE_TACK_ACTIVE_BASE = RESOURCE_BASEPATH + "activeTack.png";

		public const string RESOURCE_DUMMY_BG = RESOURCE_BASEPATH + "dummyBg.png";

		public static Texture2D tickTex;
		public static Texture2D conditionLineBgTex;
		public static Texture2D timelineHeaderTex;
		public static Texture2D frameTex;
		public static Texture2D whitePointTex;
		public static Texture2D grayPointTex;
		public static Texture2D whitePointSingleTex;
		public static Texture2D grayPointSingleTex;
		public static Texture2D activeTackBaseTex;

		public static Texture2D dummyTex;
	}
}