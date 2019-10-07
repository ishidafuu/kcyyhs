using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct ReiPool : IComponentData
    {
        public EnumReiState m_reiState;
        public int m_amount;
        public int m_count;
    }
}