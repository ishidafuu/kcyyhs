using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Reflection;
using System.Collections.Generic;
using System;

namespace NKKD.EDIT
{
	[Serializable]
	public class MotionScores : ScriptableObject
	{
		public List<MotionScore> scores = new List<MotionScore>();
	}
}