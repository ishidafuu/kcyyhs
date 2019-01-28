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
	public class TackPointInspectorAni
	{
		public void Draw(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var motionData = tackPoint.GetMotionData();
			var paletteAni = (enPaletteAni)EditorGUILayout.EnumPopup("paletteAni", (enPaletteAni)motionData.mColor.palette);
			var alphaAni = (enAlphaAni)EditorGUILayout.EnumPopup("alphaAni", (enAlphaAni)motionData.mColor.alphaAni);
			var alphaVar = EditorGUILayout.IntField("alphaVar", (int)motionData.mColor.alphaVar);

			if (EditorGUI.EndChangeCheck())
			{
				var lastData = motionData.mColor;
				Action action = () =>
				{
					motionData.mColor.palette = (int)paletteAni;
					motionData.mColor.alphaAni = (int)alphaAni;
					motionData.mColor.alphaVar = alphaVar;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { motionData.mColor = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}
	}
}