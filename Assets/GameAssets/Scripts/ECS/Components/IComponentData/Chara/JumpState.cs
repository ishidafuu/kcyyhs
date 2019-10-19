using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct JumpState : IComponentData
    {
        public EnumJumpState m_state;
        public EnumJumpEffectStep m_effectStep;
        public int m_totalCount;
        public int m_animationCount;
        public int m_stepCount;
        public int m_charge;
    }
}