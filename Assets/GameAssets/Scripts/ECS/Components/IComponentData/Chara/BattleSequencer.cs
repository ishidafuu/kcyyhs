using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct BattleSequencer : IComponentData
    {
        public bool isPlay;
        public bool isTransition;
        public int sequenceStep;
        public bool isLastSideA;
        public SideState sideA;
        public SideState sideB;

        public BattleAnim animation;
    }

    public struct SideState
    {
        public bool isSideA;
        public int charaNo;
        public EnumActionType actionType;
        public int actionNo;
        public EnumAnimationStep animStep;
        public EnumDefenceType enemyDeffenceType;
        public EnumDamageLv enemyDamageLv;
    }

    public struct BattleAnim
    {
        public int charaNo;
        public EnumAnimationName animName;
        public int count;
        public bool isSideA;
    }
}