using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct ToukiMeter : IComponentData
    {
        public EnumCrossType m_cross;
        public int m_value;
        public int m_bgScroll;
        public float m_bgScrollTextureUL;
        public float m_bgScrollTextureUR;
        public int m_animationCount;
        public bool m_isDecided;
    }
}