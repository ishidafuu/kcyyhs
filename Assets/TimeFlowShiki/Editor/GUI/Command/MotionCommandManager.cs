using System.Collections.Generic;
using UnityEngine;

namespace NKKD.EDIT
{
	public class MotionCommandManager
	{
		readonly int STACKMAX = 512;
		List<MotionCommand> undoStack = new List<MotionCommand>();
		List<MotionCommand> redoStack = new List<MotionCommand>();
		bool isSameIdThrough_;//trueの場合、同じアクションをUndoスタックに積まない

		//
		public MotionCommandManager(bool isSameIdThrough)
		{
			isSameIdThrough_ = isSameIdThrough;
		}

		public void Do(MotionCommand command)
		{
			//Debug.Log(command.GetId());
			command.Do();

			if ((undoStack.Count == 0) 
				|| !isSameIdThrough_
				|| !command.IsId()
				|| (undoStack.Peek().GetId() != command.GetId()))
			{
				undoStack.Push(command);
				if (undoStack.Count >= STACKMAX) undoStack.RemoveAt(undoStack.Count - 1);
			}

			redoStack.Clear();
		}

		public void Clear()
		{
			//Debug.Log("UndoClear");
			undoStack.Clear();
			redoStack.Clear();
		}

		public void Undo()
		{
			if (undoStack.Count == 0) return;

			var command = undoStack.Pop();
			command.Undo();
			Debug.Log("Undo " + command.GetId());
			redoStack.Push(command);
		}

		public void Redo()
		{
			if (redoStack.Count == 0) return;

			var command = redoStack.Pop();
			command.Redo();
			Debug.Log("Redo " + command.GetId());
			undoStack.Push(command);
		}

		public bool CanUndo()
		{
			return undoStack.Count > 0;
		}

		public bool CanRedo()
		{
			return redoStack.Count > 0;
		}
	}
}