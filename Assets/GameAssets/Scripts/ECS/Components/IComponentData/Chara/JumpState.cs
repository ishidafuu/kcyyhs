using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct JumpState : IComponentData
    {
        public EnumJumpState m_state;
        public EnumJumpEffectStep m_effectStep;
        public int m_count;
        public int m_stepCount;
    }
}