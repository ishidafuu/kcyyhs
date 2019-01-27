using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	//[SerializeField]
	public partial class ARIMotionSubWindow : EditorWindow
	{

		public class PartsObject
		{
			public enPartsType partsType;
			public Vector2Int pos;
			public PartsTransformState partsTransform;

			public PartsObject(enPartsType partsType)
			{
				this.partsType = partsType;
			}
		}

		public static Action<OnTrackEvent> ParentEmit;
		enum enFocusObject
		{
			focusTack,
			focusTimeline,
			focusScore,
		}

		const int GRIDSIZE = 16;
		const int GRIDSIZE_Z = 8;
		const int TIPSIZE = 24;
		const int MAXPOS = 32;
		const int MAXMAG = 16;
		const int MINMAG = 2;
		int mag_ = (MAXMAG / 2);
		[SerializeField]
		ARIMotionMainWindow parent_;
		//JMCharManager charManager_;

		Vector2 camPos_ = new Vector2(32, 64);
		Vector2 mouseStPos_;
		bool isRepaint_;

		Dictionary<enPartsType, PartsObject> partsObjects_ = new Dictionary<enPartsType, PartsObject>();
		MotionState sendMotion_; //シーンに移すデータ
		Vector2 tempMovePos_; //位置移動反映

		enFocusObject focusObject_;
		TimelineType timelineType_;
		Dictionary<enEditPartsType, bool> isMultiParts_ = new Dictionary<enEditPartsType, bool>();
		Dictionary<enEditPartsType, Vector2Int> multiOffset_ = new Dictionary<enEditPartsType, Vector2Int>();
		string lastTackId_;
		string lastParentTimelineId_;
		int selectedFrame_;
		string lastScoreId_;
		MotionPos clipPos_;
		bool isSabunPos_;

		public void OnEnable()
		{

			InitializeSubWindowView();
		}

		void InitializeSubWindowView()
		{
			this.titleContent = new GUIContent("TimeFlowSub");

			this.wantsMouseMove = true;

		}

		// サブウィンドウを開く
		public static ARIMotionSubWindow ShowEditor(ARIMotionMainWindow parent)
		{
			ARIMotionSubWindow window = EditorWindow.GetWindow<ARIMotionSubWindow>();
			window.Show();
			//window.minSize = new Vector2Int(WINDOW_W, WINDOW_H);
			window.SetParent(parent);
			window.init();
			return window;
		}

		void SetParent(ARIMotionMainWindow parent)
		{
			this.parent_ = parent;
		}

		// サブウィンドウの初期化
		public void init()
		{
			wantsMouseMove = true; // マウス情報を取得.
			isRepaint_ = true;
			foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
			{
				var enEditParts = PartsConverter.Convert(item);

				isMultiParts_[enEditParts] = false;
				multiOffset_[enEditParts] = Vector2Int.zero;
			}
		}

		void OnGUI()
		{
			//親閉じたら閉じる
			if (parent_ == null)
			{
				Close();
				return;
			}
			isRepaint_ = false;

			var viewWidth = this.position.width;
			var viewHeight = this.position.height;

			GUI.BeginGroup(new Rect(0, 0, viewWidth, viewHeight));
			{
				SwitchEditType();
				HandlingEvent();
				DrawAutoConponent();
			}
			GUI.EndGroup();

		}

		//現在メイン窓で選択中の項目
		void SwitchEditType()
		{
			var activeScore = parent_.GetActiveScore();
			if (activeScore.IsActiveTackPoint())
			{
				SelectedTack();
			}
			else if (activeScore.IsActiveTimeline())
			{
				SelectedTimeline();
			}
			else
			{
				SelectedScore();
			}
		}

		//スコア選択中
		void SelectedScore()
		{

			//違うタックが選ばれた
			if ((focusObject_ != enFocusObject.focusScore)
				|| ((selectedFrame_ != parent_.scoreWindow_.GetSelectedFrame()))
				|| (lastScoreId_ != parent_.GetActiveScore().id_))
			{
				//Undoクリア
				ARIMotionMainWindow.tackCmd_.Clear();
				//タイムラインに応じた位置
				focusObject_ = enFocusObject.focusScore;
				selectedFrame_ = parent_.scoreWindow_.GetSelectedFrame();
				lastScoreId_ = parent_.GetActiveScore().id_;
				ClearSelectedParts();

				SetupPartsData(false);

			}

			//SetupPartsData();//座標、トランスフォーム変化
		}
		//タイムライン選択中
		void SelectedTimeline()
		{

			//違うタックが選ばれた
			if (focusObject_ != enFocusObject.focusTimeline)
			{
				//Undoクリア
				ARIMotionMainWindow.tackCmd_.Clear();

				focusObject_ = enFocusObject.focusTimeline;
				selectedFrame_ = 0;
				ClearSelectedParts();

				//SetupPartsData();

			}
		}
		//タック選択中
		void SelectedTack()
		{
			//あとまわし
			//タックの種類によって持ってくるデータが違う

			var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
			//違うタックが選ばれた
			if ((focusObject_ != enFocusObject.focusTack)
				|| ((activeTack.tackId_ != lastTackId_)
					|| (activeTack.parentTimelineId_ != lastParentTimelineId_)))
			{
				//Undoクリア
				ARIMotionMainWindow.tackCmd_.Clear();

				timelineType_ = (TimelineType)activeTack.timelineType_;
				//Debug.Log(timelineType_.ToString());
				lastParentTimelineId_ = activeTack.parentTimelineId_;
				lastTackId_ = activeTack.tackId_;
				focusObject_ = enFocusObject.focusTack;
				selectedFrame_ = (activeTack.start_ + activeTack.span_ - 1); //タック末端

				SetupPartsData(false);

			}
		}

		//遅延命令
		public void Emit(OnTrackEvent onTrackEvent)
		{

			var type = onTrackEvent.eventType;
			// tack events.
			switch (type)
			{
				case OnTrackEvent.EventType.EVENT_PARTS_MOVED:
					{
						//Undo.RecordObject(this, "Parts Moved");
						//Debug.Log("Parts Moved");
						parent_.NeedSave();
						return;
					}
				case OnTrackEvent.EventType.EVENT_PARTS_COPY:
					{
						var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
						clipPos_ = activeTack.motionData_.mPos;

						return;
					}
				case OnTrackEvent.EventType.EVENT_PARTS_PASTE:
					{
						Undo.RecordObject(this, "Parts Paste");
						var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
						activeTack.motionData_.mPos = clipPos_;
						SetupPartsData(true);
						return;
					}
				default:
					{
						//親に投げる
						ParentEmit(onTrackEvent);
						break;
					}
			}
		}

		//現在の選択フレームの状態を各パーツに移す
		public void SetupPartsData(bool isChange)
		{
			//各種タイムライン
			SetupPartsPos();
			SetupPartsTransform();
			SetupPartsMove();
			//SetupPartsAtari();
			//SetupPartsThrow();
			SetupPartsColor();
			SetupPartsEffect();
			SetupPartsPassive();

			StateToPartsObject();
			RefreshMotionManager();

			isRepaint_ = true;

			if (isChange)Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_PARTS_MOVED, null));
		}

		void StateToPartsObject()
		{
			foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
			{
				GetPartsObject(item).pos = sendMotion_.stPos.GetPos(item) + BasePosition.GetPosEdit(item, false);
				GetPartsObject(item).partsTransform = sendMotion_.stTransform.GetTransform(item);
			}
		}

		////タックのデータをサブウインドウに移す
		// void SetHoldPartsData(enPartsType partsType, Vector2Int pos, PartsTransformState partsTransform, MotionHoldState motionHold)
		//{
		//	//この段階でエディタ用のパーツごとの位置補正がされるので、全体反転や全体回転もここで加味する
		//	//反転が入っている場合は左右があるパーツは、X位置を反転させ、逆側の基礎位置を使用する
		//	if (motionHold.mirror) pos.x = -pos.x;
		//	//回転が入っている場合は基礎位置設定後、原点パーツが原点になるように移動させ、原点を中心に回転し、移動させた分をそのまま戻す

		//	pos += BasePosition.GetPosEdit(partsType, motionHold.mirror);
		//	//HOLDは足下原点ではなく、ボディ原点にする
		//	pos -= BasePosition.GetPosEdit(enPartsType.Body, false);
		//	Vector2Int tempPos = pos;
		//	switch ((enPartsRotate)motionHold.rotate)
		//	{
		//		case enPartsRotate.Rotate0:
		//			break;
		//		case enPartsRotate.Rotate90:
		//			pos.x = -tempPos.y;
		//			pos.y = tempPos.x;
		//			break;
		//		case enPartsRotate.Rotate180:
		//			pos.x = -tempPos.x;
		//			pos.y = -tempPos.y;
		//			break;
		//		case enPartsRotate.Rotate270:
		//			pos.x = tempPos.y;
		//			pos.y = -tempPos.x;
		//			break;
		//	}
		//	//pos += BasePosition.GetPosEdit(enPartsType.Body, false);
		//	partsTransform.rotate = ((partsTransform.rotate + motionHold.rotate) % 360);

		//}

		void SetupPartsPos()
		{
			//Pos
			var selectedFrameAndPrevPos = parent_.GetActiveScore().GetSelectedFrameAndPrevPos(selectedFrame_);
			if (selectedFrameAndPrevPos.Count == 0)return;

			TackPoint prevTack = (selectedFrameAndPrevPos.Count == 2)
				? selectedFrameAndPrevPos[1]
				: new TackPoint(); //最初のタックは前がないのでニュートラル

			TackPoint selectedTack = selectedFrameAndPrevPos[0];

			//中間モーション生成
			var intermediate = MotionPos.MakeIntermediate2(
				prevTack.motionData_.mPos,
				selectedTack.motionData_.mPos,
				selectedTack.start_, selectedTack.span_, selectedFrame_);

			sendMotion_.stPos = intermediate;
		}
		void SetupPartsTransform()
		{
			//MotionTransform latestTransform = parent_.GetActiveScore().GetLatestTransform(selectedFrame_);
			//sendMotion_.stTransform.InportMotion(latestTransform, selectedFrame_);
		}

		//現在の選択フレームの位置移動状態算出
		void SetupPartsMove()
		{
			tempMovePos_ = Vector2Int.zero;

			if (focusObject_ != enFocusObject.focusScore)return;

			if (!parent_.scoreWindow_.isMovePos_)return;

			//Move
			var untilSelectedFrameMove = parent_.GetActiveScore().GetUntilSelectedFrameMove(selectedFrame_);
			if (untilSelectedFrameMove == null)return;
			if (untilSelectedFrameMove.Count == 0)return;

			Vector2 nowSpeed = Vector2.zero;

			for (int i = 0; i < selectedFrame_; i++)
			{
				var nowTack = untilSelectedFrameMove
					.Where(t => t.IsExistTack_)
					.Where(t => (t.start_ <= i))
					.Where(t => ((t.start_ + t.span_) > i))
					.FirstOrDefault();

				float ACCTIME = (1f / (float)ARIMotionMainWindow.FPS);

				if (nowTack != null)
				{
					//そのタックに入ったフレーム
					if (i == nowTack.start_)
					{
						//初速の書き換え
						if (!nowTack.motionData_.mMove.isKeepX)nowSpeed.x = nowTack.motionData_.mMove.delta.x;
						if (!nowTack.motionData_.mMove.isKeepY)nowSpeed.y = nowTack.motionData_.mMove.delta.y;
					}

					//タック内
					nowSpeed.x += (nowTack.motionData_.mMove.accel.x * ACCTIME);
					nowSpeed.y += (nowTack.motionData_.mMove.accel.y * ACCTIME);

					//ブレーキ
					if (nowTack.motionData_.mMove.decelMag > 0)
					{
						//正規化された現在の速度に摩擦倍率
						Vector3 revVelocity = nowSpeed.normalized * nowTack.motionData_.mMove.decelMag;

						//ブレーキ
						float newX = (Mathf.Abs(nowSpeed.x) > Mathf.Abs(revVelocity.x))
							? nowSpeed.x - revVelocity.x
							: 0.0f;

						float newY = (Mathf.Abs(nowSpeed.y) > Mathf.Abs(revVelocity.y))
							? nowSpeed.y - revVelocity.y
							: 0.0f;

						nowSpeed.x = newX;
						nowSpeed.y = newY;
					}

					////浮いてるときだけ重力
					//if ((tempMovePos_.y < 0) && !nowTack.motionData_.mMove.isZeroGrv) {
					//	nowSpeed.y += (MoveDefine.main.gravity_ * ACCTIME);
					//}
				}

				//位置変更
				tempMovePos_.x += nowSpeed.x;
				tempMovePos_.y -= nowSpeed.y;
				if (tempMovePos_.y > 0)tempMovePos_.y = 0; //地面

			}
		}

		////当たり判定
		//void SetupPartsAtari()
		//{
		//	var atariTack = parent_.GetActiveScore().GetSelectedFrame(selectedFrame_, TimelineType.TL_ATARI);
		//	if (atariTack == null)
		//	{
		//		sendMotion_.stAtari.isActive = false;
		//	}
		//	else
		//	{
		//		sendMotion_.stAtari.InportMotion(atariTack.motionData_.mAtari);
		//	}
		//}

		////投げ位置
		//void SetupPartsThrow()
		//{
		//	var throwTack = parent_.GetActiveScore().GetSelectedFrame(selectedFrame_, TimelineType.TL_THROW);
		//	if (throwTack == null)
		//	{
		//		sendMotion_.stThrow.isActive = false;
		//	}
		//	else
		//	{
		//		sendMotion_.stThrow.InportMotion(throwTack.motionData_.mThrow, true);
		//	}

		//}

		//アニメーション
		void SetupPartsColor()
		{
			var aniTack = parent_.GetActiveScore().GetSelectedFrame(selectedFrame_, TimelineType.TL_COLOR);
			if (aniTack == null)
			{
				sendMotion_.stColor.isActive = false;
			}
			else
			{
				int frame = (selectedFrame_ - aniTack.start_);
				sendMotion_.stColor.Inportmotion(aniTack.motionData_.mColor, frame);
			}
		}

		//エフェクト
		void SetupPartsEffect()
		{
			var effectTack = parent_.GetActiveScore().GetSelectedFrame(selectedFrame_, TimelineType.TL_EFFECT);
			if (effectTack == null)
			{
				sendMotion_.stEffect.isActive = false;
			}
			else
			{
				sendMotion_.stEffect.InportMotion(effectTack.motionData_.mEffect);
			}
		}

		//状態
		void SetupPartsPassive()
		{
			var passiveTack = parent_.GetActiveScore().GetSelectedFrame(selectedFrame_, TimelineType.TL_PASSIVE);
			if (passiveTack != null)
			{
				sendMotion_.stPassive.InportMotion(passiveTack.motionData_.mPassive);
			}
		}

		void ClearSelectedParts()
		{
			foreach (enPartsType item in Enum.GetValues(typeof(enPartsType)))
			{
				var enEditParts = PartsConverter.Convert(item);

				isMultiParts_[enEditParts] = false;
				multiOffset_[enEditParts] = Vector2Int.zero;
			}
		}

		bool IsSelectedParts()
		{
			return isMultiParts_.Where(m => m.Value).Any();
		}

		//サブウインドウの編集情報をタックに送る

		//ポジション少数丸め
		Vector2Int RoundPosVector(Vector2 pos)
		{
			return new Vector2Int((int)pos.x, (int)pos.y);
		}

		//サブウインドウのパーツオブジェクト取得
		PartsObject GetPartsObject(enPartsType partsType)
		{
			if (!partsObjects_.ContainsKey(partsType))
				partsObjects_[partsType] = new PartsObject(partsType);

			return partsObjects_[partsType];
		}

		//シーンにタック情報を反映
		void RefreshMotionManager()
		{
			try
			{
				//あとまわしとりあえずPOSのみ
				switch (focusObject_)
				{
					case enFocusObject.focusScore:
					case enFocusObject.focusTack:
						//parent_.charManager_.motionManager.SetMotionDataFromEditor(sendMotion_);
						break;
					case enFocusObject.focusTimeline:
						break;
				}
			}
			catch
			{

			}

		}

	}
}