using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct BattleSequencer : IComponentData
    {
        public bool m_isPlay;
        public bool m_isTransition;
        public int m_sequenceStep;
        public bool m_isLastSideA;
        public EnumAnimType m_animType;
        public SideState m_sideA;
        public SideState m_sideB;

        public BattleAnim m_animation;
    }

    public struct SideState
    {
        public bool m_isSideA;
        public int m_charaNo;
        public EnumActionType m_actionType;
        public int m_actionNo;
        public EnumDefenceType m_enemyDeffenceType;
        public EnumDamageLv m_enemyDamageLv;
        public EnumDamageReaction m_enemyDamageReaction;
        public bool m_isNeedDefence;
        public EnumAnimationStep m_animStep;
        public bool m_isEndDefence;
    }

    public struct BattleAnim
    {
        public int m_charaNo;
        public EnumAnimationName m_animName;
        public int m_count;
        public bool m_isSideA;
    }
}