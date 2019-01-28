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
		TackPointModel model = new TackPointModel();
		TackPointViewModel viewModel = new TackPointViewModel();
		TackPointInspector tackPointInspector_;
		TackPointView view = new TackPointView();
		TackPointOutput output = new TackPointOutput();

		public void SetModels()
		{
			view.SetModels(model, viewModel);
			output.SetModels(model);
		}

		///<summary>空のTack</summary>
		public TackPoint()
		{
			SetModels();
		}

		///<summary>新規作成</summary>
		public TackPoint(int index, int start, int span, int timelineType)
		{
			SetModels();
			model.TackId = WindowSettings.ID_HEADER_TACK + Guid.NewGuid().ToString();
			model.Index = index;
			model.IsExistTack = true;
			model.Start = start;
			model.Span = span;
			model.TimelineType = timelineType;
		}

		///<summary>ファイルからの読み込み</summary>
		public TackPoint(int index, Dictionary<string, object> timelineTacksDict)
		{
			SetModels();
			model.TackId = WindowSettings.ID_HEADER_TACK + Guid.NewGuid().ToString();
			model.Index = index;
			model.IsExistTack = true;
			model.Start = Convert.ToInt32(timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_START]);
			model.Span = Convert.ToInt32(timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_SPAN]);
			model.TimelineType = Convert.ToInt32(timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TIMELINETYPE]);

			string dictToStr = "";

			switch ((TimelineType)model.TimelineType)
			{
				case TimelineType.TL_POS:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_POSDATA] as string;
					model.MotionData.mPos = JsonUtility.FromJson<MotionPos>(dictToStr);
					break;
				case TimelineType.TL_TRANSFORM:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TRANSFORMDATA] as string;
					model.MotionData.mTransform = JsonUtility.FromJson<MotionTransform>(dictToStr);
					break;
				case TimelineType.TL_COLOR:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_COLORDATA] as string;
					model.MotionData.mColor = JsonUtility.FromJson<MotionColor>(dictToStr);
					break;
				case TimelineType.TL_EFFECT:
					dictToStr = timelineTacksDict[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_EFFECTDATA] as string;
					model.MotionData.mEffect = JsonUtility.FromJson<MotionEffect>(dictToStr);
					break;
				default:
					Debug.LogError("other timelineType_");
					break;
			}
		}

		///<summary>ペースト</summary>
		public TackPoint(int index, int start, TackPoint srcTack)
		{
			SetModels();
			model.TackId = WindowSettings.ID_HEADER_TACK + Guid.NewGuid().ToString();
			model.Index = index;
			model.Start = start;
			model.IsExistTack = true;
			model.Span = srcTack.model.Span;
			model.TimelineType = srcTack.model.TimelineType;

			switch ((TimelineType)model.TimelineType)
			{
				case TimelineType.TL_POS:
					model.MotionData.mPos = srcTack.model.MotionData.mPos;
					break;
				case TimelineType.TL_TRANSFORM:
					model.MotionData.mTransform = srcTack.model.MotionData.mTransform;
					break;
				case TimelineType.TL_COLOR:
					model.MotionData.mColor = srcTack.model.MotionData.mColor;
					break;
				case TimelineType.TL_EFFECT:
					model.MotionData.mEffect = srcTack.model.MotionData.mEffect;
					break;
				default:
					Debug.LogError("other timelineType_");
					break;
			}
		}

		public void SetActive()
		{
			model.Active = true;

			ApplyDataToInspector();
			Selection.activeObject = tackPointInspector_;
		}

		public void SetDeactive()
		{
			model.Active = false;
		}

		public bool IsActive()
		{
			return model.Active;
		}

		public void BeforeSave()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_BEFORESAVE, model.TackId, model.Start));
		}

		public void Save()
		{
			Emit(new OnTrackEvent(OnTrackEvent.EventType.EVENT_TACK_SAVE, model.TackId, model.Start));
		}

		public void Deleted(bool isCancel)
		{
			model.IsExistTack = isCancel;
		}

		public bool ContainsFrame(int frame)
		{
			if (model.Start <= frame && frame <= model.Start + model.Span - 1)return true;
			return false;
		}

		public void UpdatePos(int start, int span)
		{
			model.Start = start;
			model.Span = span;
			ApplyDataToInspector();
		}

		public int GetLastStart()
		{
			return viewModel.lastStart_;
		}

		public int GetLastSpan()
		{
			return viewModel.lastSpan_;
		}

		public int GetStart()
		{
			return model.Start;
		}

		public int GetSpan()
		{
			return model.Span;
		}

		public int GetTimelineType()
		{
			return model.TimelineType;
		}

		public MotionData GetMotionData()
		{
			return model.MotionData;
		}

		public void ApplyDataToInspector()
		{
			if (tackPointInspector_ == null)tackPointInspector_ = ScriptableObject.CreateInstance("TackPointInspector")as TackPointInspector;

			tackPointInspector_.UpdateTackPoint(this);
		}
	}
}