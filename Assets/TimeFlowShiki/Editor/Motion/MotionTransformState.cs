using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	public struct MotionTransformState
	{
		public int rotate;

		public void Reset()
		{
			this = new MotionTransformState();
		}
	}
}