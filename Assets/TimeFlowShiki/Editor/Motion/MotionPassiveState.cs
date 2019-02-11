using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
namespace NKKD.EDIT {
	public struct MotionPassiveState {
		public bool isLeft;
		public bool isBack;
		public int faceNo;
		//public bool isNoLand;//着地無視
		//public bool isLoop;//ループ
		//public bool isMuteki;//無敵状態
		//public bool isCancel;//キャンセル可能状態
		//public Dictionary<enSwitchCondition, string> switchCondition;

		public void InportMotion(MotionPassive motionPassive) {
			isLeft = motionPassive.isLeft;
			isBack = motionPassive.isBack;
			faceNo = motionPassive.faceNo;
			//isNoLand = motionPassive.isNoLand;
			//isLoop = motionPassive.isLoop;
			//isMuteki = motionPassive.isMuteki;
			//isCancel = motionPassive.isCancel;

			//if (switchCondition == null) switchCondition = new Dictionary<enSwitchCondition, string>();
			//switchCondition[(enSwitchCondition)motionPassive.switchCondition] = motionPassive.switchId;
		}
		//public string GetSwitchId(enSwitchCondition condition)
		//{
		//	string res = "Idle";//とりあえず
		//	if ((switchCondition != null) && (switchCondition.ContainsKey(condition)))
		//	{
		//		res = switchCondition[condition];
		//	}

		//	return res;
		//}
	}
}
