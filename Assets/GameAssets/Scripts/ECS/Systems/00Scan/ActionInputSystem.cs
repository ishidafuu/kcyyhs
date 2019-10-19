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
    [UpdateAfter(typeof(ToukiMeterInputSystem))]
    public class ActionInputSystem : ComponentSystem
    {
        EntityQuery m_queryChara;
        EntityQuery m_queryBattle;

        protected override void OnCreate()
        {
            m_queryChara = GetEntityQuery(
                ComponentType.ReadOnly<PadScan>(),
                ComponentType.ReadOnly<SideInfo>(),
                ComponentType.ReadOnly<ToukiMeter>(),
                ComponentType.ReadOnly<JumpState>()
            );
        }

        protected override void OnUpdate()
        {
            NativeArray<PadScan> padScans = m_queryChara.ToComponentDataArray<PadScan>(Allocator.TempJob);
            NativeArray<SideInfo> sideInfos = m_queryChara.ToComponentDataArray<SideInfo>(Allocator.TempJob);
            NativeArray<ToukiMeter> toukiMeters = m_queryChara.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            NativeArray<JumpState> jumpStates = m_queryChara.ToComponentDataArray<JumpState>(Allocator.TempJob);

            var seq = GetSingleton<BattleSequencer>();

            bool isJumpUpdate = false;

            for (int i = 0; i < padScans.Length; i++)
            {
                var padScan = padScans[i];
                var toukiMeter = toukiMeters[i];
                var sideInfo = sideInfos[i];
                var jumpState = jumpStates[i];
                bool isSideA = i == 0;

                if (seq.m_seqState >= EnumBattleSequenceState.Start)
                {
                    if (isSideA)
                    {
                        if (seq.m_sideA.m_actionType != EnumActionType.None)
                            continue;
                    }
                    else
                    {
                        if (seq.m_sideB.m_actionType != EnumActionType.None)
                            continue;
                    }
                }


                if (padScan.GetPressButton() == EnumButtonType.None)
                    continue;

                // TODO:アイテム使用が入る場合、０でも発動する
                // TODO:一時的に０でも技出るように
                if (toukiMeter.m_value == 0)
                    continue;

                EnumActionType actionType = EnumActionType.ShortAttack;
                int actionNo = 0;
                // 霊力アップなどの相手にディフェンスステップが発生しない場合はfalseにする
                bool isNeedDefence = true;

                // ジャンプ
                if (toukiMeter.m_cross == EnumCrossType.Up
                    && padScan.GetPressButton() == EnumButtonType.X)
                {
                    if (jumpState.m_state != EnumJumpState.None)
                        continue;

                    actionType = EnumActionType.Jump;
                }
                else
                {
                    // TODO:技情報から引いてくる
                    switch (padScan.GetPressButton())
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
                }



                bool isIdleSeq = seq.m_seqState == EnumBattleSequenceState.Idle;

                // アニメーション未開始＆ジャンプ以外の時はアニメに入らない
                bool isNoneAnim = isIdleSeq
                    && (actionType == EnumActionType.Jump);

                if (isNoneAnim)
                {
                    StartJump(ref jumpState, toukiMeter.m_value);
                    jumpStates[i] = jumpState;
                    isJumpUpdate = true;
                }
                else
                {
                    // TODO:仮
                    isNeedDefence = true;
                    actionNo = 0;
                    actionType = (isSideA)
                        ? EnumActionType.LongAttack
                        : EnumActionType.ShortAttack;

                    if (isIdleSeq)
                    {
                        InitSequencer(ref seq, sideInfo.m_isSideA);
                    }

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
                    // TODO:仮
                    defenceType = EnumDefenceType.Stand;

                    if (sideInfo.m_isSideA)
                    {
                        InitActionSide(sideInfo, ref seq.m_sideA, actionNo, actionType,
                            defenceType, isNeedDefence);
                    }
                    else
                    {
                        InitActionSide(sideInfo, ref seq.m_sideB, actionNo, actionType,
                            defenceType, isNeedDefence);
                    }

                    JudgeDamage(ref sideInfo, ref seq, i, isIdleSeq);
                }

                SetSingleton<BattleSequencer>(seq);
            }


            if (isJumpUpdate)
            {
                m_queryChara.CopyFromComponentDataArray(jumpStates);
            }

            padScans.Dispose();
            sideInfos.Dispose();
            toukiMeters.Dispose();
            jumpStates.Dispose();
        }

        private void StartJump(ref JumpState jumpState, int charge)
        {
            jumpState.m_state = EnumJumpState.Jumping;
            jumpState.m_charge = charge;
            jumpState.m_effectStep = EnumJumpEffectStep.JumpStart;
            jumpState.m_totalCount = 0;
            jumpState.m_animationCount = 0;
            jumpState.m_stepCount = 0;
        }

        private static void JudgeDamage(ref SideInfo sideInfo, ref BattleSequencer battleSequencer, int i, bool isStartAnim)
        {
            if (isStartAnim)
            {
                if (sideInfo.m_isSideA)
                {
                    battleSequencer.m_sideA.m_enemyDamageLv = EnumDamageLv.NoDamage;
                }
                else
                {
                    battleSequencer.m_sideA.m_enemyDamageLv = EnumDamageLv.NoDamage;
                }
            }
            else
            {
                // TODO:確率算出
                int sideADamage = UnityEngine.Random.Range(0, 3);
                int sideBDamage = UnityEngine.Random.Range(0, 3);
                battleSequencer.m_sideA.m_enemyDamageLv = (EnumDamageLv)sideADamage;
                battleSequencer.m_sideB.m_enemyDamageLv = (EnumDamageLv)sideBDamage;
                // TODO:バランス値で変化させる
                battleSequencer.m_sideA.m_enemyDamageReaction = EnumDamageReaction.None;
                battleSequencer.m_sideB.m_enemyDamageReaction = EnumDamageReaction.None;
            }

            // TODO:仮
            battleSequencer.m_sideA.m_enemyDamageLv = EnumDamageLv.Hit;
            battleSequencer.m_sideB.m_enemyDamageLv = EnumDamageLv.Hit;
            // Debug.Log(battleSequencer.m_sideA.m_enemyDamageLv);
            // Debug.Log(battleSequencer.m_sideB.m_enemyDamageLv);
        }

        private static void InitSequencer(ref BattleSequencer battleSequencer, bool isSideA)
        {
            battleSequencer.m_seqState = EnumBattleSequenceState.Start;
            battleSequencer.m_isLastSideA = isSideA;

            battleSequencer.m_sideA.m_actionType = EnumActionType.None;
            battleSequencer.m_sideA.m_animStep = EnumAnimationStep.Sleep;
            battleSequencer.m_sideA.m_isDefenceFinished = false;

            battleSequencer.m_sideB.m_actionType = EnumActionType.None;
            battleSequencer.m_sideB.m_animStep = EnumAnimationStep.Sleep;
            battleSequencer.m_sideB.m_isDefenceFinished = false;
        }

        private static void InitActionSide(SideInfo sideInfo, ref SideState sideState,
            int actionNo, EnumActionType actionType, EnumDefenceType defenceType, bool isNeedDefence)
        {
            sideState.m_isSideA = sideInfo.m_isSideA;
            sideState.m_charaNo = sideInfo.m_charaNo;
            sideState.m_actionNo = actionNo;
            sideState.m_actionType = actionType;
            sideState.m_enemyDeffenceType = defenceType;
            sideState.m_isEnemyNeedDefence = isNeedDefence;
            sideState.m_animStep = EnumAnimationStep.WaitPageA;
            sideState.m_isDefenceFinished = false;
        }

    }
}
