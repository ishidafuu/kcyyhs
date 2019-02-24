using Unity.Entities;
using UnityEngine;
namespace YYHS
{
	/// <summary>
	/// キャラの向き
	/// </summary>
	public struct CharaMuki : IComponentData
	{
		public EnumMuki muki;
	}
}