using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	//タイムライン：向きなど
	[Serializable]
	public struct MotionTransform
	{
		public int rotate;

		public void Reset()
		{
			rotate = (int)enPartsRotate.Rotate0;
		}
	}
}