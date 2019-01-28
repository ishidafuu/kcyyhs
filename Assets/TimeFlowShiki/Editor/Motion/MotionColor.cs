using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	[Serializable]
	public struct MotionColor
	{
		public int palette; //パレットアニメーション
		public int alphaAni; //透過アニメーション
		public int alphaVar; //透過引数
	}
}