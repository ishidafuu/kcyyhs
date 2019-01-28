using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	[Serializable] //シリアライズするため必要
	public struct MotionPos
	{
		public Vector2Int pos;
		public int curveX; //0等速1sin2cos
		public int curveY; //0等速1sin2cos

		//カーブの計算
		public static float IntermediateCurve(float frame, int curve, float stPos, float edPos)
		{
			float edPer = 0;
			//速度のカーブなので位置自体はサインコサインが逆になる
			switch ((enCurve)curve)
			{
				case enCurve.Normal:
					edPer = frame;
					break;
				case enCurve.SinCurve:
					edPer = 1 - Mathf.Cos(frame * Mathf.PI / 2);
					break;
				case enCurve.CosCurve:
					edPer = Mathf.Sin(frame * Mathf.PI / 2);
					break;
				case enCurve.Sin180Curve:
					edPer = (1 - Mathf.Cos(frame * Mathf.PI)) / 2;
					break;
				case enCurve.Exp2Curve:
					edPer = Mathf.Pow(frame, 2);
					break;
				case enCurve.Log2Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 2);
					break;
				case enCurve.Exp3Curve:
					edPer = Mathf.Pow(frame, 3);
					break;
				case enCurve.Log3Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 3);
					break;
				case enCurve.Exp4Curve:
					edPer = Mathf.Pow(frame, 4);
					break;
				case enCurve.Log4Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 4);
					break;
				case enCurve.Exp6Curve:
					edPer = Mathf.Pow(frame, 6);
					break;
				case enCurve.Log6Curve:
					edPer = 1 - Mathf.Pow(1 - frame, 6);
					break;

			}
			float stPer = (1 - edPer);
			return (stPos * stPer) + (edPos * edPer);
		}

		//中割り
		public static MotionPosState MakeIntermediate2(MotionPos stPos, MotionPos edPos,
			float start, float span, float pos)
		{
			MotionPosState res = new MotionPosState();
			//0~1
			float frame = (((pos + 1) - start) / span);
			float posx = IntermediateCurve(frame, edPos.curveX, stPos.pos.x, edPos.pos.x);
			float posy = IntermediateCurve(frame, edPos.curveY, stPos.pos.y, edPos.pos.y);
			res.pos = new Vector2Int((int)Math.Round(posx), (int)Math.Round(posy));
			return res;

		}
	}
}