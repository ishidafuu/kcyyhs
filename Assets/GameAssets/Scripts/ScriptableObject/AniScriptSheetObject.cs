using System;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
	[Serializable]
	public class AniScriptSheetObject : ScriptableObject
	{
		//各モーションごとのアニメーション情報
		public List<AniScript> scripts;
	}
}