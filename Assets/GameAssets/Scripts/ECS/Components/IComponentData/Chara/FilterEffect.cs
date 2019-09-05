using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct FilterEffect : IComponentData
    {
        public int m_id;
        public bool m_isActive;
        public int m_effectNo;
        public int m_count;
    }
}