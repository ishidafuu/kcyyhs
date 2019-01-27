using System;
using System.Collections.Generic;
using UnityEngine;
namespace NKKD.EDIT
{
	public static class BasePosition
	{
		public static readonly int BODY_Y = 12;
		public static readonly int HEAD_Y = 18;
		public static readonly int ANT_Y = 13;
		public static readonly int ARM_X = 0;
		public static readonly int ARM_Y = 12;
		public static readonly int HAND_X = 3;
		public static readonly int HAND_Y = 12;
		public static readonly int LEG_X = 0;
		public static readonly int LEG_Y = 7;
		public static readonly int FOOT_X = 2;
		public static readonly int FOOT_Y = 2;

		//デフォで45度斜めを向いてる
		public static readonly int CORE_ANGLE_MAG = 15;
		public static readonly int CORE_ANGLE = 45;

		public static readonly float CORE_BREAST_SIZE = 4f;
		public static readonly float CORE_WAIST_SIZE = 3f;
		public static readonly Vector2Int BODY_BASE = new Vector2Int(0, BODY_Y);
		public static readonly Vector2Int HEAD_BASE = new Vector2Int(0, HEAD_Y);
		public static readonly Vector2Int L_ARM_BASE = new Vector2Int(ARM_X, ARM_Y);
		public static readonly Vector2Int R_ARM_BASE = new Vector2Int(-ARM_X, ARM_Y);
		public static readonly Vector2Int L_HAND_BASE = new Vector2Int(HAND_X, HAND_Y);
		public static readonly Vector2Int R_HAND_BASE = new Vector2Int(-HAND_X, HAND_Y);
		public static readonly Vector2Int L_LEG_BASE = new Vector2Int(LEG_X, LEG_Y);
		public static readonly Vector2Int R_LEG_BASE = new Vector2Int(-LEG_X, LEG_Y);
		public static readonly Vector2Int L_FOOT_BASE = new Vector2Int(FOOT_X, FOOT_Y);
		public static readonly Vector2Int R_FOOT_BASE = new Vector2Int(-FOOT_X, FOOT_Y);
		public static readonly Vector2Int ANT_BASE = new Vector2Int(0, (BODY_Y + ANT_Y));

		public static readonly List<int> FRONT_DEPTH = new List<int>
			{
				(int)enPartsType.LeftArm,
				(int)enPartsType.LeftHand,
				(int)enPartsType.LeftLeg,
				(int)enPartsType.LeftFoot,
				(int)enPartsType.Body,
				(int)enPartsType.Head,
				(int)enPartsType.Ant,
				(int)enPartsType.RightLeg,
				(int)enPartsType.RightFoot,
				(int)enPartsType.RightArm,
				(int)enPartsType.RightHand,
			};
		public static readonly List<int> BACK_DEPTH = new List<int>
			{
				(int)enPartsType.LeftHand,
				(int)enPartsType.LeftArm,
				(int)enPartsType.LeftFoot,
				(int)enPartsType.LeftLeg,
				(int)enPartsType.Ant,
				(int)enPartsType.Head,
				(int)enPartsType.Body,
				(int)enPartsType.RightFoot,
				(int)enPartsType.RightLeg,
				(int)enPartsType.RightHand,
				(int)enPartsType.RightArm,
			};

		public static Vector2Int GetPosEdit(enPartsType partsType, bool isMirror)
		{
			Vector2Int res = Vector2Int.zero;
			switch (partsType)
			{
				case enPartsType.Body:
					res = BODY_BASE;
					break;
				case enPartsType.Head:
					res = HEAD_BASE;
					break;
				case enPartsType.Ant:
					res = ANT_BASE;
					break;
				case enPartsType.LeftArm:
					res = (isMirror) ? R_ARM_BASE : L_ARM_BASE;
					break;
				case enPartsType.RightArm:
					res = (isMirror) ? L_ARM_BASE : R_ARM_BASE;
					break;
				case enPartsType.LeftHand:
					res = (isMirror) ? R_HAND_BASE : L_HAND_BASE;
					break;
				case enPartsType.RightHand:
					res = (isMirror) ? L_HAND_BASE : R_HAND_BASE;
					break;
				case enPartsType.LeftLeg:
					res = (isMirror) ? R_LEG_BASE : L_LEG_BASE;
					break;
				case enPartsType.RightLeg:
					res = (isMirror) ? L_LEG_BASE : R_LEG_BASE;
					break;
				case enPartsType.LeftFoot:
					res = (isMirror) ? R_FOOT_BASE : L_FOOT_BASE;
					break;
				case enPartsType.RightFoot:
					res = (isMirror) ? L_FOOT_BASE : R_FOOT_BASE;
					break;
			}
			return res;
		}

