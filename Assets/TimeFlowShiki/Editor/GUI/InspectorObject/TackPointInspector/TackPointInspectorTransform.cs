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
	public class TackPointInspectorTransform
	{
		public void Draw(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var motionData = tackPoint.GetMotionData();
			var rotate = (enPartsRotate)EditorGUILayout.EnumPopup("Ant.rotate", (enPartsRotate)motionData.mTransform.rotate);
			if (EditorGUI.EndChangeCheck())
			{

				var lastData = motionData.mTransform;
				Action action = () =>
				{
					motionData.mTransform.rotate = (int)rotate;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { motionData.mTransform = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}
	}
}