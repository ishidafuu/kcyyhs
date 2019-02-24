using Unity.Entities;
using UnityEngine;
namespace YYHS
{
	public struct ToukiMeter : IComponentData
	{
		public EnumCrossType muki;
		public int value;
		public EnumToukiMaterState state;

		// public void UpdateMuki(EnumCrossType muki)
		// {
		// 	if (this.muki != muki)
		// 	{
		// 		this.value = 0;
		// 		this.muki = muki;
		// 	}
		// }
	}
}