using UnityEngine;
using System;
using System.Collections;

namespace NKKD.EDIT
{
	public struct MotionColorState
	{
		public bool isActive;
		public int frame;
		public int startFrame;
		public MotionColor data;

		public void Inportmotion(MotionColor motionColor, int frame, int startFrame)
		{
			this.isActive = true;
			this.frame = frame;
			this.startFrame = startFrame;
			this.data = motionColor;
		}

		public void Inportmotion(MotionColor motionColor, int frame)
		{
			this.isActive = true;
			this.frame = frame;
			//this.startFrame = startFrame;
			this.data = motionColor;
		}
	}

}
