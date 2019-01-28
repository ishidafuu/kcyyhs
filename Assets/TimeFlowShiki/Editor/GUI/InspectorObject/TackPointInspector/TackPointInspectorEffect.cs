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
	public class TackPointInspectorEffect
	{
		public void Draw(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();

			var motionData = tackPoint.GetMotionData();
			var se = EditorGUILayout.IntField("se", motionData.mEffect.se);
			var particle = (enParticleEffect)EditorGUILayout.EnumPopup("particle", (enParticleEffect)motionData.mEffect.particle);
			var special = (enSpecialEffect)EditorGUILayout.EnumPopup("special", (enSpecialEffect)motionData.mEffect.special);

			if (EditorGUI.EndChangeCheck())
			{
				var lastData = tackPoint.GetMotionData().mEffect;
				Action action = () =>
				{
					motionData.mEffect.se = se;
					motionData.mEffect.particle = (int)particle;
					motionData.mEffect.special = (int)special;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { motionData.mEffect = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}
	}
}