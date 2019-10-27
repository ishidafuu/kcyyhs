using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct BattleSequencer : IComponentData
    {
        public EnumBattleSequenceState m_seqState;
        public EnumAnimType m_animType;
        public bool m_isLastSideA;
        public bool m_isDistributeRei;

        // TODO:structが大きくなっているので分割する
        public BattleAnim m_animation;
        public SideState m_sideA;
        public SideState m_sideB;
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
        public bool m_isEnemyNeedDefence;
        public EnumAnimationStep m_animStep;
        public bool m_isDefenceFinished;
        public bool m_isStartDamage;
        public bool m_isConsumeRei;
        public bool m_isReverse;
    }

    public struct BattleAnim
    {
        public bool m_isSideA;
        public int m_charaNo;
        public EnumAnimationName m_animName;
        public int m_count;
    }
}