		public static Vector2Int GetPos(enPartsType partsType)
		{
			Vector2Int res = Vector2Int.zero;
			switch (partsType)
			{
				case enPartsType.Body:
					res = BODY_BASE;
					break;
				case enPartsType.Head:
					res = HEAD_BASE;
					break;
				case enPartsType.LeftArm:
					res = L_ARM_BASE;
					break;
				case enPartsType.RightArm:
					res = R_ARM_BASE;
					break;
				case enPartsType.LeftHand:
					res = L_HAND_BASE;
					break;
				case enPartsType.RightHand:
					res = R_HAND_BASE;
					break;
				case enPartsType.LeftLeg:
					res = L_LEG_BASE;
					break;
				case enPartsType.RightLeg:
					res = R_LEG_BASE;
					break;
				case enPartsType.LeftFoot:
					res = L_FOOT_BASE;
					break;
				case enPartsType.RightFoot:
					res = R_FOOT_BASE;
					break;
				case enPartsType.Ant:
					res = ANT_BASE;
					break;
			}
			return res;
		}

		//public static float PartsAngleBaseZ(enPartsType partsType, enPartsAngle bodyAngle, bool isFlip) {
		//	float res = 0;
		//	float DEPTH = -0.001f;
		//	//描画順
		//	switch (bodyAngle) {
		//		case enPartsAngle.Look: res = LOOKDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Back: res = BACKDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Side: res = SIDEDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Front: res = FRONTDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//		case enPartsAngle.Rear: res = REARDEPTH.IndexOf((int)partsType) * DEPTH; break;
		//	}

		//	//全体が反転してるときはスプライトボードが裏返っているので奥行きを反転
		//	if (isFlip) res = -res;

		//	return res;
		//}

		//ボディ向きから割り出した描画プライオリティリスト
		public static List<enPartsType> GenGetZSortList(bool isLeft, bool isBack)
		{
			List<enPartsType> res = new List<enPartsType>();
			if (isBack)
			{
				foreach (var item in BACK_DEPTH)
				{
					enPartsType pt = (enPartsType)item;
					if (isLeft)
					{
						if (pt == enPartsType.LeftArm)
							pt = enPartsType.RightArm;
						else if (pt == enPartsType.RightArm)
							pt = enPartsType.LeftArm;
						else if (pt == enPartsType.LeftHand)
							pt = enPartsType.RightHand;
						else if (pt == enPartsType.RightHand)
							pt = enPartsType.LeftHand;
						else if (pt == enPartsType.LeftLeg)
							pt = enPartsType.RightLeg;
						else if (pt == enPartsType.RightLeg)
							pt = enPartsType.LeftLeg;
						else if (pt == enPartsType.LeftFoot)
							pt = enPartsType.RightFoot;
						else if (pt == enPartsType.RightFoot)
							pt = enPartsType.LeftFoot;
					}
					res.Add(pt);
				}
			}
			else
			{
				foreach (var item in FRONT_DEPTH)
				{
					enPartsType pt = (enPartsType)item;
					if (isLeft)
					{
						if (pt == enPartsType.LeftArm)
							pt = enPartsType.RightArm;
						else if (pt == enPartsType.RightArm)
							pt = enPartsType.LeftArm;
						else if (pt == enPartsType.LeftHand)
							pt = enPartsType.RightHand;
						else if (pt == enPartsType.RightHand)
							pt = enPartsType.LeftHand;
						else if (pt == enPartsType.LeftLeg)
							pt = enPartsType.RightLeg;
						else if (pt == enPartsType.RightLeg)
							pt = enPartsType.LeftLeg;
						else if (pt == enPartsType.LeftFoot)
							pt = enPartsType.RightFoot;
						else if (pt == enPartsType.RightFoot)
							pt = enPartsType.LeftFoot;
					}
					res.Add(pt);
				}
			}
			////描画順
			//switch (bodyAngle) {
			//	case enPartsAngle.Look: foreach (var item in LOOKDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Back: foreach (var item in BACKDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Front: foreach (var item in FRONTDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Side: foreach (var item in SIDEDEPTH) res.Add((enPartsType)item); break;
			//	case enPartsAngle.Rear: foreach (var item in REARDEPTH) res.Add((enPartsType)item); break;
			//}
			return res;
		}

		////スクリプタブルオブジェクト用
		//public static float[] OutputDepth(enPartsAngle angle) {
		//	List<float> res = new List<float>();
		//	//描画順
		//	float DEPTH = -0.001f;
		//	switch (angle) {
		//		case enPartsAngle.Look: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(LOOKDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Back: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(BACKDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Front: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(FRONTDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Side: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(SIDEDEPTH.IndexOf(i) * DEPTH); break;
		//		case enPartsAngle.Rear: for (int i = 0; i < (int)enPartsType._END; i++) res.Add(REARDEPTH.IndexOf(i) * DEPTH); break;
		//	}
		//	return res.ToArray();
		//}

		//スクリプタブルオブジェクト用
		public static AniDepth OutputDepth(bool isBack)
		{
			List<float> res = new List<float>();
			//描画順
			float DEPTH = -0.0001f; //やっぱマイナスが上
			if (isBack)
			{
				for (int i = 0; i < (int)enPartsType._END; i++)res.Add(BACK_DEPTH.IndexOf(i) * DEPTH);
			}
			else
			{
				for (int i = 0; i < (int)enPartsType._END; i++)res.Add(FRONT_DEPTH.IndexOf(i) * DEPTH);
			}
			AniDepth res2 = new AniDepth();
			res2.SetData(res.ToArray());
			return res2;
		}

	}
}