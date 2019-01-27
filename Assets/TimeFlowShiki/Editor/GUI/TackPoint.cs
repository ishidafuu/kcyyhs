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

	[Serializable]
	public class TackPoint
	{
		public static Action<OnTrackEvent> Emit;

		[SerializeField]
		private TackPointInspector tackPointInspector_;

		[SerializeField]
		public string tackId_;
		[SerializeField]
		public string parentTimelineId_;
		[SerializeField]
		private int index_;

		[SerializeField]
		private bool active_ = false;
		[SerializeField]
		public bool IsExistTack_ = true;

		//���ێ��f�[�^
		[SerializeField]
		public int start_;
		[SerializeField]
		public int span_;

		[SerializeField]
		public int timelineType_;

		[SerializeField]
		public MotionData motionData_;

		[SerializeField]
		private Texture2D tackBackTransparentTex_;
		[SerializeField]
		private Texture2D tackColorTex_;

		private Vector2 distance_ = Vector2.zero;
		private int lastStart_;
		private int lastSpan_;

		private enum TackModifyMode : int
		{
			NONE,
			GRAB_BODY,
			GRAB_END,
			DRAG_BODY,
			DRAG_END,
		}
		private TackModifyMode mode_ = TackModifyMode.NONE;

		private Vector2 dragBeginPoint_;
		private GUIStyle labelStyle_;

		public TackPoint()
		{

		}

		public TackPoint(int index, int start, int span, int timelineType)
		{
			this.tackId_ = WindowSettings.ID_HEADER_TACK + Guid.NewGuid().ToString();
			this.index_ = index;
			this.IsExistTack_ = true;
			this.start_ = start;
			this.span_ = span;
			this.timelineType_ = timelineType;
		}

		public TackPoint(int index, Dictionary<string, object> timelineTacksDict)
		{
			this.tackId_ = WindowSettings.ID_HEADER_TACK + Guid.NewGuid().ToString();
			this.index_ = index;

			this.IsExistTack_ = true;
			//this.title_ = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TITLE] as string;
			this.start_ = Convert.ToInt32(timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_START]);
			this.span_ = Convert.ToInt32(timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_SPAN]);
			this.timelineType_ = Convert.ToInt32(timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TIMELINETYPE]);

			string dictToStr = "";

			switch ((TimelineType)this.timelineType_)
			{
				case TimelineType.TL_POS:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_POSDATA] as string;
					motionData_.mPos = JsonUtility.FromJson<MotionPos>(dictToStr);
					break;
				case TimelineType.TL_TRANSFORM:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TRANSFORMDATA] as string;
					motionData_.mTransform = JsonUtility.FromJson<MotionTransform>(dictToStr);
					break;
				case TimelineType.TL_MOVE:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_MOVEDATA] as string;
					motionData_.mMove = JsonUtility.FromJson<MotionMove>(dictToStr);
					break;
					//case TimelineType.TL_ATARI:
					//	dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_ATARIDATA] as string;
					//	motionData_.mAtari = JsonUtility.FromJson<MotionAtari>(dictToStr);
					//	break;
					//case TimelineType.TL_HOLD:
					//	dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_HOLDDATA] as string;
					//	motionData_.mHold = JsonUtility.FromJson<MotionHold>(dictToStr);
					//	break;
					//case TimelineType.TL_THROW:
					//	dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_THROWDATA] as string;
					//	motionData_.mThrow = JsonUtility.FromJson<MotionThrow>(dictToStr);
					//	break;
				case TimelineType.TL_COLOR:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_COLORDATA] as string;
					motionData_.mColor = JsonUtility.FromJson<MotionColor>(dictToStr);
					break;
				case TimelineType.TL_EFFECT:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_EFFECTDATA] as string;
					motionData_.mEffect = JsonUtility.FromJson<MotionEffect>(dictToStr);
					break;
				case TimelineType.TL_PASSIVE:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_PASSIVEDATA] as string;
					motionData_.mPassive = JsonUtility.FromJson<MotionPassive>(dictToStr);
					break;
				default:
					Debug.LogError("other timelineType_");
					break;
			}
		}

		//�R�s�y�p
		public TackPoint(int index, int start, TackPoint srcTack)
		{
			this.tackId_ = WindowSettings.ID_HEADER_TACK + Guid.NewGuid().ToString();
			this.index_ = index;
			this.start_ = start; //�C���f�b�N�X�ƃX�^�[�g�ʒu�͐V�K

			this.IsExistTack_ = true;

			//���f�[�^�ǉ��̍ۂ̓R�R�ɒǉ�
			//this.title_ = srcTack.title_;
			this.span_ = srcTack.span_;
			this.timelineType_ = srcTack.timelineType_;

			//�e��^�C�����C��
			switch ((TimelineType)this.timelineType_)
			{
				case TimelineType.TL_POS:
					motionData_.mPos = srcTack.motionData_.mPos;
					break;
				case TimelineType.TL_TRANSFORM:
					motionData_.mTransform = srcTack.motionData_.mTransform;
					break;
				case TimelineType.TL_MOVE:
					motionData_.mMove = srcTack.motionData_.mMove;
					break;
					//case TimelineType.TL_ATARI: motionData_.mAtari = srcTack.motionData_.mAtari; break;
					//case TimelineType.TL_HOLD: motionData_.mHold = srcTack.motionData_.mHold; break;
					//case TimelineType.TL_THROW: motionData_.mThrow = srcTack.motionData_.mThrow; break;
				case TimelineType.TL_COLOR:
					motionData_.mColor = srcTack.motionData_.mColor;
					break;
				case TimelineType.TL_EFFECT:
					motionData_.mEffect = srcTack.motionData_.mEffect;
					break;
				case TimelineType.TL_PASSIVE:
					motionData_.mPassive = srcTack.motionData_.mPassive;
					break;
				default:
					Debug.LogError("other timelineType_");
					break;
			}
		}

		//���f�[�^�ǉ��̍ۂ̓R�R�ɒǉ�
		public Dictionary<string, object> OutputDict()
		{
			var res = new Dictionary<string, object>
				{
					//{TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TITLE, this.title_},
					{ TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_START, this.start_ },
					{ TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_SPAN, this.span_ },
					{ TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TIMELINETYPE, this.timelineType_ },
				};

			//DebugPanel.Log("motionTransformJSON", JsonUtility.ToJson(motionData_.mTransform));

			//�e��^�C�����C��
			switch ((TimelineType)this.timelineType_)
			{
				case TimelineType.TL_POS:
					res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_POSDATA] = JsonUtility.ToJson(motionData_.mPos);
					break;
				case TimelineType.TL_TRANSFORM:
					res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TRANSFORMDATA] = JsonUtility.ToJson(motionData_.mTransform);
					break;
				case TimelineType.TL_MOVE:
					res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_MOVEDATA] = JsonUtility.ToJson(motionData_.mMove);
					break;
					//case TimelineType.TL_ATARI:
					//	res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_ATARIDATA] = JsonUtility.ToJson(motionData_.mAtari);
					//	break;
					//case TimelineType.TL_HOLD:
					//	res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_HOLDDATA] = JsonUtility.ToJson(motionData_.mHold);
					//	break;
					//case TimelineType.TL_THROW:
					//	res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_THROWDATA] = JsonUtility.ToJson(motionData_.mThrow);
					//	break;
				case TimelineType.TL_COLOR:
					res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_COLORDATA] = JsonUtility.ToJson(motionData_.mColor);
					break;
				case TimelineType.TL_EFFECT:
					res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_EFFECTDATA] = JsonUtility.ToJson(motionData_.mEffect);
					break;
				case TimelineType.TL_PASSIVE:
					res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_PASSIVEDATA] = JsonUtility.ToJson(motionData_.mPassive);
					break;
				default:
					Debug.LogError("other timelineType_");
					break;

			}
			return res;
		}

		//�X�N���v�^�u���I�u�W�F�p�o�́i�^�C�v���Ƃ̏o�́j
		//�e��^�C�����C��
		public MotionTackPos OutputMotionTackPos()
		{
			return new MotionTackPos(start_, span_, motionData_.mPos);
		}
		public MotionTackTransform OutputMotionTackTransform()
		{
			return new MotionTackTransform(start_, span_, motionData_.mTransform);
		}
		public MotionTackMove OutputMotionTackMove()
		{
			return new MotionTackMove(start_, span_, motionData_.mMove);
		}
		//public MotionTackAtari OutputMotionTackAtari()
		//{
		//	return new MotionTackAtari(start_, span_, motionData_.mAtari);
		//}
		//public MotionTackHold OutputMotionTackHold()
		//{
		//	return new MotionTackHold(start_, span_, motionData_.mHold);
		//}
		//public MotionTackThrow OutputMotionTackThrow()
		//{
		//	return new MotionTackThrow(start_, span_, motionData_.mThrow);
		//}
		public MotionTackColor OutputMotionTackColor()
		{
			return new MotionTackColor(start_, span_, motionData_.mColor);
		}
		public MotionTackEffect OutputMotionTackEffect()
		{
			return new MotionTackEffect(start_, span_, motionData_.mEffect);
		}
		public MotionTackPassive OutputMotionTackPassive()
		{
			return new MotionTackPassive(start_, span_, motionData_.mPassive);
		}

		public Texture2D GetColorTex()
		{
			return tackColorTex_;
		}

		public void InitializeTackTexture(Texture2D baseTex)
		{
			GenerateTextureFromBaseTexture(baseTex, index_);

			labelStyle_ = new GUIStyle();
			labelStyle_.normal.textColor = Color.white;
			labelStyle_.fontSize = 10;
			labelStyle_.wordWrap = true;
		}

		public void SetActive()
		{
			active_ = true;

			ApplyDataToInspector();
			Selection.activeObject = tackPointInspector_;
		}

		public void SetDeactive()
		{
			active_ = false;
		}

		public bool IsActive()
		{
			return active_;
		}

		//�`��ƃ}�E�X�C�x���g�̃n���h�����O
		public void DrawTack(Rect limitRect, string parentTimelineId, float startX, float startY, bool isUnderEvent)
		{
			if (!IsExistTack_)return;

			this.parentTimelineId_ = parentTimelineId;

			var tackBGRect = DrawTackPointInRect(startX, startY);

			var globalMousePos = Event.current.mousePosition;

			var useEvent = false;

			var localMousePos = new Vector2(globalMousePos.x - tackBGRect.x, globalMousePos.y - tackBGRect.y);
			var sizeRect = new Rect(0, 0, tackBGRect.width, tackBGRect.height);

			if (!isUnderEvent)return;

			// mouse event Armling.
			switch (this.mode_)
			{
				case TackModifyMode.NONE:
					{
						//�}�E�X�����ꂽ�u��
						useEvent = BeginTackModify(tackBGRect, globalMousePos);
						break;
					}

				case TackModifyMode.GRAB_BODY:
				case TackModifyMode.GRAB_END:
					{
						//�h���b�O���n�߂��u��
						useEvent = RecognizeTackModify(globalMousePos);
						break;
					}
				case TackModifyMode.DRAG_BODY:
				case TackModifyMode.DRAG_END:
					{
						//�}�E�X���ړ����邽��&�}�E�X�����ꂽ�Ƃ�
						useEvent = UpdateTackModify(limitRect, tackBGRect, globalMousePos);
						break;
					}
			}

			if (sizeRect.Contains(localMousePos))
			{
				switch (Event.current.type)
				{

					case EventType.ContextClick:
						{
							ShowContextMenu();
							useEvent = true;
							break;
						}

						// clicked.
					case EventType.MouseUp:
						{
							// right click.
							if (Event.current.button == 1)
							{
								ShowContextMenu();
								useEvent = true;
								break;
							}

							Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, tackId_));
							useEvent = true;
							break;
						}
						//break;
				}
			}

			if (useEvent)
			{
				Event.current.Use();
			}
		}

		public void BeforeSave()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_BEFORESAVE, this.tackId_, start_));
		}

		public void Save()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_SAVE, this.tackId_, start_));
		}

		private void ShowContextMenu()
		{
			var framePoint = start_;
			var menu = new GenericMenu();

			var menuItems = new Dictionary<string, OnTrackEvent.EventType>
				{ { "Delete This Tack", OnTrackEvent.EventType.EVENT_TACK_DELETED },
					{ "Copy Tack", OnTrackEvent.EventType.EVENT_TACK_COPY },
				};

			foreach (var key in menuItems.Keys)
			{
				var eventType = menuItems[key];
				menu.AddItem(
					new GUIContent(key),
					false,
					() =>
					{
						Emit(new OnTrackEvent(eventType, this.tackId_, framePoint));
					}
				);
			}
			menu.ShowAsContext();
		}

		private void GenerateTextureFromBaseTexture(Texture2D baseTex, int index)
		{
			var samplingColor = baseTex.GetPixels()[0];
			var rgbVector = new Vector3(samplingColor.r, samplingColor.g, samplingColor.b);

			var rotatedVector = Quaternion.AngleAxis(12.5f * index, new Vector3(1.5f * index, 1.25f * index, 1.37f * index)) * rgbVector;

			var slidedColor = new Color(rotatedVector.x, rotatedVector.y, rotatedVector.z, 1);

			this.tackBackTransparentTex_ = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			tackBackTransparentTex_.SetPixel(0, 0, new Color(slidedColor.r, slidedColor.g, slidedColor.b, 0.5f));
			tackBackTransparentTex_.Apply();

			this.tackColorTex_ = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			tackColorTex_.SetPixel(0, 0, new Color(slidedColor.r, slidedColor.g, slidedColor.b, 1.0f));
			tackColorTex_.Apply();
		}

		private Rect DrawTackPointInRect(float startX, float startY)
		{
			var tackStartPointX = startX + (start_ * WindowSettings.TACK_FRAME_WIDTH);
			var end = start_ + span_ - 1;
			var tackEndPointX = startX + (end * WindowSettings.TACK_FRAME_WIDTH);

			var tackBGRect = new Rect(tackStartPointX, startY, span_ * WindowSettings.TACK_FRAME_WIDTH + 1f, WindowSettings.TACK_HEIGHT);

			switch (mode_)
			{
				//case TackModifyMode.DRAG_START:
				//	{
				//		tackStartPointX = startX + (start_ * WindowSettings.TACK_FRAME_WIDTH) + distance_.x;
				//		tackBGRect = new Rect(tackStartPointX, startY, span_ * WindowSettings.TACK_FRAME_WIDTH + 1f - distance_.x, WindowSettings.TACK_HEIGHT);
				//		break;
				//	}
				case TackModifyMode.DRAG_BODY:
					{
						tackStartPointX = startX + (start_ * WindowSettings.TACK_FRAME_WIDTH) + distance_.x;
						tackEndPointX = startX + (end * WindowSettings.TACK_FRAME_WIDTH) + distance_.x;
						tackBGRect = new Rect(tackStartPointX, startY, span_ * WindowSettings.TACK_FRAME_WIDTH + 1f, WindowSettings.TACK_HEIGHT);
						break;
					}
				case TackModifyMode.DRAG_END:
					{
						tackEndPointX = startX + (end * WindowSettings.TACK_FRAME_WIDTH) + distance_.x;
						tackBGRect = new Rect(tackStartPointX, startY, span_ * WindowSettings.TACK_FRAME_WIDTH + distance_.x + 1f, WindowSettings.TACK_HEIGHT);
						break;
					}
			}

			// draw tack.
			{
				// draw bg.
				var frameBGRect = new Rect(tackBGRect.x, tackBGRect.y, tackBGRect.width, WindowSettings.TACK_FRAME_HEIGHT);

				GUI.DrawTexture(frameBGRect, tackBackTransparentTex_);

				// draw points.
				{
					// tackpoint back line.
					if (span_ == 1)GUI.DrawTexture(new Rect(tackBGRect.x + (WindowSettings.TACK_FRAME_WIDTH / 3) + 1, startY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 1, (WindowSettings.TACK_FRAME_WIDTH / 3) - 1, 11), tackColorTex_);
					if (1 < span_)GUI.DrawTexture(new Rect(tackBGRect.x + (WindowSettings.TACK_FRAME_WIDTH / 2), startY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 1, tackEndPointX - tackBGRect.x, 11), tackColorTex_);

					// frame start point.
					DrawTackPoint(start_, tackBGRect.x, startY);

					// frame end point.
					if (1 < span_)DrawTackPoint(end, tackEndPointX, startY);
				}

				var routineComponentY = startY + WindowSettings.TACK_FRAME_HEIGHT;

				// routine component.
				{
					var height = WindowSettings.ROUTINE_HEIGHT_DEFAULT;
					if (active_)GUI.DrawTexture(new Rect(tackBGRect.x, routineComponentY, tackBGRect.width, height), WindowSettings.activeTackBaseTex);

					GUI.DrawTexture(new Rect(tackBGRect.x + 1, routineComponentY, tackBGRect.width - 2, height - 1), tackColorTex_);

					//�^�b�N������
					string labelText = start_.ToString() + " - " + span_.ToString(); // +"\n"+ title_;

					//switch ((TimelineType)timelineType_)
					//{
					//	case TimelineType.TL_ATARI:
					//		if (motionData_.mAtari.isBomb) labelText += "\n" + "isBomb";
					//		if (motionData_.mAtari.IsAtariAny()) labelText += "\n" + "isAtariAny";
					//		break;
					//}
					GUI.Label(new Rect(tackBGRect.x + 1, routineComponentY, tackBGRect.width - 2, height - 1), labelText, labelStyle_);
				}
			}

			return tackBGRect;
		}

		//�}�E�X�����ꂽ�u��
		private bool BeginTackModify(Rect tackBGRect, Vector2 beginPoint)
		{

			switch (Event.current.type)
			{
				case EventType.MouseDown:
					{
						//var startRect = new Rect(tackBGRect.x, tackBGRect.y, WindowSettings.TACK_FRAME_WIDTH, WindowSettings.TACK_FRAME_HEIGHT);
						//if (startRect.Contains(beginPoint))
						//{
						//	if (span_ == 1)
						//	{
						//		dragBeginPoint_ = beginPoint;
						//		mode = TackModifyMode.GRAB_HALF;
						//		return true;
						//	}
						//	dragBeginPoint_ = beginPoint;
						//	mode = TackModifyMode.GRAB_START;
						//	return true;
						//}
						var endRect = new Rect(tackBGRect.x + tackBGRect.width - WindowSettings.TACK_FRAME_WIDTH, tackBGRect.y, WindowSettings.TACK_FRAME_WIDTH, WindowSettings.TACK_FRAME_HEIGHT);
						if (endRect.Contains(beginPoint))
						{
							dragBeginPoint_ = beginPoint;
							mode_ = TackModifyMode.GRAB_END;
							return true;
						}
						if (tackBGRect.Contains(beginPoint))
						{
							dragBeginPoint_ = beginPoint;
							mode_ = TackModifyMode.GRAB_BODY;
							return true;
						}
						return false;
					}
			}

			return false;
		}

		//�h���b�O���n�߂��u��
		private bool RecognizeTackModify(Vector2 mousePos)
		{

			switch (Event.current.type)
			{
				case EventType.MouseDrag:
					{
						switch (mode_)
						{
							//case TackModifyMode.GRAB_START:
							//	{
							//		mode = TackModifyMode.DRAG_START;
							//		Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, tackId_));
							//		return true;
							//	}
							case TackModifyMode.GRAB_BODY:
								{
									mode_ = TackModifyMode.DRAG_BODY;
									Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, tackId_));
									return true;
								}
							case TackModifyMode.GRAB_END:
								{
									mode_ = TackModifyMode.DRAG_END;
									Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, tackId_));
									return true;
								}
								//case TackModifyMode.GRAB_HALF:
								//	{

								//		if (mousePos.x < dragBeginPoint_.x) mode = TackModifyMode.DRAG_START;
								//		if (dragBeginPoint_.x < mousePos.x) mode = TackModifyMode.DRAG_END;
								//		Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, tackId_));
								//		return true;
								//	}
						}

						return false;
					}
				case EventType.MouseUp:
					{
						mode_ = TackModifyMode.NONE;
						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_OBJECT_SELECTED, tackId_));
						return true;
					}
			}

			return false;
		}

		//�}�E�X���ړ����邽��
		private bool UpdateTackModify(Rect limitRect, Rect tackBGRect, Vector2 draggingPoint)
		{
			//�}�E�X�����ꂽ�Ƃ��i�g����O�ꂽ�j
			if (!limitRect.Contains(draggingPoint))
			{
				ExitUpdate(distance_);
				return true;
			}

			// far from bandwidth, exit mode.
			if (draggingPoint.y < 0 || tackBGRect.height + WindowSettings.TIMELINE_HEADER_HEIGHT < draggingPoint.y)
			{
				//�}�E�X�����ꂽ�Ƃ��i�g����O�ꂽ�j
				ExitUpdate(distance_);
				return true;
			}

			//Debug.Log(Event.current.type.ToString());

			switch (Event.current.type)
			{
				case EventType.MouseDrag:
					{
						Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_MOVING, tackId_));

						distance_ = draggingPoint - dragBeginPoint_;
						var distanceToFrame = DistanceToFrame(distance_.x);

						//�^�b�N�̒��̂ǂ������ꂽ��
						switch (mode_)
						{
							//case TackModifyMode.DRAG_START:
							//	{
							//		// limit 0 <= start
							//		if ((start_ + distanceToFrame) < 0) distance_.x = -FrameToDistance(start_);

							//		// limit start <= end
							//		if (span_ <= (distanceToFrame + 1)) distance_.x = FrameToDistance(span_ - 1);
							//		break;
							//	}
							case TackModifyMode.DRAG_BODY:
								{
									// limit 0 <= start
									if ((start_ + distanceToFrame) < 0)distance_.x = -FrameToDistance(start_);
									break;
								}
							case TackModifyMode.DRAG_END:
								{
									// limit start <= end
									if ((span_ + distanceToFrame) <= 1)distance_.x = -FrameToDistance(span_ - 1);
									break;
								}
						}

						return true;
					}
				case EventType.MouseUp:
					{
						//�}�E�X�����ꂽ�Ƃ�
						ExitUpdate(distance_);
						return true;
					}
			}

			return false;
		}

		//�}�E�X�����ꂽ�Ƃ�
		private void ExitUpdate(Vector2 currentDistance)
		{
			var distanceToFrame = DistanceToFrame(currentDistance.x);

			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_MOVED, tackId_));

			//lastStart_ = start_;
			//lastSpan_ = span_;

			var newStart = start_;
			var newSpan = span_;
			var lastStart = start_;
			var lastSpan = span_;

			//�ړ�����
			switch (mode_)
			{
				case TackModifyMode.DRAG_BODY: //�S�̂�ړ�
					{
						newStart = start_ + distanceToFrame;
						//Debug.Log("DRAG_BODY"+ newStart.ToString()+ newSpan.ToString());
						break;
					}
				case TackModifyMode.DRAG_END: //������L�΂�
					{
						newSpan = span_ + distanceToFrame;
						//Debug.Log("DRAG_END");
						break;
					}
			}

			if (newStart < 0)newStart = 0;

			//Debug.Log("start" + newStart.ToString() + "span"+newSpan.ToString());
			string id = MethodBase.GetCurrentMethod().Name;

			Action action = () =>
			{
				start_ = newStart;
				span_ = newSpan;
				lastStart_ = lastStart;
				lastSpan_ = lastSpan;
			};

			Action undo = () =>
			{
				start_ = lastStart;
				span_ = lastSpan;
				lastStart_ = lastStart;
				lastSpan_ = lastSpan;
			};

			ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));

			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_MOVED_AFTER, tackId_));

			mode_ = TackModifyMode.NONE;

			distance_ = Vector2.zero;
		}

		private int DistanceToFrame(float distX)
		{
			var distanceToFrame = (int)(distX / WindowSettings.TACK_FRAME_WIDTH);
			var distanceDelta = distX % WindowSettings.TACK_FRAME_WIDTH;

			// adjust behaviour by frame width.
			if (WindowSettings.BEHAVE_FRAME_MOVE_RATIO <= distanceDelta)distanceToFrame = distanceToFrame + 1;
			if (distanceDelta <= -WindowSettings.BEHAVE_FRAME_MOVE_RATIO)distanceToFrame = distanceToFrame - 1;

			return distanceToFrame;
		}

		private float FrameToDistance(int frame)
		{
			return WindowSettings.TACK_FRAME_WIDTH * frame;
		}

		private void DrawTackPoint(int frame, float pointX, float pointY)
		{
			if (span_ == 1)
			{
				if (frame % 5 == 0 && 0 < frame)
				{
					GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.grayPointSingleTex);
				}
				else
				{
					GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.whitePointSingleTex);
				}
				return;
			}

			if (frame % 5 == 0 && 0 < frame)
			{
				GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.grayPointTex);
			}
			else
			{
				GUI.DrawTexture(new Rect(pointX + 2, pointY + (WindowSettings.TACK_FRAME_HEIGHT / 3) - 2, WindowSettings.TACK_POINT_SIZE, WindowSettings.TACK_POINT_SIZE), WindowSettings.whitePointTex);
			}
		}

		public void Deleted(bool isCancel)
		{
			IsExistTack_ = isCancel;
		}

		public bool ContainsFrame(int frame)
		{
			if (start_ <= frame && frame <= start_ + span_ - 1)return true;
			return false;
		}

		public void UpdatePos(int start, int span)
		{
			//Debug.Log("UpdatePos" + start.ToString());
			this.start_ = start;
			this.span_ = span;
			ApplyDataToInspector();
		}

		public int GetLastStart()
		{
			return lastStart_;
		}

		public int GetLastSpan()
		{
			return lastSpan_;
		}

		public void ApplyDataToInspector()
		{
			if (tackPointInspector_ == null)tackPointInspector_ = ScriptableObject.CreateInstance("TackPointInspector")as TackPointInspector;

			tackPointInspector_.UpdateTackPoint(this);
		}
	}
}