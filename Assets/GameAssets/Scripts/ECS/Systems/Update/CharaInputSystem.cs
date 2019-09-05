using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


namespace YYHS
{
    [UpdateInGroup(typeof(ScanGroup))]
    public class CharaInputSystem : ComponentSystem
    {
        EntityQuery m_queryChara;
        EntityQuery m_queryBattle;

        protected override void OnCreate()
        {
            m_queryChara = GetEntityQuery(
                ComponentType.ReadOnly<PadScan>(),
                ComponentType.ReadOnly<SideInfo>(),
                ComponentType.ReadOnly<ToukiMeter>()
            );
        }

        protected override void OnUpdate()
        {
            NativeArray<PadScan> padScans = m_queryChara.ToComponentDataArray<PadScan>(Allocator.TempJob);
            NativeArray<SideInfo> sideInfos = m_queryChara.ToComponentDataArray<SideInfo>(Allocator.TempJob);
            NativeArray<ToukiMeter> toukiMeters = m_queryChara.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var seq = GetSingleton<BattleSequencer>();

            for (int i = 0; i < padScans.Length; i++)
            {
                var toukiMeter = toukiMeters[i];

                if (seq.isPlay)
                {
                    if (i == 0)
                    {
                        if (seq.sideA.actionType != EnumActionType.None)
                            break;
                    }
                    else
                    {
                        if (seq.sideB.actionType != EnumActionType.None)
                            break;
                    }
                }


                if (padScans[i].GetPressButton() == EnumButtonType.None)
                    break;

                // TODO:アイテム使用が入る場合、０でも発動する
                if (toukiMeter.value == 0)
                    break;

                bool isStartAnim = !seq.isPlay;

                if (isStartAnim)
                {
                    InitSequencer(ref seq, sideInfos[i].isSideA);
                }


                EnumActionType actionType = EnumActionType.ShortAttack;
                int actionNo = 0;
                // 霊力アップなどの相手にディフェンスステップが発生しない場合はfalseにする
                bool isNeedDefence = true;

                // TODO:技情報から引いてくる
                switch (padScans[i].GetPressButton())
                {
                    case EnumButtonType.A:
                        actionType = EnumActionType.MiddleAttack;
                        actionNo = 0;
                        break;
                    case EnumButtonType.B:
                        actionType = EnumActionType.ShortAttack;
                        actionNo = 1;
                        break;
                    case EnumButtonType.X:
                        actionType = EnumActionType.LongAttack;
                        actionNo = 2;
                        break;
                    case EnumButtonType.Y:
                        actionType = EnumActionType.LongAttack;
                        actionNo = 3;
                        break;
                }

                // TODO:仮
                isNeedDefence = false;
                actionNo = 0;
                actionType = EnumActionType.ShortAttack;

                EnumDefenceType defenceType = EnumDefenceType.Stand;
                switch (actionType)
                {
                    case EnumActionType.LongAttack:
                    case EnumActionType.MiddleAttack:
                        defenceType = EnumDefenceType.Fly;
                        break;
                    default:
                        defenceType = EnumDefenceType.Stand;
                        break;
                }

                if (sideInfos[i].isSideA)
                {
                    InitActionSide(sideInfos[i], ref seq.sideA, actionNo, actionType,
                        defenceType, isNeedDefence);
                }
                else
                {
                    InitActionSide(sideInfos[i], ref seq.sideB, actionNo, actionType,
                        defenceType, isNeedDefence);
                }

                JudgeDamage(ref sideInfos, ref seq, i, isStartAnim);
            }

            SetSingleton<BattleSequencer>(seq);

            padScans.Dispose();
            sideInfos.Dispose();
            toukiMeters.Dispose();
        }

        private static void JudgeDamage(ref NativeArray<SideInfo> sideInfos, ref BattleSequencer battleSequencer, int i, bool isStartAnim)
        {
            if (isStartAnim)
            {
                if (sideInfos[i].isSideA)
                {
                    battleSequencer.sideA.enemyDamageLv = EnumDamageLv.Hit;
                }
                else
                {
                    battleSequencer.sideA.enemyDamageLv = EnumDamageLv.Hit;
                }
            }
            else
            {
                // TODO:確率算出
                int sideADamage = UnityEngine.Random.Range(0, 3);
                int sideBDamage = UnityEngine.Random.Range(0, 3);
                battleSequencer.sideA.enemyDamageLv = (EnumDamageLv)sideADamage;
                battleSequencer.sideB.enemyDamageLv = (EnumDamageLv)sideBDamage;
                // TODO:バランス値で変化させる
                battleSequencer.sideA.enemyDamageReaction = EnumDamageReaction.None;
                battleSequencer.sideB.enemyDamageReaction = EnumDamageReaction.None;
            }
        }

        private static void InitSequencer(ref BattleSequencer battleSequencer, bool isSideA)
        {
            battleSequencer.isPlay = true;
            battleSequencer.isTransition = true;
            battleSequencer.sequenceStep = 0;
            battleSequencer.isLastSideA = isSideA;
            battleSequencer.sideA.actionType = EnumActionType.None;
            battleSequencer.sideA.animStep = EnumAnimationStep.Sleep;
            battleSequencer.sideB.actionType = EnumActionType.None;
            battleSequencer.sideA.animStep = EnumAnimationStep.Sleep;
        }

        private static void InitActionSide(SideInfo sideInfo, ref SideState sideState,
            int actionNo, EnumActionType actionType, EnumDefenceType defenceType, bool isNeedDefence)
        {
            sideState.isSideA = sideInfo.isSideA;
            sideState.charaNo = sideInfo.charaNo;
            sideState.actionNo = actionNo;
            sideState.actionType = actionType;
            sideState.enemyDeffenceType = defenceType;
            sideState.isNeedDefence = isNeedDefence;
            sideState.animStep = EnumAnimationStep.Start;
            sideState.isEndDefence = false;
        }

    }
}
