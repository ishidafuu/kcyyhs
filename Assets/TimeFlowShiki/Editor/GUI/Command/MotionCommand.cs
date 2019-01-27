using System;

namespace NKKD.EDIT
{
	public class MotionCommand
	{
		string id;
		Action doAction;
		Action redoAction;
		Action undoAction;

		//public MotionCommand(string id, Action doAction, Action undoAction, Action redoAction)
		//{
		//	this.id = id;
		//	this.doAction = doAction;
		//	this.undoAction = undoAction;
		//	this.redoAction = redoAction;
		//}

		public MotionCommand(string id, Action doAction, Action undoAction)
		{
			this.id = id;
			this.doAction = doAction;
			this.undoAction = undoAction;
			this.redoAction = doAction;
		}

		public MotionCommand(Action doAction, Action undoAction)
		{
			this.id = "";
			this.doAction = doAction;
			this.undoAction = undoAction;
			this.redoAction = doAction;
		}
		public bool IsId() { return (id != ""); }
		public string GetId() { return id; }
		public void Do() { doAction(); }
		public void Undo() { undoAction(); }
		public void Redo() { redoAction(); }
	}
}