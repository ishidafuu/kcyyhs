using UnityEngine;
using System;
using System.Collections;

namespace NKKD.EDIT
{
	//タイムライン：位置移動
	[Serializable]
	public struct MotionMove
	{
		public Vector2 delta;//初速
		public Vector2 accel;//加速
		public float decelMag;//減速（摩擦と同じ原理）
		public bool isZeroGrv;//無重力
		public bool isZeroFric;//摩擦なし
		public bool isKeepX;//X速度保持
		public bool isKeepY;//Y速度保持
	}
}
