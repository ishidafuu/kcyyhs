using Unity.Entities;
using UnityEngine;
namespace YYHS
{
	/// <summary>
	/// キャラの向き
	/// </summary>
	public struct CharaDash : IComponentData
	{
		public EnumMuki dashMuki;
	}
}