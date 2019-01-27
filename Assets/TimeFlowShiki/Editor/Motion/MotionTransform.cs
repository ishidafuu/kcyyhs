using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	[Serializable] //シリアライズするため必要
	public struct PartsTransform
	{
		//public int ani;
		public int rotate;
		//public int angle;
		//public bool isLeft;
		//public bool isBack;

		public void Reset()
		{
			//ani = (int)enTransformAni.None;
			rotate = (int)enPartsRotate.Rotate0;
			//angle = (int)enPartsAngle.Side;
			//isLeft = false;
			//isBack = false;
		}
	}

	//タイムライン：向きなど
	[Serializable]
	public struct MotionTransform
	{

		public PartsTransform head;
		public PartsTransform body;
		public PartsTransform gaster;
		public PartsTransform leftArm;
		public PartsTransform rightArm;
		public PartsTransform leftHand;
		public PartsTransform rightHand;
		public PartsTransform leftLeg;
		public PartsTransform rightLeg;
		public PartsTransform leftFoot;
		public PartsTransform rightFoot;
		public PartsTransform ant;

		public PartsTransform GetTransform(enPartsType partsType)
		{
			PartsTransform res = new PartsTransform();
			switch (partsType)
			{
				case enPartsType.Body:
					res = body;
					break;
				case enPartsType.Head:
					res = head;
					break;
				case enPartsType.LeftArm:
					res = leftArm;
					break;
				case enPartsType.RightArm:
					res = rightArm;
					break;
				case enPartsType.LeftHand:
					res = leftHand;
					break;
				case enPartsType.RightHand:
					res = rightHand;
					break;
				case enPartsType.LeftLeg:
					res = leftLeg;
					break;
				case enPartsType.RightLeg:
					res = rightLeg;
					break;
				case enPartsType.LeftFoot:
					res = leftFoot;
					break;
				case enPartsType.RightFoot:
					res = rightFoot;
					break;
				case enPartsType.Ant:
					res = ant;
					break;
			}
			return res;
		}

		public void SetTransform(enPartsType partsType, PartsTransform value)
		{
			switch (partsType)
			{
				case enPartsType.Body:
					body = value;
					break;
				case enPartsType.Head:
					head = value;
					break;
				case enPartsType.LeftArm:
					leftArm = value;
					break;
				case enPartsType.RightArm:
					rightArm = value;
					break;
				case enPartsType.LeftHand:
					leftHand = value;
					break;
				case enPartsType.RightHand:
					rightHand = value;
					break;
				case enPartsType.LeftLeg:
					leftLeg = value;
					break;
				case enPartsType.RightFoot:
					rightFoot = value;
					break;
				case enPartsType.LeftFoot:
					leftFoot = value;
					break;
				case enPartsType.RightLeg:
					rightLeg = value;
					break;
				case enPartsType.Ant:
					ant = value;
					break;
			}

		}

		public void SetRotate(enPartsType partsType, enPartsRotate rotate)
		{
			int intRotate = (int)rotate;
			switch (partsType)
			{
				case enPartsType.Body:
					body.rotate = intRotate;
					break;
				case enPartsType.Head:
					head.rotate = intRotate;
					break;
				case enPartsType.LeftArm:
					leftArm.rotate = intRotate;
					break;
				case enPartsType.LeftLeg:
					leftLeg.rotate = intRotate;
					break;
				case enPartsType.RightArm:
					rightArm.rotate = intRotate;
					break;
				case enPartsType.RightLeg:
					rightLeg.rotate = intRotate;
					break;
				case enPartsType.Ant:
					ant.rotate = intRotate;
					break;
			}
		}
		public enPartsRotate GetRotate(enPartsType partsType)
		{
			int res = 0;
			switch (partsType)
			{
				case enPartsType.Body:
					res = body.rotate;
					break;
				case enPartsType.Head:
					res = head.rotate;
					break;
				case enPartsType.LeftArm:
					res = leftArm.rotate;
					break;
				case enPartsType.RightArm:
					res = rightArm.rotate;
					break;
				case enPartsType.LeftHand:
					res = leftHand.rotate;
					break;
				case enPartsType.RightHand:
					res = rightHand.rotate;
					break;
				case enPartsType.LeftLeg:
					res = leftLeg.rotate;
					break;
				case enPartsType.RightLeg:
					res = rightLeg.rotate;
					break;
				case enPartsType.LeftFoot:
					res = leftLeg.rotate;
					break;
				case enPartsType.RightFoot:
					res = rightLeg.rotate;
					break;
				case enPartsType.Ant:
					res = ant.rotate;
					break;
			}
			return (enPartsRotate)res;
		}

		//public void SetAngle(enPartsType partsType, enPartsAngle angle)
		//{
		//	int intAngle = (int)angle;
		//	switch (partsType)
		//	{
		//		case enPartsType.Thorax: thorax.angle = intAngle; break;
		//		case enPartsType.Gaster: gaster.angle = intAngle; break;
		//		case enPartsType.Head: head.angle = intAngle; break;
		//		case enPartsType.LeftArm: leftArm.angle = intAngle; break;
		//		case enPartsType.LeftLeg: leftLeg.angle = intAngle; break;
		//		case enPartsType.RightArm: rightArm.angle = intAngle; break;
		//		case enPartsType.RightLeg: rightLeg.angle = intAngle; break;
		//		case enPartsType.Ant: ant.angle = intAngle; break;
		//	}
		//}
		//public enPartsAngle GetAngle(enPartsType partsType)
		//{
		//	int res = 0;
		//	switch (partsType)
		//	{
		//		case enPartsType.Thorax: res = thorax.angle; break;
		//		case enPartsType.Gaster: res = gaster.angle; break;
		//		case enPartsType.Head: res = head.angle; break;
		//		case enPartsType.LeftArm: res = leftArm.angle; break;
		//		case enPartsType.LeftLeg: res = leftLeg.angle; break;
		//		case enPartsType.RightArm: res = rightArm.angle; break;
		//		case enPartsType.RightLeg: res = rightLeg.angle; break;
		//		case enPartsType.Ant: res = ant.angle; break;
		//	}
		//	return (enPartsAngle)res;
		//}
		//public void SetMirror(enPartsType partsType, bool mirror)
		//{
		//	switch (partsType)
		//	{
		//		case enPartsType.Thorax: thorax.mirror = mirror; break;
		//		case enPartsType.Gaster: gaster.mirror = mirror; break;
		//		case enPartsType.Head: head.mirror = mirror; break;
		//		case enPartsType.LeftArm: leftArm.mirror = mirror; break;
		//		case enPartsType.LeftLeg: leftLeg.mirror = mirror; break;
		//		case enPartsType.RightArm: rightArm.mirror = mirror; break;
		//		case enPartsType.RightLeg: rightLeg.mirror = mirror; break;
		//		case enPartsType.Ant: ant.mirror = mirror; break;
		//	}
		//}
		//public bool GetMirror(enPartsType partsType)
		//{
		//	bool res = false;
		//	switch (partsType)
		//	{
		//		case enPartsType.Thorax: res = thorax.mirror; break;
		//		case enPartsType.Gaster: res = gaster.mirror; break;
		//		case enPartsType.Head: res = head.mirror; break;
		//		case enPartsType.LeftArm: res = leftArm.mirror; break;
		//		case enPartsType.LeftLeg: res = leftLeg.mirror; break;
		//		case enPartsType.RightArm: res = rightArm.mirror; break;
		//		case enPartsType.RightLeg: res = rightLeg.mirror; break;
		//		case enPartsType.Ant: res = ant.mirror; break;
		//	}
		//	return res;
		//}
		//public void SetAni(enPartsType partsType, enTransformAni ani)
		//{
		//	int intAni = (int)ani;
		//	switch (partsType)
		//	{
		//		case enPartsType.Thorax: thorax.ani = intAni; break;
		//		case enPartsType.Gaster: gaster.ani = intAni; break;
		//		case enPartsType.Head: head.ani = intAni; break;
		//		case enPartsType.LeftArm: leftArm.ani = intAni; break;
		//		case enPartsType.LeftLeg: leftLeg.ani = intAni; break;
		//		case enPartsType.RightArm: rightArm.ani = intAni; break;
		//		case enPartsType.RightLeg: rightLeg.ani = intAni; break;
		//		case enPartsType.Ant: ant.ani = intAni; break;
		//	}
		//}
		//public enTransformAni GetAni(enPartsType partsType)
		//{
		//	int res = 0;
		//	switch (partsType)
		//	{
		//		case enPartsType.Thorax: res = thorax.ani; break;
		//		case enPartsType.Gaster: res = gaster.ani; break;
		//		case enPartsType.Head: res = head.ani; break;
		//		case enPartsType.LeftArm: res = leftArm.ani; break;
		//		case enPartsType.LeftLeg: res = leftLeg.ani; break;
		//		case enPartsType.RightArm: res = rightArm.ani; break;
		//		case enPartsType.RightLeg: res = rightLeg.ani; break;
		//		case enPartsType.Ant: res = ant.ani; break;
		//	}
		//	return (enTransformAni)res;
		//}

		public void Reset(enPartsType partsType)
		{
			switch (partsType)
			{
				case enPartsType.Body:
					body.Reset();
					break;
				case enPartsType.Head:
					head.Reset();
					break;
				case enPartsType.LeftArm:
					leftArm.Reset();
					break;
				case enPartsType.RightArm:
					rightArm.Reset();
					break;
				case enPartsType.LeftHand:
					leftHand.Reset();
					break;
				case enPartsType.RightHand:
					rightHand.Reset();
					break;
				case enPartsType.LeftLeg:
					leftLeg.Reset();
					break;
				case enPartsType.RightLeg:
					rightLeg.Reset();
					break;
				case enPartsType.LeftFoot:
					leftFoot.Reset();
					break;
				case enPartsType.RightFoot:
					rightFoot.Reset();
					break;
				case enPartsType.Ant:
					ant.Reset();
					break;
			}
		}
	}

}