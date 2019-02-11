using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public partial class ARIMotionSubWindow
	{
		const float GIZMO_SIZE = 1f;

		///<summary>描画</summary>
		void DrawAutoConponent()
		{
			DrawLabel();

			DrawChar();
			if (isRepaint_)Repaint();
		}

		///<summary>キャラ描画</summary>
		void DrawChar()
		{
			DrawPartsAll();
			//DrawPartsAll(false);
			// DrawPartsLabel();

			DrawGridLine3();
			if (focusObject_ == enFocusObject.focusTack)
			{
				DrawGridLine3Sub();
				DrawGridLine3Cross(tempMovePos_, new Color(1f, 0.5f, 1f, 0.8f));
			}
		}

		///<summary>ラベル描画</summary>
		void DrawLabel()
		{

			if (focusObject_ == enFocusObject.focusTack)
			{
				EditorGUILayout.LabelField(timelineType_.ToString());
				EditorGUILayout.LabelField("S：保存");
				EditorGUILayout.LabelField("L：読み込み");
				EditorGUILayout.LabelField("Z：やり直し");
				EditorGUILayout.LabelField("Y：元に戻す");
				EditorGUILayout.LabelField("A：全パーツ選択");
				EditorGUILayout.LabelField("Q：座標切替");

				switch (timelineType_)
				{
					case TimelineType.TL_POS:
						EditorGUILayout.LabelField("左クリック：パーツ選択");
						EditorGUILayout.LabelField("右クリック：パーツ選択解除");
						EditorGUILayout.LabelField("上下左右：パーツ移動");
						EditorGUILayout.LabelField("-：位置リセット");
						break;
					case TimelineType.TL_TRANSFORM:
						EditorGUILayout.LabelField("左クリック：パーツ選択");
						EditorGUILayout.LabelField("右クリック：パーツ選択解除");
						EditorGUILayout.LabelField("12345：向き前横後");
						EditorGUILayout.LabelField("79：角度");
						EditorGUILayout.LabelField("8：反転");
						EditorGUILayout.LabelField("-：リセット");
						break;
					case TimelineType.TL_MOVE:
						break;
					case TimelineType.TL_ATARI:
						EditorGUILayout.LabelField("左クリック：当たり判定ON／OFF");
						EditorGUILayout.LabelField("右クリック：当たり判定OFF");
						break;
					case TimelineType.TL_HOLD:
						EditorGUILayout.LabelField("上下左右：移動");
						EditorGUILayout.LabelField("79：角度");
						EditorGUILayout.LabelField("8：反転");
						EditorGUILayout.LabelField("-：リセット");
						break;
					case TimelineType.TL_THROW:
						break;
					case TimelineType.TL_COLOR:
						break;
					case TimelineType.TL_EFFECT:
						break;
					case TimelineType.TL_PASSIVE:
						break;
				}
			}

		}

		///<summary>グリッド線を描画</summary>
		void DrawGridLine3()
		{
			// grid
			Handles.color = new Color(1f, 1f, 1f, 0.3f);

			//縦線
			{
				Vector2 st = new Vector2(0, -64);
				Vector2 ed = new Vector2(0, 0);
				Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
			}

			//横線
			{
				Vector2 st = new Vector2(-64, 0);
				Vector2 ed = new Vector2(64, 0);
				Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
			}

		}

		///<summary>方眼線</summary>
		void DrawGridLine3Sub()
		{
			for (int i = 0; i < 8; i++)
			{
				//縦線
				{
					Vector2 st = new Vector2(i * 8, -64);
					Vector2 ed = new Vector2(i * 8, 0);
					Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
				}

				//縦線
				{
					Vector2 st = new Vector2(i * -8, -64);
					Vector2 ed = new Vector2(i * -8, 0);
					Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
				}

				//横線
				{
					Vector2 st = new Vector2(-64, i * -8);
					Vector2 ed = new Vector2(64, i * -8);
					Handles.DrawLine((camPos_ + st) * mag_, (camPos_ + ed) * mag_);
				}
			}
		}

		///<summary>原点表示線</summary>
		void DrawGridLine3Cross(Vector2 pos, Color col)
		{
			if (pos == Vector2.zero)return;
			// grid
			Handles.color = col;

			//縦線
			{
				Vector2 st = new Vector2(0, -32);
				Vector2 ed = new Vector2(0, 32);
				Handles.DrawLine((camPos_ + st + pos) * mag_, (camPos_ + ed + pos) * mag_);
			}

			//横線
			{
				Vector2 st = new Vector2(-32, 0);
				Vector2 ed = new Vector2(32, 0);
				Handles.DrawLine((camPos_ + st + pos) * mag_, (camPos_ + ed + pos) * mag_);
			}
		}
		///<summary>全パーツ描画</summary>
		void DrawPartsAll()
		{
			try
			{
				List<enPartsType> drawList = BasePosition.GenGetZSortList(sendMotion_.stPassive.isLeft, sendMotion_.stPassive.isBack);

				if (focusObject_ == enFocusObject.focusTack)
				{
					foreach (var item in drawList)
					{
						//非選択
						if (!isMultiParts_[PartsConverter.Convert(item)])
						{
							DrawParts(item, false);
						}
					}
					foreach (var item in drawList)
					{
						if (isMultiParts_[PartsConverter.Convert(item)])
						{
							DrawParts(item, false);
						}
					}
				}
				else
				{
					foreach (var item in drawList)
					{
						DrawParts(item, false);
					}
				}

				//パーツギズモ
				foreach (var item in drawList)
				{
					DrawPartsGizmoPoint(item);
				}

				//パーツギズモ

				DrawPartsGizmoLine(drawList);

			}
			catch
			{

			}

		}

		///<summary>グレーアウト表示</summary>
		bool IsDark(enPartsType partsType)
		{
			bool res = false;
			if (focusObject_ == enFocusObject.focusTack)
			{
				var activeTack = parent_.GetActiveScore().GetActiveTackPoint();
				switch (timelineType_)
				{
					case TimelineType.TL_POS:
					case TimelineType.TL_TRANSFORM:
						res = (isMultiParts_[PartsConverter.Convert(partsType)] == false);
						break;
					case TimelineType.TL_MOVE:
						break;
						//case TimelineType.TL_ATARI:
						//	res = !activeTack.motionData_.mAtari.IsAtari(partsType);
						//	break;
						//case TimelineType.TL_HOLD:
						//	res = !isHold;
						//	break;
					case TimelineType.TL_COLOR:
						res = !activeTack.motionData_.mColor.IsActive(partsType);
						break;
					case TimelineType.TL_EFFECT:
						break;
					case TimelineType.TL_PASSIVE:
						break;
				}
			}
			return res;
		}

		///<summary>各パーツ描画</summary>
		void DrawParts(enPartsType partsType, bool isLabel)
		{

			PartsObject partsObject = GetPartsObject(partsType);
			Vector2 pos = partsObject.pos;
			//上下反転
			pos.y = -pos.y;

			Sprite sp = parent_.GetSprite(partsType, sendMotion_.stPassive.isBack, sendMotion_.stPassive.faceNo);
			if (sp != null)
			{
				Vector2 basepos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
				Vector2 size = new Vector2(sp.rect.width, sp.rect.height);

				Vector2 drawPos = Vector2.zero;
				int MAG = mag_;
				if (isLabel)
				{
					MAG = 5;
					Vector2 labelpos = new Vector2(128, 256) / MAG;
					drawPos = (basepos + labelpos + pos);
				}
				else
				{
					drawPos = (basepos + camPos_ + pos + tempMovePos_);
				}

				Rect drawRect = new Rect(drawPos * MAG, size * MAG);
				if (sendMotion_.stPassive.isLeft)
				{
					drawRect.x += drawRect.width;
					drawRect.width = -drawRect.width;
				}
				Vector2 rotatePivot = new Vector2(drawRect.center.x, drawRect.center.y);
				float rotate = partsObject.partsTransform.rotate;

				if ((partsType == enPartsType.LeftArm)
					|| (partsType == enPartsType.LeftLeg))
				{
					GUI.color = (isLabel || IsDark(partsType))
						? new Color(0.75f, 0.5f, 0.5f)
						: new Color(1, 0.8f, 0.8f);
				}
				else
				{
					GUI.color = (isLabel || IsDark(partsType))
						? new Color(0.5f, 0.5f, 0.5f)
						: new Color(1, 1, 1);
				}

				GUIUtility.RotateAroundPivot(-rotate, rotatePivot);
				GUI.DrawTextureWithTexCoords(drawRect, sp.texture, GetSpriteNormalRect(sp)); //描画

				// RotateAroundPivot等は行列の掛け算なので、一旦初期値に戻す
				GUI.matrix = Matrix4x4.identity;
				GUI.color = new Color(1, 1, 1);

				var rectPos = drawPos * MAG;
				rectPos.x += drawRect.width / 2;
				rectPos.y += drawRect.height / 2;

			}

		}

		///<summary>各パーツのギズモ描画</summary>
		void DrawPartsGizmoPoint(enPartsType partsType)
		{
			PartsObject partsObject = GetPartsObject(partsType);

			Vector2 pos = partsObject.pos;

			pos.y = -pos.y; //上下反転

			Sprite sp = parent_.GetSprite(partsType, sendMotion_.stPassive.isBack, sendMotion_.stPassive.faceNo);
			if (sp != null)
			{
				Vector2 basepos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
				Vector2 size = new Vector2(sp.rect.width, sp.rect.height);

				int MAG = mag_;
				Vector2 drawPos = (basepos + camPos_ + pos + tempMovePos_);
				Rect drawRect = new Rect(drawPos * MAG, size * MAG);
				Handles.color = new Color(1, 1, 1);
				switch (partsType)
				{
					case enPartsType.LeftArm:
					case enPartsType.LeftHand:
					case enPartsType.LeftLeg:
					case enPartsType.LeftFoot:
						Handles.color = new Color(0, 1, 1);
						break;
					case enPartsType.RightArm:
					case enPartsType.RightHand:
					case enPartsType.RightLeg:
					case enPartsType.RightFoot:
						Handles.color = new Color(1, 0, 1);
						break;
				}

				var rectPos = drawPos;
				rectPos.x += sp.rect.width / 2;
				rectPos.y += sp.rect.height / 2;

				Handles.DrawRectangle((int)partsType, rectPos * MAG, Quaternion.identity, GIZMO_SIZE * MAG);
				// Handles.DrawLine(drawPos * MAG, drawPos * MAG * 2);

			}

		}

		///<summary>各パーツのギズモ描画</summary>
		void DrawPartsGizmoLine(List<enPartsType> drawList)
		{

			foreach (var partsType in drawList)
			{

				if (partsType == enPartsType.Body)
					continue;

				enPartsType targetPartsType = enPartsType.Body;
				switch (partsType)
				{
					case enPartsType.Ant:
						targetPartsType = enPartsType.Head;
						break;
					case enPartsType.LeftFoot:
						targetPartsType = enPartsType.LeftLeg;
						break;
					case enPartsType.RightFoot:
						targetPartsType = enPartsType.RightLeg;
						break;
					case enPartsType.LeftHand:
						targetPartsType = enPartsType.LeftArm;
						break;
					case enPartsType.RightHand:
						targetPartsType = enPartsType.RightArm;
						break;
				}

				PartsObject partsObject = GetPartsObject(partsType);
				PartsObject targetObject = GetPartsObject(targetPartsType);

				Vector2 pos = partsObject.pos;
				Vector2 targetPos = targetObject.pos;

				pos.y = -pos.y; //上下反転
				targetPos.y = -targetPos.y; //上下反転

				Sprite sp = parent_.GetSprite(partsType, sendMotion_.stPassive.isBack, sendMotion_.stPassive.faceNo);
				Sprite targetsp = parent_.GetSprite(targetPartsType, sendMotion_.stPassive.isBack, sendMotion_.stPassive.faceNo);
				if ((sp != null) && (targetsp != null))
				{
					Vector2 basepos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
					Vector2 size = new Vector2(sp.rect.width, sp.rect.height);

					int MAG = mag_;
					Vector2 drawPos = (basepos + camPos_ + pos + tempMovePos_);
					drawPos.x += sp.rect.width / 2;
					drawPos.y += sp.rect.height / 2;
					Vector2 drawTargetPos = (basepos + camPos_ + targetPos + tempMovePos_);
					drawTargetPos.x += targetsp.rect.width / 2;
					drawTargetPos.y += targetsp.rect.height / 2;

					switch (partsType)
					{
						case enPartsType.LeftArm:
							drawTargetPos.x += GIZMO_SIZE;
							drawTargetPos.y -= GIZMO_SIZE;
							break;
						case enPartsType.RightArm:
							drawTargetPos.x -= GIZMO_SIZE;
							drawTargetPos.y -= GIZMO_SIZE;
							break;
						case enPartsType.LeftLeg:
							drawTargetPos.x += GIZMO_SIZE;
							drawTargetPos.y += GIZMO_SIZE;
							break;
						case enPartsType.RightLeg:
							drawTargetPos.x -= GIZMO_SIZE;
							drawTargetPos.y += GIZMO_SIZE;
							break;
					}

					Handles.color = new Color(1, 1, 0);
					switch (partsType)
					{
						case enPartsType.LeftArm:
						case enPartsType.LeftHand:
						case enPartsType.LeftLeg:
						case enPartsType.LeftFoot:
							Handles.color = new Color(0, 1, 1);
							break;
						case enPartsType.RightArm:
						case enPartsType.RightHand:
						case enPartsType.RightLeg:
						case enPartsType.RightFoot:
							Handles.color = new Color(1, 0, 1);
							break;
					}

					Handles.DrawLine(drawPos * MAG, drawTargetPos * MAG);

				}

			}

		}

		///<summary>パーツラベル描画</summary>
		void DrawPartsLabel()
		{
			List<enPartsType> drawList = BasePosition.GenGetZSortList(sendMotion_.stPassive.isLeft, sendMotion_.stPassive.isBack);
			foreach (var item in drawList)DrawParts(item, false);

			const int MAG = 5;

			foreach (var item in drawList)
			{
				PartsObject partsObject = GetPartsObject(item);

				Vector2 pos = partsObject.pos;
				pos.y = -pos.y; //上下反転
				//bool mirror = partsObject.partsTransform.mirror;
				Sprite sp = parent_.GetSprite(item, sendMotion_.stPassive.isBack, sendMotion_.stPassive.faceNo);

				if (sp == null)break;

				Vector2 basepos = new Vector2(-sp.pivot.x, +sp.pivot.y - sp.rect.height);
				//Vector2 size = new Vector2(sp.rect.width, sp.rect.height);

				Vector2 labelpos = new Vector2(128, 256) / MAG;
				Vector2 drawPos = (basepos + labelpos + pos);
				// GUIの見た目を変える。
				GUIStyle guiStyle = new GUIStyle();
				GUIStyleState styleState = new GUIStyleState();

				string vecstr = "";
				if (isSabunPos_)
				{
					// テキストの色を設定
					styleState.textColor = Color.yellow;
					vecstr = "(" + ((int)(sendMotion_.stPos.GetPos(item).x)).ToString() + "," + ((int)(sendMotion_.stPos.GetPos(item).y)).ToString() + ")";
				}
				else
				{
					// テキストの色を設定
					styleState.textColor = Color.white;
					vecstr = "(" + ((int)(drawPos.x)).ToString() + "," + ((int)(drawPos.y)).ToString() + ")";
				}

				// スタイルの設定。
				guiStyle.normal = styleState;
				guiStyle.alignment = TextAnchor.MiddleCenter;

				Vector2 labelPos = new Vector2(drawPos.x, drawPos.y);
				Rect labelRect = new Rect(labelPos * MAG, new Vector2(sp.rect.width, sp.rect.height) * MAG);

				GUI.Label(labelRect, vecstr, guiStyle);
			}

		}

		///<summary>スプライトの大きさ取得</summary>
		public static Rect GetSpriteNormalRect(Sprite sp)
		{
			// spriteの親テクスチャー上のRect座標を取得.
			Rect rectPosition = sp.textureRect;

			// 親テクスチャーの大きさを取得.
			float parentWith = sp.texture.width;
			float parentHeight = sp.texture.height;
			// spriteの座標を親テクスチャーに合わせて正規化.
			Rect NormalRect = new Rect(
				rectPosition.x / parentWith,
				rectPosition.y / parentHeight,
				rectPosition.width / parentWith,
				rectPosition.height / parentHeight
			);

			return NormalRect;
		}

	}
}