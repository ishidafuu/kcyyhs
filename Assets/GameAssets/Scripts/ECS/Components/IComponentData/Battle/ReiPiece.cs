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
    }
}