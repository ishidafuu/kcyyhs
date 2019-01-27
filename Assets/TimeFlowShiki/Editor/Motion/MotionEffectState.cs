using UnityEngine;
using System;
using System.Collections;

namespace NKKD.EDIT
{
	//タイムライン：エフェクト
	[Serializable]
	public struct MotionEffectState
	{
		public bool isActive;
		public MotionEffect data;
		public void InportMotion(MotionEffect motionEffect)
		{
			isActive = true;
			data = motionEffect;
		}
	}
}
