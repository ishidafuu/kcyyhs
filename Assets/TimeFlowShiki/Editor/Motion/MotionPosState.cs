using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{

	///<summary>実際に出力される座標データ</summary>
	public struct MotionPosState
	{
		public Vector2Int pos;

		public AniFrame OutputFrame()
		{
			AniFrame res = new AniFrame();
			res.pos = pos;
			return res;
		}
	}
}