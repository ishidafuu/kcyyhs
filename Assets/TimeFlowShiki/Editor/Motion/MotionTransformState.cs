using System;
using System.Collections;
using UnityEngine;

namespace NKKD.EDIT
{
	public struct PartsTransformState
	{
		public int rotate;

		public void SetFromPartsTransform(PartsTransform partsTransform)
		{
			rotate = partsTransform.rotate;
			//isLeft = partsTransform.isLeft;
			//isBack = partsTransform.isBack;
		}

		public void Reset()
		{
			this = new PartsTransformState();
		}
	}

	public struct MotionTransformState
	{
		public PartsTransformState head;
		public PartsTransformState body;
		public PartsTransformState leftArm;
		public PartsTransformState rightArm;
		public PartsTransformState leftHand;
		public PartsTransformState rightHand;
		public PartsTransformState leftLeg;
		public PartsTransformState rightLeg;
		public PartsTransformState leftFoot;
		public PartsTransformState rightFoot;
		public PartsTransformState ant;

		public PartsTransformState GetTransform(enPartsType partsType)
		{
			PartsTransformState res = new PartsTransformState();
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

		public void SetFromPartsTransform(enPartsType partsType, PartsTransform partsTransform)
		{
			switch (partsType)
			{
				case enPartsType.Body:
					body.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.Head:
					head.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.LeftArm:
					leftArm.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.RightArm:
					rightArm.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.LeftHand:
					leftHand.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.RightHand:
					rightHand.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.LeftLeg:
					leftLeg.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.RightLeg:
					rightLeg.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.LeftFoot:
					leftFoot.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.RightFoot:
					rightFoot.SetFromPartsTransform(partsTransform);
					break;
				case enPartsType.Ant:
					ant.SetFromPartsTransform(partsTransform);
					break;
			}
		}
	}
}