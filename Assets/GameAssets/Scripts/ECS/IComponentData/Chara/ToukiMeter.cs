using Unity.Entities;
using UnityEngine;
namespace YYHS
{
	public struct ToukiMeter : IComponentData
	{
		public EnumCrossType muki;
		public int value;
		public EnumToukiMaterState state;
	}
}