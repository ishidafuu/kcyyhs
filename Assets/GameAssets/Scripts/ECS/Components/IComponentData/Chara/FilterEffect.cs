using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct FilterEffect : IComponentData
    {
        public bool m_isActive;
        public int m_effectIndex;
        public int m_count;
    }
}