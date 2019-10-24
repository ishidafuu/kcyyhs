using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct ReiPiece : IComponentData
    {
        public int m_id;
        public EnumReiState m_reiState;
        public bool m_isSideA;
        public int m_count;
        public int m_offset;
        public float m_speed;
        public float m_width;
        public Vector2Int m_basePos;
        public Vector2Int m_movePos;
    }
}