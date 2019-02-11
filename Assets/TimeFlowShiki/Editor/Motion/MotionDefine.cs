using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{

	//モーション
	public enum enMotionType
	{
		Idle,
		Walk,
		Run,
		Step,
		BStep,
		Slip,
		Jump,
		Fall,
		Land,
		Damage,
		Fly,
		Down,
		Dead,
		Arm0,
		Arm1,
		Arm2,
		Leg0,
		Leg1,
		Leg2,
		Opt0,
		Action,
	}
	// 	public enum enPartsTypeBase
	// 	{
	// 	Ant,
	// 	Head,
	// 	Body,
	// 	Arm,
	// 	Hand,
	// 	Leg,
	// 	Foot,
	// 	_END,
	// }

	//パーツ編集種類（Arm,LegはCoreの値で算出する）
	public enum enEditPartsType
	{
		Head,
		Body,
		LeftHand,
		RightHand,
		LeftFoot,
		RightFoot,
		Arm,
		Leg,
		_END,
	}

	//実際の描画パーツ種類位置
	public enum enPartsType
	{
		Ant = 0,
		Head,
		Body,
		LeftArm,
		RightArm,
		LeftHand,
		RightHand,
		LeftLeg,
		RightLeg,
		LeftFoot,
		RightFoot,
		_END,
	}

	//各種タイムライン
	public enum TimelineType : int
	{
		TL_POS,
		TL_TRANSFORM,
		TL_MOVE,
		TL_ATARI,
		TL_HOLD,
		TL_THROW,
		TL_COLOR,
		TL_EFFECT,
		TL_PASSIVE,
	}

	public enum enPartsRotate
	{
		Rotate0 = 0,
		Rotate90 = 90,
		Rotate180 = 180,
		Rotate270 = 270,
	}

	//当たりエフェクト
	public enum enAtariEffect
	{
		Normal,
	}

	//パーティクルエフェクト
	public enum enParticleEffect
	{
		None,
	}

	//特殊エフェクト
	public enum enSpecialEffect
	{
		None,
	}

	//位置移動カーブ
	public enum enCurve
	{
		Normal,
		SinCurve,
		CosCurve,
		Sin180Curve,
		Exp2Curve,
		Log2Curve,
		Exp3Curve,
		Log3Curve,
		Exp4Curve,
		Log4Curve,
		Exp6Curve,
		Log6Curve,
		Exp8Curve,
		Log8Curve,
	}

	//パレットアニメーション
	public enum enPaletteAni
	{
		Def, //現状維持
		Mono,
		Mono2,
		Cyan,
		Cyan2,
		Blue,
		Blue2,
		Magenta,
		Magenta2,
		Red,
		Red2,
		Orange,
		Orange2,
		Yellow,
		Yellow2,
		Lime,
		Lime2,
		Green,
		Green2,
		Emerald,
		Emerald2,
		Water,
	}

	//トランスフォームアニメーション
	public enum enTransformAni
	{
		None,
		AngleLLo,
		AngleLMid,
		AngleLHi,
		AngleRLo,
		AngleRMid,
		AngleRHi,
		RotateLLo,
		RotateLMid,
		RotateLHi,
		RotateRLo,
		RotateRMid,
		RotateRHi,
	}

	//透過アニメーション
	public enum enAlphaAni
	{
		Continue, //現状維持
		Stop, //停止
		Alpha, //透過度
		Brink, //点滅
		FadeIn, //フェードイン
		FadeOut, //フェードアウト
	}

	//分岐条件
	public enum enSwitchCondition
	{
		None,
		Hit, //攻撃ヒット
		Damage, //ダメージ
		Land, //着地
		Input, //追加入力
		Finish, //アニメーション終了時
	}

	public struct MotionState
	{
		//各種タイムライン
		public MotionPosState stPos;
		public MotionTransformState stTransform;
		public MotionMoveState stMove;
		public MotionColorState stColor;
		public MotionEffectState stEffect;
		public MotionPassiveState stPassive;

		public void Reset(bool isPosReset)
		{
			if (isPosReset)stPos = new MotionPosState();
			stTransform = new MotionTransformState();
			stMove = new MotionMoveState();
			stColor = new MotionColorState();
			stEffect = new MotionEffectState();
			stPassive = new MotionPassiveState();
		}
	}

	[SerializeField]
	public struct MotionData
	{
		//各種タイムライン
		[SerializeField]
		public MotionPos mPos;
		[SerializeField]
		public MotionTransform mTransform;
		[SerializeField]
		public MotionMove mMove;
		[SerializeField]
		public MotionColor mColor;
		[SerializeField]
		public MotionEffect mEffect;
		[SerializeField]
		public MotionPassive mPassive;
	}

	//パレットアニメーション
	public static class PaletteAnimation
	{
		public static int GetPaletteNo(enPaletteAni paletteAni, int frame)
		{
			int res = 0;
			if (paletteAni != enPaletteAni.Def)
			{
				int[] PALNO = { 0, 1, 2, 1 };

				int itv = (8 * FPSManager.Instance.motionFps_) / 60;
				int no = PALNO[(frame / itv) % PALNO.Length];
				res = ((((int)paletteAni - 1) * 3) + no) + 1;
			}

			return res;
		}
	}

	public static class PartsConverter
	{
		public static enEditPartsType Convert(enPartsType partsType)
		{
			enEditPartsType res = enEditPartsType._END;

			switch (partsType)
			{
				case enPartsType.Body:
					res = enEditPartsType.Body;
					break;
				case enPartsType.Head:
					res = enEditPartsType.Head;
					break;
				case enPartsType.LeftHand:
					res = enEditPartsType.LeftHand;
					break;
				case enPartsType.RightHand:
					res = enEditPartsType.RightHand;
					break;
				case enPartsType.LeftFoot:
					res = enEditPartsType.LeftFoot;
					break;
				case enPartsType.RightFoot:
					res = enEditPartsType.RightFoot;
					break;
				case enPartsType.LeftArm:
				case enPartsType.RightArm:
					res = enEditPartsType.Arm;
					break;
				case enPartsType.LeftLeg:
				case enPartsType.RightLeg:
					res = enEditPartsType.Leg;
					break;
			}

			return res;
		}
	}
}