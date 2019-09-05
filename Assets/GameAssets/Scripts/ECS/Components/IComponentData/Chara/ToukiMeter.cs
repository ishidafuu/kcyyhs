using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct ToukiMeter : IComponentData
    {
        public int charaNo;
        public EnumCrossType muki;
        public int value;
        public int bgScroll;
        public float bgScrollTextureUL;
        public float bgScrollTextureUR;
    }
}