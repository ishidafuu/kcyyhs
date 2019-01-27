using UnityEngine;
using System;
using System.Collections;

namespace NKKD.EDIT
{
	//タイムライン：位置移動
	[Serializable]
	public struct MotionMoveState
	{
		public bool isActive;
		public MotionMove data;
		public bool isStart;
		public void InportMotion(MotionMove motionMove, bool isStart)
		{
			isActive = true;
			data = motionMove;
			this.isStart = isStart;
		}
	}
}
