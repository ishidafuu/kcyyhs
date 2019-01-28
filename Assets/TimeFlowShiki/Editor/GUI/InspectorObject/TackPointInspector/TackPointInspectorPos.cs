using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class TackPointInspectorPos
	{
		public void Draw(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var motionData = tackPoint.GetMotionData();
			var posX = EditorGUILayout.IntField("pos.x", (int)motionData.mPos.pos.x);
			var posY = EditorGUILayout.IntField("pos.y", (int)motionData.mPos.pos.y);

			if (EditorGUI.EndChangeCheck())
			{
				var lastData = motionData.mPos;
				Action action = () =>
				{
					motionData.mPos.pos.x = posX;
					motionData.mPos.pos.y = posY;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { motionData.mPos = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}
	}
}