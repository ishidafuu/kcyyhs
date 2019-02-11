using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{

	//タイムライン：パーツアニメーション
	[Serializable]
	public struct MotionColor
	{
		public int palette; //パレットアニメーション
		public int alphaAni; //透過アニメーション
		public int alphaVar; //透過引数

		public bool isAnt;
		public bool isHead;
		public bool isBody;
		public bool isLeftArm;
		public bool isRightArm;

		public bool isLeftHand;
		public bool isRightHand;
		public bool isLeftLeg;
		public bool isRightLeg;
		public bool isLeftFoot;
		public bool isRightFoot;

		public bool IsActive(enPartsType partsType)
		{
			bool res = false;
			switch (partsType)
			{
				case enPartsType.Ant:
					res = isAnt;
					break;
				case enPartsType.Head:
					res = isHead;
					break;
				case enPartsType.Body:
					res = isBody;
					break;
				case enPartsType.LeftArm:
					res = isLeftArm;
					break;
				case enPartsType.RightArm:
					res = isRightArm;
					break;
				case enPartsType.LeftHand:
					res = isLeftHand;
					break;
				case enPartsType.RightHand:
					res = isRightHand;
					break;
				case enPartsType.LeftLeg:
					res = isLeftLeg;
					break;
				case enPartsType.RightLeg:
					res = isRightLeg;
					break;
				case enPartsType.LeftFoot:
					res = isLeftFoot;
					break;
				case enPartsType.RightFoot:
					res = isRightFoot;
					break;
			}
			return res;
		}
	}
}