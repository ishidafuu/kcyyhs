using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class TackPointInspector : ScriptableObject
	{
		public TackPoint tackPoint;

		public void UpdateTackPoint(TackPoint tackPoint)
		{
			this.tackPoint = tackPoint;

		}
	}

	[CustomEditor(typeof(TackPointInspector))]
	public class TackPointInspectorGUI : Editor
	{
		public override void OnInspectorGUI()
		{
			var insp = (TackPointInspector)target;

			var tackPoint = insp.tackPoint;
			//UpdateTackTitle(tackPoint);

			DrawTackSpan(tackPoint);

			switch ((TimelineType)tackPoint.timelineType_)
			{
				case TimelineType.TL_POS:
					DrawTackPos(tackPoint);
					break;
				case TimelineType.TL_TRANSFORM:
					DrawTackTransform(tackPoint);
					break;
				case TimelineType.TL_MOVE:
					DrawTackMove(tackPoint);
					break;
					//case TimelineType.TL_ATARI: DrawTackAtari(tackPoint); break;
					//case TimelineType.TL_HOLD: DrawTackHold(tackPoint); break;
					//case TimelineType.TL_THROW: DrawTackThrow(tackPoint); break;
				case TimelineType.TL_COLOR:
					DrawTackAni(tackPoint);
					break;
				case TimelineType.TL_EFFECT:
					DrawTackEffect(tackPoint);
					break;
				case TimelineType.TL_PASSIVE:
					DrawTackPassive(tackPoint);
					break;
				default:
					Debug.LogError(tackPoint.timelineType_.ToString());
					break;
			}

			UndoKey();

		}

		private void UndoKey()
		{
			if (Event.current.type != EventType.KeyDown)return;

			if (Event.current.keyCode == KeyCode.Z)
			{
				ARIMotionMainWindow.tackCmd_.Undo();
				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
			else if (Event.current.keyCode == KeyCode.Y)
			{
				ARIMotionMainWindow.tackCmd_.Redo();
				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}

		}

		private Vector2 RoundPosVector(Vector2 pos)
		{
			return new Vector2((int)pos.x, (int)pos.y);
		}

		private void DrawTackSpan(TackPoint tackPoint)
		{
			var start = tackPoint.start_;
			GUILayout.Label("start : " + start);
			var span = tackPoint.span_;
			var end = start + span - 1;
			GUILayout.Label("end : " + end);
			GUILayout.Label("span : " + span);
		}

		private List<string> GetAllMotionList()
		{
			//�x�[�X���[�V�����ȊO�̎���Ή�����
			//�R�R�ō쐬�����X�V���ɏ��������̕���������
			return ARIMotionMainWindow.fileList_;
		}

		private int GetMotionIndex(string motionId)
		{
			int res = 0;
			var item = GetAllMotionList()
				.Select((x, i) => new { x, i })
				.Where(xi => xi.x == motionId)
				.FirstOrDefault();

			if (item != null)res = item.i;

			return res;
			//var motionId = EditorGUILayout.Popup("motionId", selectedIndex, JMMotionMainWindow.fileList_.ToArray());
		}

		private void DrawTackPos(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var bodyX = EditorGUILayout.IntField("body.pos.x", (int)tackPoint.motionData_.mPos.body.pos.x);
			var bodyY = EditorGUILayout.IntField("body.pos.y", (int)tackPoint.motionData_.mPos.body.pos.y);
			var headX = EditorGUILayout.IntField("head.pos.x", (int)tackPoint.motionData_.mPos.head.pos.x);
			var headY = EditorGUILayout.IntField("head.pos.y", (int)tackPoint.motionData_.mPos.head.pos.y);
			// var leftArmX = EditorGUILayout.IntField("leftArm.pos.x", (int)tackPoint.motionData_.mPos.leftArm.pos.x);
			// var leftArmY = EditorGUILayout.IntField("leftArm.pos.y", (int)tackPoint.motionData_.mPos.leftArm.pos.y);
			// var rightArmX = EditorGUILayout.IntField("rightArm.pos.x", (int)tackPoint.motionData_.mPos.rightArm.pos.x);
			// var rightArmY = EditorGUILayout.IntField("rightArm.pos.y", (int)tackPoint.motionData_.mPos.rightArm.pos.y);

			var leftHandX = EditorGUILayout.IntField("leftHand.pos.x", (int)tackPoint.motionData_.mPos.leftHand.pos.x);
			var leftHandY = EditorGUILayout.IntField("leftHand.pos.y", (int)tackPoint.motionData_.mPos.leftHand.pos.y);
			var rightHandX = EditorGUILayout.IntField("rightHand.pos.x", (int)tackPoint.motionData_.mPos.rightHand.pos.x);
			var rightHandY = EditorGUILayout.IntField("rightHand.pos.y", (int)tackPoint.motionData_.mPos.rightHand.pos.y);

			// var leftLegX = EditorGUILayout.IntField("leftLeg.pos.x", (int)tackPoint.motionData_.mPos.leftLeg.pos.x);
			// var leftLegY = EditorGUILayout.IntField("leftLeg.pos.y", (int)tackPoint.motionData_.mPos.leftLeg.pos.y);
			// var rightLegX = EditorGUILayout.IntField("rightLeg.pos.x", (int)tackPoint.motionData_.mPos.rightLeg.pos.x);
			// var rightLegY = EditorGUILayout.IntField("rightLeg.pos.y", (int)tackPoint.motionData_.mPos.rightLeg.pos.y);

			var leftFootX = EditorGUILayout.IntField("leftFoot.pos.x", (int)tackPoint.motionData_.mPos.leftFoot.pos.x);
			var leftFootY = EditorGUILayout.IntField("leftFoot.pos.y", (int)tackPoint.motionData_.mPos.leftFoot.pos.y);
			var rightFootX = EditorGUILayout.IntField("rightFoot.pos.x", (int)tackPoint.motionData_.mPos.rightFoot.pos.x);
			var rightFootY = EditorGUILayout.IntField("rightFoot.pos.y", (int)tackPoint.motionData_.mPos.rightFoot.pos.y);

			// var antX = EditorGUILayout.IntField("ant.pos.x", (int)tackPoint.motionData_.mPos.ant.pos.x);
			// var antY = EditorGUILayout.IntField("ant.pos.y", (int)tackPoint.motionData_.mPos.ant.pos.y);

			var armX = EditorGUILayout.IntField("arm.pos.x", (int)tackPoint.motionData_.mPos.arm.pos.x);
			var legY = EditorGUILayout.IntField("leg.pos.x", (int)tackPoint.motionData_.mPos.leg.pos.x);

			var bodyCurveX = (enCurve)EditorGUILayout.EnumPopup("body.curveX", (enCurve)tackPoint.motionData_.mPos.body.curveX);
			var bodyCurveY = (enCurve)EditorGUILayout.EnumPopup("body.curveY", (enCurve)tackPoint.motionData_.mPos.body.curveY);
			var headCurveX = (enCurve)EditorGUILayout.EnumPopup("head.curveX", (enCurve)tackPoint.motionData_.mPos.head.curveX);
			var headCurveY = (enCurve)EditorGUILayout.EnumPopup("head.curveY", (enCurve)tackPoint.motionData_.mPos.head.curveY);

			// var leftArmCurveX = (enCurve)EditorGUILayout.EnumPopup("leftArm.curveX", (enCurve)tackPoint.motionData_.mPos.leftArm.curveX);
			// var leftArmCurveY = (enCurve)EditorGUILayout.EnumPopup("leftArm.curveY", (enCurve)tackPoint.motionData_.mPos.leftArm.curveY);
			// var rightArmCurveX = (enCurve)EditorGUILayout.EnumPopup("rightArm.curveX", (enCurve)tackPoint.motionData_.mPos.rightArm.curveX);
			// var rightArmCurveY = (enCurve)EditorGUILayout.EnumPopup("rightArm.curveY", (enCurve)tackPoint.motionData_.mPos.rightArm.curveY);

			var leftHandCurveX = (enCurve)EditorGUILayout.EnumPopup("leftHand.curveX", (enCurve)tackPoint.motionData_.mPos.leftHand.curveX);
			var leftHandCurveY = (enCurve)EditorGUILayout.EnumPopup("leftHand.curveY", (enCurve)tackPoint.motionData_.mPos.leftHand.curveY);
			var rightHandCurveX = (enCurve)EditorGUILayout.EnumPopup("rightHand.curveX", (enCurve)tackPoint.motionData_.mPos.rightHand.curveX);
			var rightHandCurveY = (enCurve)EditorGUILayout.EnumPopup("rightHand.curveY", (enCurve)tackPoint.motionData_.mPos.rightHand.curveY);

			// var leftLegCurveX = (enCurve)EditorGUILayout.EnumPopup("leftLeg.curveX", (enCurve)tackPoint.motionData_.mPos.leftLeg.curveX);
			// var leftLegCurveY = (enCurve)EditorGUILayout.EnumPopup("leftLeg.curveY", (enCurve)tackPoint.motionData_.mPos.leftLeg.curveY);
			// var rightLegCurveX = (enCurve)EditorGUILayout.EnumPopup("rightLeg.curveX", (enCurve)tackPoint.motionData_.mPos.rightLeg.curveX);
			// var rightLegCurveY = (enCurve)EditorGUILayout.EnumPopup("rightLeg.curveY", (enCurve)tackPoint.motionData_.mPos.rightLeg.curveY);

			var leftFootCurveX = (enCurve)EditorGUILayout.EnumPopup("leftFoot.curveX", (enCurve)tackPoint.motionData_.mPos.leftFoot.curveX);
			var leftFootCurveY = (enCurve)EditorGUILayout.EnumPopup("leftFoot.curveY", (enCurve)tackPoint.motionData_.mPos.leftFoot.curveY);
			var rightFootCurveX = (enCurve)EditorGUILayout.EnumPopup("rightFoot.curveX", (enCurve)tackPoint.motionData_.mPos.rightFoot.curveX);
			var rightFootCurveY = (enCurve)EditorGUILayout.EnumPopup("rightFoot.curveY", (enCurve)tackPoint.motionData_.mPos.rightFoot.curveY);

			// var AntCurveX = (enCurve)EditorGUILayout.EnumPopup("Ant.curveX", (enCurve)tackPoint.motionData_.mPos.ant.curveX);
			// var AntCurveY = (enCurve)EditorGUILayout.EnumPopup("Ant.curveY", (enCurve)tackPoint.motionData_.mPos.ant.curveY);

			var ArmCurveX = (enCurve)EditorGUILayout.EnumPopup("arm.curveX", (enCurve)tackPoint.motionData_.mPos.arm.curveX);
			var LegCurveX = (enCurve)EditorGUILayout.EnumPopup("leg.curveX", (enCurve)tackPoint.motionData_.mPos.leg.curveX);

			if (EditorGUI.EndChangeCheck())
			{
				var lastData = tackPoint.motionData_.mPos;
				Action action = () =>
				{
					tackPoint.motionData_.mPos.body.pos.x = bodyX;
					tackPoint.motionData_.mPos.body.pos.y = bodyY;
					tackPoint.motionData_.mPos.head.pos.x = headX;
					tackPoint.motionData_.mPos.head.pos.y = headY;
					// tackPoint.motionData_.mPos.leftArm.pos.x = leftArmX;
					// tackPoint.motionData_.mPos.leftArm.pos.y = leftArmY;
					// tackPoint.motionData_.mPos.rightArm.pos.x = rightArmX;
					// tackPoint.motionData_.mPos.rightArm.pos.y = rightArmY;
					tackPoint.motionData_.mPos.leftHand.pos.x = leftHandX;
					tackPoint.motionData_.mPos.leftHand.pos.y = leftHandY;
					tackPoint.motionData_.mPos.rightHand.pos.x = rightHandX;
					tackPoint.motionData_.mPos.rightHand.pos.y = rightHandY;
					// tackPoint.motionData_.mPos.leftLeg.pos.x = leftLegX;
					// tackPoint.motionData_.mPos.leftLeg.pos.y = leftLegY;
					// tackPoint.motionData_.mPos.rightLeg.pos.x = rightLegX;
					// tackPoint.motionData_.mPos.rightLeg.pos.y = rightLegY;
					tackPoint.motionData_.mPos.leftFoot.pos.x = leftFootX;
					tackPoint.motionData_.mPos.leftFoot.pos.y = leftFootY;
					tackPoint.motionData_.mPos.rightFoot.pos.x = rightFootX;
					tackPoint.motionData_.mPos.rightFoot.pos.y = rightFootY;
					// tackPoint.motionData_.mPos.ant.pos.x = antX;
					// tackPoint.motionData_.mPos.ant.pos.y = antY;
					tackPoint.motionData_.mPos.arm.pos.x = armX;
					tackPoint.motionData_.mPos.leg.pos.x = legY;

					tackPoint.motionData_.mPos.body.curveX = (int)bodyCurveX;
					tackPoint.motionData_.mPos.body.curveY = (int)bodyCurveY;
					tackPoint.motionData_.mPos.head.curveX = (int)headCurveX;
					tackPoint.motionData_.mPos.head.curveY = (int)headCurveY;
					// tackPoint.motionData_.mPos.leftArm.curveX = (int)leftArmCurveX;
					// tackPoint.motionData_.mPos.leftArm.curveY = (int)leftArmCurveY;
					// tackPoint.motionData_.mPos.rightArm.curveX = (int)rightArmCurveX;
					// tackPoint.motionData_.mPos.rightArm.curveY = (int)rightArmCurveY;
					tackPoint.motionData_.mPos.leftHand.curveX = (int)leftHandCurveX;
					tackPoint.motionData_.mPos.leftHand.curveY = (int)leftHandCurveY;
					tackPoint.motionData_.mPos.rightHand.curveX = (int)rightHandCurveX;
					tackPoint.motionData_.mPos.rightHand.curveY = (int)rightHandCurveY;
					// tackPoint.motionData_.mPos.leftLeg.curveX = (int)leftLegCurveX;
					// tackPoint.motionData_.mPos.leftLeg.curveY = (int)leftLegCurveY;
					// tackPoint.motionData_.mPos.rightLeg.curveX = (int)rightLegCurveX;
					// tackPoint.motionData_.mPos.rightLeg.curveY = (int)rightLegCurveY;
					tackPoint.motionData_.mPos.leftFoot.curveX = (int)leftFootCurveX;
					tackPoint.motionData_.mPos.leftFoot.curveY = (int)leftFootCurveY;
					tackPoint.motionData_.mPos.rightFoot.curveX = (int)rightFootCurveX;
					tackPoint.motionData_.mPos.rightFoot.curveY = (int)rightFootCurveY;
					// tackPoint.motionData_.mPos.ant.curveX = (int)AntCurveX;
					// tackPoint.motionData_.mPos.ant.curveY = (int)AntCurveY;

					tackPoint.motionData_.mPos.arm.curveX = (int)ArmCurveX;
					tackPoint.motionData_.mPos.leg.curveX = (int)LegCurveX;

				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { tackPoint.motionData_.mPos = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}

		private void DrawTackMove(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var deltaX = EditorGUILayout.IntField("delta.x", (int)tackPoint.motionData_.mMove.delta.x);
			var deltaY = EditorGUILayout.IntField("delta.y", (int)tackPoint.motionData_.mMove.delta.y);
			var accelX = EditorGUILayout.FloatField("accel.x", tackPoint.motionData_.mMove.accel.x);
			var accelY = EditorGUILayout.FloatField("accel.y", tackPoint.motionData_.mMove.accel.y);
			var decelMag = EditorGUILayout.FloatField("decelMag", tackPoint.motionData_.mMove.decelMag);
			var isZeroGrv = EditorGUILayout.Toggle("isZeroGrv", tackPoint.motionData_.mMove.isZeroGrv);
			var isZeroFric = EditorGUILayout.Toggle("isZeroFric", tackPoint.motionData_.mMove.isZeroFric);
			var isKeepX = EditorGUILayout.Toggle("isKeepX", tackPoint.motionData_.mMove.isKeepX);
			var isKeepY = EditorGUILayout.Toggle("isKeepY", tackPoint.motionData_.mMove.isKeepY);
			//var isNoLand = EditorGUILayout.Toggle("isNoLand", tackPoint.motionData_.mMove.isNoLand);
			if (EditorGUI.EndChangeCheck())
			{

				var lastData = tackPoint.motionData_.mMove;
				Action action = () =>
				{
					tackPoint.motionData_.mMove.delta.x = deltaX;
					tackPoint.motionData_.mMove.delta.y = deltaY;
					tackPoint.motionData_.mMove.accel.x = accelX;
					tackPoint.motionData_.mMove.accel.y = accelY;
					tackPoint.motionData_.mMove.decelMag = decelMag;
					//tackPoint.motionData_.mMove.decel.x = decelX;
					//tackPoint.motionData_.mMove.decel.y = decelY;
					tackPoint.motionData_.mMove.isZeroGrv = isZeroGrv;
					tackPoint.motionData_.mMove.isZeroFric = isZeroFric;
					tackPoint.motionData_.mMove.isKeepX = isKeepX;
					tackPoint.motionData_.mMove.isKeepY = isKeepY;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { tackPoint.motionData_.mMove = lastData; }));

				//tackPoint.motionData_.mMove.isNoLand = isNoLand;
				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}

		private void DrawTackTransform(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var AntRotate = (enPartsRotate)EditorGUILayout.EnumPopup("Ant.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.ant.rotate);
			var headRotate = (enPartsRotate)EditorGUILayout.EnumPopup("head.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.head.rotate);
			var bodyRotate = (enPartsRotate)EditorGUILayout.EnumPopup("body.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.body.rotate);
			var leftArmRotate = (enPartsRotate)EditorGUILayout.EnumPopup("leftArm.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.leftArm.rotate);
			var rightArmRotate = (enPartsRotate)EditorGUILayout.EnumPopup("rightArm.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.rightArm.rotate);
			var leftHandRotate = (enPartsRotate)EditorGUILayout.EnumPopup("leftHand.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.leftHand.rotate);
			var rightHandRotate = (enPartsRotate)EditorGUILayout.EnumPopup("rightHand.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.rightHand.rotate);
			var leftLegRotate = (enPartsRotate)EditorGUILayout.EnumPopup("leftLeg.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.leftLeg.rotate);
			var rightLegRotate = (enPartsRotate)EditorGUILayout.EnumPopup("rightLeg.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.rightLeg.rotate);
			var leftFootRotate = (enPartsRotate)EditorGUILayout.EnumPopup("leftFoot.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.leftFoot.rotate);
			var rightFootRotate = (enPartsRotate)EditorGUILayout.EnumPopup("rightFoot.rotate", (enPartsRotate)tackPoint.motionData_.mTransform.rightFoot.rotate);

			if (EditorGUI.EndChangeCheck())
			{

				var lastData = tackPoint.motionData_.mTransform;
				Action action = () =>
				{
					tackPoint.motionData_.mTransform.ant.rotate = (int)AntRotate;
					tackPoint.motionData_.mTransform.head.rotate = (int)headRotate;
					tackPoint.motionData_.mTransform.body.rotate = (int)bodyRotate;

					tackPoint.motionData_.mTransform.leftArm.rotate = (int)leftArmRotate;
					tackPoint.motionData_.mTransform.rightArm.rotate = (int)rightArmRotate;
					tackPoint.motionData_.mTransform.leftHand.rotate = (int)leftHandRotate;
					tackPoint.motionData_.mTransform.rightHand.rotate = (int)rightHandRotate;
					tackPoint.motionData_.mTransform.leftLeg.rotate = (int)leftLegRotate;
					tackPoint.motionData_.mTransform.rightLeg.rotate = (int)rightLegRotate;
					tackPoint.motionData_.mTransform.leftFoot.rotate = (int)leftFootRotate;
					tackPoint.motionData_.mTransform.rightFoot.rotate = (int)rightFootRotate;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { tackPoint.motionData_.mTransform = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}

		//private void DrawTackAtari(TackPoint tackPoint)
		//{
		//	EditorGUI.BeginChangeCheck();
		//	var power = EditorGUILayout.IntField("power", tackPoint.motionData_.mAtari.power);
		//	var isBomb = EditorGUILayout.Toggle("isBomb", tackPoint.motionData_.mAtari.isBomb);
		//	var isHitReset = EditorGUILayout.Toggle("isHitReset", tackPoint.motionData_.mAtari.isHitReset);
		//	var effect = (enAtariEffect)EditorGUILayout.EnumPopup("effect", (enAtariEffect)tackPoint.motionData_.mAtari.effect);

		//	var isHead = EditorGUILayout.Toggle("isHead", tackPoint.motionData_.mAtari.isHead);
		//	var isBody = EditorGUILayout.Toggle("isBody", tackPoint.motionData_.mAtari.isBody);
		//	var isLeftArm = EditorGUILayout.Toggle("isLeftArm", tackPoint.motionData_.mAtari.isLeftArm);
		//	var isRightArm = EditorGUILayout.Toggle("isRightArm", tackPoint.motionData_.mAtari.isRightArm);
		//	var isLeftLeg = EditorGUILayout.Toggle("isLeftLeg", tackPoint.motionData_.mAtari.isLeftLeg);
		//	var isRightLeg = EditorGUILayout.Toggle("isRightLeg", tackPoint.motionData_.mAtari.isRightLeg);
		//	var isAnt = EditorGUILayout.Toggle("isAnt", tackPoint.motionData_.mAtari.isAnt);
		//	var isRightAnt = EditorGUILayout.Toggle("isRightAnt", tackPoint.motionData_.mAtari.isRightAnt);

		//	if (EditorGUI.EndChangeCheck())
		//	{
		//		var lastData = tackPoint.motionData_.mAtari;
		//		Action action = () =>
		//		{
		//			tackPoint.motionData_.mAtari.power = power;
		//			tackPoint.motionData_.mAtari.isHitReset = isHitReset;
		//			tackPoint.motionData_.mAtari.isBomb = isBomb;
		//			tackPoint.motionData_.mAtari.effect = (int)effect;
		//			tackPoint.motionData_.mAtari.isHead = isHead;
		//			tackPoint.motionData_.mAtari.isBody = isBody;
		//			tackPoint.motionData_.mAtari.isLeftArm = isLeftArm;
		//			tackPoint.motionData_.mAtari.isRightArm = isRightArm;
		//			tackPoint.motionData_.mAtari.isLeftLeg = isLeftLeg;
		//			tackPoint.motionData_.mAtari.isRightLeg = isRightLeg;
		//			tackPoint.motionData_.mAtari.isAnt = isAnt;
		//			tackPoint.motionData_.mAtari.isRightAnt = isRightAnt;
		//		};

		//		JMMotionMainWindow.tackCmd_.Do(
		//			new MotionCommand(MethodBase.GetCurrentMethod().Name,
		//			() => { action(); },
		//			() => { tackPoint.motionData_.mAtari = lastData; }));

		//		TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
		//	}
		//}
		//private void DrawTackHold(TackPoint tackPoint)
		//{
		//	EditorGUI.BeginChangeCheck();
		//	int selectedIndex = GetMotionIndex(tackPoint.motionData_.mHold.motionId);
		//	var motionId = EditorGUILayout.Popup("motionId", selectedIndex, GetAllMotionList().ToArray());
		//	var posX = EditorGUILayout.IntField("posX", (int)tackPoint.motionData_.mHold.pos.x);
		//	var posY = EditorGUILayout.IntField("posY", (int)tackPoint.motionData_.mHold.pos.y);
		//	var rotate = (enPartsRotate)EditorGUILayout.EnumPopup("rotate", (enPartsRotate)tackPoint.motionData_.mHold.rotate);
		//	var mirror = EditorGUILayout.Toggle("mirror", tackPoint.motionData_.mHold.mirror);
		//	var isFront = EditorGUILayout.Toggle("isFront", tackPoint.motionData_.mHold.isFront);
		//	var curveX = (enCurve)EditorGUILayout.EnumPopup("curveX", (enCurve)tackPoint.motionData_.mHold.curveX);
		//	var curveY = (enCurve)EditorGUILayout.EnumPopup("curveY", (enCurve)tackPoint.motionData_.mHold.curveY);

		//	if (EditorGUI.EndChangeCheck())
		//	{
		//		var lastData = tackPoint.motionData_.mHold;
		//		Action action = () =>
		//		{
		//			tackPoint.motionData_.mHold.motionId = JMMotionMainWindow.fileList_[motionId];
		//			tackPoint.motionData_.mHold.pos.x = posX;
		//			tackPoint.motionData_.mHold.pos.y = posY;
		//			tackPoint.motionData_.mHold.rotate = (int)rotate;
		//			tackPoint.motionData_.mHold.mirror = mirror;
		//			tackPoint.motionData_.mHold.isFront = isFront;
		//			tackPoint.motionData_.mHold.curveX = (int)curveX;
		//			tackPoint.motionData_.mHold.curveY = (int)curveY;
		//		};

		//		JMMotionMainWindow.tackCmd_.Do(
		//			new MotionCommand(MethodBase.GetCurrentMethod().Name,
		//			() => { action(); },
		//			() => { tackPoint.motionData_.mHold = lastData; }));

		//		TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
		//	}
		//}
		//private void DrawTackThrow(TackPoint tackPoint)
		//{
		//	EditorGUI.BeginChangeCheck();

		//	var posX = EditorGUILayout.IntField("posX", (int)tackPoint.motionData_.mThrow.pos.x);
		//	var posY = EditorGUILayout.IntField("posY", (int)tackPoint.motionData_.mThrow.pos.y);
		//	var deltaX = EditorGUILayout.IntField("deltaX", (int)tackPoint.motionData_.mThrow.delta.x);
		//	var deltaY = EditorGUILayout.IntField("deltaY", (int)tackPoint.motionData_.mThrow.delta.y);
		//	var isLiner = EditorGUILayout.Toggle("mirror", tackPoint.motionData_.mThrow.isLiner);

		//	if (EditorGUI.EndChangeCheck())
		//	{

		//		var lastData = tackPoint.motionData_.mThrow;
		//		Action action = () =>
		//		{
		//			tackPoint.motionData_.mThrow.pos.x = posX;
		//			tackPoint.motionData_.mThrow.pos.y = posY;
		//			tackPoint.motionData_.mThrow.delta.x = deltaX;
		//			tackPoint.motionData_.mThrow.delta.y = deltaY;
		//			tackPoint.motionData_.mThrow.isLiner = isLiner;
		//		};

		//		JMMotionMainWindow.tackCmd_.Do(
		//			new MotionCommand(MethodBase.GetCurrentMethod().Name,
		//			() => { action(); },
		//			() => { tackPoint.motionData_.mThrow = lastData; }));

		//		TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
		//	}
		//}
		private void DrawTackAni(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var paletteAni = (enPaletteAni)EditorGUILayout.EnumPopup("paletteAni", (enPaletteAni)tackPoint.motionData_.mColor.palette);
			//var transformAni = (enTransformAni)EditorGUILayout.EnumPopup("transformAni", (enTransformAni)tackPoint.motionData_.mAni.transformAni);
			var alphaAni = (enAlphaAni)EditorGUILayout.EnumPopup("alphaAni", (enAlphaAni)tackPoint.motionData_.mColor.alphaAni);
			var alphaVar = EditorGUILayout.IntField("alphaVar", (int)tackPoint.motionData_.mColor.alphaVar);

			var isThorax = EditorGUILayout.Toggle("isThorax", tackPoint.motionData_.mColor.isBody);
			var isHead = EditorGUILayout.Toggle("isHead", tackPoint.motionData_.mColor.isHead);
			var isLeftArm = EditorGUILayout.Toggle("isLeftArm", tackPoint.motionData_.mColor.isLeftArm);
			var isRightArm = EditorGUILayout.Toggle("isRightArm", tackPoint.motionData_.mColor.isRightArm);
			var isLeftHand = EditorGUILayout.Toggle("isLeftHand", tackPoint.motionData_.mColor.isLeftHand);
			var isRightHand = EditorGUILayout.Toggle("isRightHand", tackPoint.motionData_.mColor.isRightHand);
			var isLeftLeg = EditorGUILayout.Toggle("isLeftLeg", tackPoint.motionData_.mColor.isLeftLeg);
			var isRightLeg = EditorGUILayout.Toggle("isRightLeg", tackPoint.motionData_.mColor.isRightLeg);
			var isLeftFoot = EditorGUILayout.Toggle("isLeftFoot", tackPoint.motionData_.mColor.isLeftFoot);
			var isRightFoot = EditorGUILayout.Toggle("isRightFoot", tackPoint.motionData_.mColor.isRightFoot);
			var isAnt = EditorGUILayout.Toggle("isAnt", tackPoint.motionData_.mColor.isAnt);

			if (EditorGUI.EndChangeCheck())
			{

				var lastData = tackPoint.motionData_.mColor;
				Action action = () =>
				{
					tackPoint.motionData_.mColor.palette = (int)paletteAni;
					//tackPoint.motionData_.mAni.transformAni = (int)transformAni;
					tackPoint.motionData_.mColor.alphaAni = (int)alphaAni;
					tackPoint.motionData_.mColor.alphaVar = alphaVar;
					tackPoint.motionData_.mColor.isBody = isThorax;
					tackPoint.motionData_.mColor.isHead = isHead;
					tackPoint.motionData_.mColor.isLeftArm = isLeftArm;
					tackPoint.motionData_.mColor.isRightArm = isRightArm;
					tackPoint.motionData_.mColor.isLeftHand = isLeftHand;
					tackPoint.motionData_.mColor.isRightHand = isRightHand;
					tackPoint.motionData_.mColor.isLeftLeg = isLeftLeg;
					tackPoint.motionData_.mColor.isRightLeg = isRightLeg;
					tackPoint.motionData_.mColor.isLeftFoot = isLeftFoot;
					tackPoint.motionData_.mColor.isRightFoot = isRightFoot;
					tackPoint.motionData_.mColor.isAnt = isAnt;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { tackPoint.motionData_.mColor = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}
		private void DrawTackEffect(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var se = EditorGUILayout.IntField("se", tackPoint.motionData_.mEffect.se);
			var particle = (enParticleEffect)EditorGUILayout.EnumPopup("particle", (enParticleEffect)tackPoint.motionData_.mEffect.particle);
			var special = (enSpecialEffect)EditorGUILayout.EnumPopup("special", (enSpecialEffect)tackPoint.motionData_.mEffect.special);

			if (EditorGUI.EndChangeCheck())
			{
				var lastData = tackPoint.motionData_.mEffect;
				Action action = () =>
				{
					tackPoint.motionData_.mEffect.se = se;
					tackPoint.motionData_.mEffect.particle = (int)particle;
					tackPoint.motionData_.mEffect.special = (int)special;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { tackPoint.motionData_.mEffect = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}
		private void DrawTackPassive(TackPoint tackPoint)
		{
			EditorGUI.BeginChangeCheck();
			var isLeft = EditorGUILayout.Toggle("isLeft", tackPoint.motionData_.mPassive.isLeft);
			var isBack = EditorGUILayout.Toggle("isBack", tackPoint.motionData_.mPassive.isBack);
			var faceNo = EditorGUILayout.IntField("faceNo", tackPoint.motionData_.mPassive.faceNo);
			if (EditorGUI.EndChangeCheck())
			{
				var lastData = tackPoint.motionData_.mPassive;
				Action action = () =>
				{
					tackPoint.motionData_.mPassive.isLeft = isLeft;
					tackPoint.motionData_.mPassive.isBack = isBack;
					tackPoint.motionData_.mPassive.faceNo = faceNo;
				};

				ARIMotionMainWindow.tackCmd_.Do(
					new MotionCommand(MethodBase.GetCurrentMethod().Name,
						() => { action(); },
						() => { tackPoint.motionData_.mPassive = lastData; }));

				TackPoint.Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_CHANGE, null));
			}
		}
	}
}