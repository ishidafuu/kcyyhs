using Unity.Entities;
using UnityEngine;
namespace YYHS
{
	public struct BgScroll : IComponentData
	{
		public EnumCrossType muki;
		public int value;
	}
}