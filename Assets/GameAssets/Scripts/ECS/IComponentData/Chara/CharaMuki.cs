using Unity.Entities;
using UnityEngine;
namespace NKKD
{
	/// <summary>
	/// キャラの向き
	/// </summary>
	public struct CharaMuki : IComponentData
	{
		public EnumMuki muki;
	}
}