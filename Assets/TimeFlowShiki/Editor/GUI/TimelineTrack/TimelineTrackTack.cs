using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NKKD.EDIT
{
	public class TimelineTrackTack
	{
		TimelineTrackView view;
		TimelineTrackModel model;
		TimelineTrackViewModel viewModel;
		TimelineTrackInspector timelineTrackInspector;

		public void SetModels(TimelineTrackView view, TimelineTrackModel model, TimelineTrackViewModel viewModel, TimelineTrackInspector timelineTrackInspector)
		{
			this.view = view;
			this.timelineTrackInspector = timelineTrackInspector;
			this.model = model;
			this.viewModel = viewModel;
		}

		public void SetMovingTack(string tackId)
		{
			viewModel.movingTackIds_ = new List<string> { tackId };
		}

		public TackPoint GetTackById(string tackId)
		{
			var selectTackIndex = model.tackPoints_.FindIndex(tack => tack.GetTackId() == tackId);
			if (selectTackIndex == -1)return null;
			return model.tackPoints_[selectTackIndex];
		}

		public TackPoint GetActiveTackPoint()
		{
			var res = model.tackPoints_.Where(t => t.IsActive()).FirstOrDefault();
			if (res == null)throw new Exception("no active TackPoint found.");
			return res;
		}

		public int GetStartFrameById(string objectId)
		{
			foreach (var tackPoint in model.tackPoints_)
			{
				if (tackPoint.GetTackId() == objectId)
					return tackPoint.GetStart();
			}
			return -1;
		}

		public void ActivateTacks(List<string> activeTackIds)
		{
			model.haveActiveTack_ = false;
			foreach (var tackPoint in model.tackPoints_)
			{
				if (activeTackIds.Contains(tackPoint.GetTackId()))
				{
					tackPoint.SetActive();
					model.haveActiveTack_ = true;
				}
				else
				{
					tackPoint.SetDeactive();
				}
			}
		}

		public void DeactivateTacks()
		{
			model.haveActiveTack_ = false;
			foreach (var tackPoint in model.tackPoints_)
			{
				tackPoint.SetDeactive();
			}
		}

		public List<TackPoint> TacksByIds(List<string> tackIds)
		{
			var results = new List<TackPoint>();
			foreach (var tackPoint in model.tackPoints_)
			{
				if (tackIds.Contains(tackPoint.GetTackId()))
				{
					results.Add(tackPoint);
				}
			}
			return results;
		}

		public void DeleteTackById(string tackId, bool isCancel)
		{
			var deletedTackIndex = model.tackPoints_.FindIndex(tack => tack.GetTackId() == tackId);
			if (deletedTackIndex == -1)
			{
				return;
			}
			else {}
			model.tackPoints_[deletedTackIndex].Deleted(isCancel);
		}

		public List<TackPoint> TacksByStart(int startPos)
		{
			var startIndex = model.tackPoints_.FindIndex(tack => startPos <= tack.GetStart());
			if (0 <= startIndex)
			{
				// if index - 1 tack contains startPos, return it.
				if (0 < startIndex && (startPos <= model.tackPoints_[startIndex - 1].GetStart() + model.tackPoints_[startIndex - 1].GetSpan() - 1))
				{
					return new List<TackPoint> { model.tackPoints_[startIndex - 1] };
				}
				return new List<TackPoint> { model.tackPoints_[startIndex] };
			}

			// no candidate found in area, but if any tack exists, select the last of it. 
			if (model.tackPoints_.Any())
			{
				return new List<TackPoint> { model.tackPoints_[model.tackPoints_.Count - 1] };
			}
			return new List<TackPoint>();
		}

		public List<TackPoint> GetSelectedFrameAndPrev(int selectedFrame)
		{
			List<TackPoint> res = new List<TackPoint>();
			var selectedTack = model.tackPoints_
				.Where(t => t.GetIsExistTack())
				.Where(t => (t.GetStart() <= selectedFrame))
				.Where(t => ((t.GetStart() + t.GetSpan()) > selectedFrame))
				.OrderBy(t => t.GetStart())
				.FirstOrDefault();

			if (selectedTack == null)return res;

			res.Add(selectedTack);
			var nextTack = model.tackPoints_
				.Where(t => t.GetIsExistTack())
				.Where(t => t.GetStart() < selectedTack.GetStart())
				.OrderBy(t => t.GetStart())
				.LastOrDefault();

			if (nextTack != null)res.Add(nextTack);

			return res;
		}

		public TackPoint GetLatestTransform(int selectedFrame)
		{
			var res = model.tackPoints_
				.Where(t => t.GetIsExistTack())
				.Where(t => (t.GetStart() <= selectedFrame))
				.OrderBy(t => t.GetStart())
				.LastOrDefault();

			return res;
		}

		public List<TackPoint> GetUntilSelectedFrame(int selectedFrame)
		{
			List<TackPoint> res = new List<TackPoint>();
			var untilTacks = model.tackPoints_
				.Where(t => t.GetIsExistTack())
				.Where(t => (t.GetStart() <= selectedFrame))
				//.Where(t => ((t.GetStart() + t.GetSpan()) > selectedFrame))
				.OrderBy(t => t.GetStart());

			if (untilTacks == null)return res;

			foreach (var item in untilTacks)res.Add(item);

			return res;
		}

		public TackPoint GetSelectedFrame(int selectedFrame)
		{
			var res = model.tackPoints_
				.Where(t => t.GetIsExistTack())
				.Where(t => (t.GetStart() <= selectedFrame))
				.Where(t => ((t.GetStart() + t.GetSpan()) > selectedFrame))
				.FirstOrDefault();

			return res;
		}

		public TackPoint NewTackToEmptyFrame(int frame, int timelineType)
		{
			var newTackPoint = new TackPoint(
				model.tackPoints_.Count,
				frame,
				WindowSettings.DEFAULT_TACK_SPAN,
				timelineType
			);

			return newTackPoint;
		}

		public void AddNewTackToEmptyFrame(int frame, TackPoint newTackPoint)
		{
			model.tackPoints_.Add(newTackPoint);
			view.ApplyTextureToTacks(model.index_);
		}

		public void PasteTackToEmptyFrame(int frame, TackPoint clipTack)
		{
			var newTackPoint = new TackPoint(
				model.tackPoints_.Count,
				frame,
				clipTack
			);
			model.tackPoints_.Add(newTackPoint);

			view.ApplyTextureToTacks(model.index_);
		}

		public void UpdateByTackMoved(string tackId)
		{
			viewModel.movingTackIds_.Clear();

			var movedTack = TacksByIds(new List<string> { tackId })[0];

			movedTack.ApplyDataToInspector();

			foreach (var targetTack in model.tackPoints_)
			{

				if (targetTack.GetTackId() == tackId)continue; //�������g
				if (!targetTack.GetIsExistTack())continue; //�����^�b�N

				// not contained case.
				if (targetTack.GetStart() + (targetTack.GetSpan() - 1) < movedTack.GetStart())continue; //���S�ɑO��

				if ((movedTack.GetStart() > targetTack.GetStart())
					&& (movedTack.GetStart() < (targetTack.GetStart() + targetTack.GetSpan())))
				{

					string id = MethodBase.GetCurrentMethod().Name;

					var newStart = targetTack.GetStart() + targetTack.GetSpan();
					var lastStart = movedTack.GetStart();
					var newSpan = movedTack.GetSpan();

					Action action = () =>
					{
						movedTack.UpdatePos(newStart, newSpan);
					};

					Action undo = () =>
					{
						movedTack.UpdatePos(lastStart, newSpan);
					};

					ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id, action, undo));
					//Debug.Log("movedTackaaaa");
					break;
				}
				else
				{
					//Debug.Log("movedTackbbbb");
				}
			}
			string id2 = MethodBase.GetCurrentMethod().Name;

			List<Action> cmdDo = new List<Action>();
			List<Action> cmdUndo = new List<Action>();

			if (movedTack.GetStart() < movedTack.GetLastStart())
			{
				foreach (var targetTack in model.tackPoints_)
				{

					if (targetTack.GetTackId() == tackId)continue;
					if (!targetTack.GetIsExistTack())continue;
					if (targetTack.GetStart() + (targetTack.GetSpan() - 1) < movedTack.GetStart())continue; //���S�ɑO��

					if ((targetTack.GetStart() < movedTack.GetLastStart())
						&& (movedTack.GetStart() <= targetTack.GetStart()))
					{
						var tag = targetTack;
						var newStart = targetTack.GetStart() + movedTack.GetSpan();
						var lastStart = targetTack.GetStart();
						var newSpan = targetTack.GetSpan();

						//Debug.Log("bbbb");
						cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
						cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));

						continue;
					}
					else
					{
						//Debug.Log("gggg");
					}
				}
			}
			else if (movedTack.GetStart() > movedTack.GetLastStart())
			{
				foreach (var targetTack in model.tackPoints_)
				{

					if (targetTack.GetTackId() == tackId)continue;
					if (!targetTack.GetIsExistTack())continue;

					if (movedTack.GetLastStart() < targetTack.GetStart())
					{
						if (targetTack.GetStart() + (targetTack.GetSpan() - 1) < movedTack.GetStart())
						{
							var tag = targetTack;
							var newStart = targetTack.GetStart() - movedTack.GetSpan();
							var lastStart = targetTack.GetStart();
							var newSpan = targetTack.GetSpan();
							//Debug.Log("cccc");
							cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
							cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));
							continue;
						}
						else if (movedTack.GetStart() <= targetTack.GetStart())
						{
							//Debug.Log("dddd");
							var tag = targetTack;
							var newStart = targetTack.GetStart() + (movedTack.GetStart() - movedTack.GetLastStart());
							var lastStart = targetTack.GetStart();
							var newSpan = targetTack.GetSpan();
							cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
							cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));

							continue;
						}
						else
						{
							//Debug.Log("asdf");
						}
					}
				}
			}
			else
			{
				foreach (var targetTack in model.tackPoints_)
				{

					if (targetTack.GetTackId() == tackId)continue;
					if (!targetTack.GetIsExistTack())continue;

					if (movedTack.GetLastStart() < targetTack.GetStart())
					{

						var tag = targetTack;
						var newStart = targetTack.GetStart() + (movedTack.GetSpan() - movedTack.GetLastSpan());
						var lastStart = targetTack.GetStart();
						var newSpan = targetTack.GetSpan();
						cmdDo.Add(() => tag.UpdatePos(newStart, newSpan));
						cmdUndo.Add(() => tag.UpdatePos(lastStart, newSpan));

						continue;
					}
					else
					{
						//Debug.Log("ffff");
					}
				}
			}

			if (cmdDo.Any())
			{
				ARIMotionMainWindow.scoreCmd_.Do(new MotionCommand(id2,
					() => { foreach (var cmd in cmdDo)cmd(); },
					() => { foreach (var cmd in cmdUndo)cmd(); }));
			}

			SqueezeTack();
		}

		public void SqueezeTack()
		{

			if ((TimelineType)model.timelineType_ == TimelineType.TL_POS)
			{
				var sortedTacks = model.tackPoints_.OrderBy(t => t.GetStart());
				int nextStart = 0;
				foreach (var targetTack in sortedTacks)
				{
					if (!targetTack.GetIsExistTack())continue;
					targetTack.UpdatePos(nextStart, targetTack.GetSpan());
					nextStart = (targetTack.GetStart() + targetTack.GetSpan());
				}
			}
		}

	}
}