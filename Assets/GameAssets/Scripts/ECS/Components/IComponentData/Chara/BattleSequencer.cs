using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct BattleSequencer : IComponentData
    {
        public bool isPlay;
        public int sequenceStep;
        public bool isLastSideA;
        public SideState sideA;
        public SideState sideB;

        public BattleAnim animation;
    }

    public struct SideState
    {
        public bool isSideA;
        public EnumActionType actionType;
        public int charaNo;
        public int actionNo;
        public EnumAnimationStep animStep;
        public int enemyDeffenceType;
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