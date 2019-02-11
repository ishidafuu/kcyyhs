using Unity.Entities;
using UnityEngine;
namespace NKKD
{
	/// <summary>
	/// キャラの向き
	/// </summary>
	public struct CharaDash : IComponentData
	{
		public EnumMuki dashMuki;
	}
}