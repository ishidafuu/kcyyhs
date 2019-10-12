using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct FilterEffect : IComponentData
    {
        public bool m_isActive;
        public bool m_isSideA;
        public int m_effectIndex;
        public int m_count;
        public EnumEffectType m_effectType;
    }
}