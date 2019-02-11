using UnityEngine;
using System;
using System.Collections;

namespace NKKD.EDIT
{
	//タイムライン：技属性

	[Serializable]
	public struct MotionPassive
	{
		public bool isLeft;
		public bool isBack;
		public int faceNo;
		//public bool isNoLand;//着地無視
		//public bool isLoop;//ループ
		//public bool isMuteki;//無敵状態
		//public bool isCancel;//キャンセル可能状態
		//public int switchCondition;//enSwitchCondition分岐条件
		//public string switchId;//分岐先
	}
}
