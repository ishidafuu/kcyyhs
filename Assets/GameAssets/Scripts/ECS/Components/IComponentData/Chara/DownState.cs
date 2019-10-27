using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct DownState : IComponentData
    {
        public EnumDownState m_state;
        public int m_count;
    }
}