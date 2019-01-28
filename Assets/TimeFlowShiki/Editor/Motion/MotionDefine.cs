using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{

	// 背景（１）
	// 背景エフェクト（１）
	// キャラ（１）
	// キャラパーツ（複）
	// キャラエフェクト（１）
	// カメラフィルター（１）
	// メッセージ（１）

	//各種タイムライン
	public enum TimelineType : int
	{
		//座標移動
		TL_POS,
		//形状
		TL_TRANSFORM,
		//色
		TL_COLOR,
		//効果
		TL_EFFECT,
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

	public struct MotionState
	{
		//各種タイムライン
		public MotionPosState stPos;
		public MotionTransformState stTransform;
		public MotionColorState stColor;
		public MotionEffectState stEffect;

		public void Reset(bool isPosReset)
		{
			if (isPosReset)
				stPos = new MotionPosState();
			stTransform = new MotionTransformState();
			stColor = new MotionColorState();
			stEffect = new MotionEffectState();
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
		public MotionColor mColor;
		[SerializeField]
		public MotionEffect mEffect;
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
}