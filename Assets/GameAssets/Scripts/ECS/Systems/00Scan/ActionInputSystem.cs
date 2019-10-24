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
                ComponentType.ReadWrite<JumpState>(),
                ComponentType.ReadWrite<DamageState>(),
                ComponentType.ReadWrite<ReiState>()
            );
        }

        protected override void OnUpdate()
        {
            NativeArray<PadScan> padScans = m_queryChara.ToComponentDataArray<PadScan>(Allocator.TempJob);
            NativeArray<SideInfo> sideInfos = m_queryChara.ToComponentDataArray<SideInfo>(Allocator.TempJob);
            NativeArray<ToukiMeter> toukiMeters = m_queryChara.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            NativeArray<JumpState> jumpStates = m_queryChara.ToComponentDataArray<JumpState>(Allocator.TempJob);
            NativeArray<DamageState> damageStates = m_queryChara.ToComponentDataArray<DamageState>(Allocator.TempJob);
            NativeArray<ReiState> reiStates = m_queryChara.ToComponentDataArray<ReiState>(Allocator.TempJob);

            var seq = GetSingleton<BattleSequencer>();

            bool isJumpUpdate = false;
            bool isReiDamageUpdate = false;

            for (int i = 0; i < padScans.Length; i++)
            {
                var padScan = padScans[i];
                var toukiMeter = toukiMeters[i];
                var sideInfo = sideInfos[i];
                var jumpState = jumpStates[i];
                var reiState = reiStates[i];

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
                    YHActionData attackData = Shared.m_yhCharaAttackList.GetData(sideInfo.m_charaNo, toukiMeter.m_cross, padScan.GetPressButton());
                    actionType = GetActionType(attackData);
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

                    JudgeDamageLv(ref sideInfo, ref seq, isIdleSeq);

                    UpdateReiAmount(ref reiState, ref sideInfo, actionNo);
                    reiStates[i] = reiState;
                    isReiDamageUpdate = true;
                }

                SetSingleton<BattleSequencer>(seq);
            }

            if (isReiDamageUpdate)
            {
                UpdateDamage(damageStates, ref seq);

                m_queryChara.CopyFromComponentDataArray(reiStates);
            }

            if (isJumpUpdate)
            {
                m_queryChara.CopyFromComponentDataArray(jumpStates);
            }

            padScans.Dispose();
            sideInfos.Dispose();
            toukiMeters.Dispose();
            jumpStates.Dispose();
            damageStates.Dispose();
            reiStates.Dispose();
        }

        private static EnumActionType GetActionType(YHActionData attackData)
        {
            EnumActionType actionType = EnumActionType.None;
            switch (attackData.rangeType)
            {
                case EnumAttackRangeType.Short:
                    actionType = EnumActionType.ShortAttack;
                    break;
                case EnumAttackRangeType.Middle:
                    actionType = EnumActionType.MiddleAttack;
                    break;
                case EnumAttackRangeType.Long:
                    actionType = EnumActionType.LongAttack;
                    break;
                case EnumAttackRangeType.Ground:
                    actionType = EnumActionType.GroundAttack;
                    break;
                case EnumAttackRangeType.Wave:
                    actionType = EnumActionType.WaveAttack;
                    break;
            }

            return actionType;
        }

        private static void UpdateReiAmount(ref ReiState reiState, ref SideInfo sideInfo, int actionNo)
        {
            YHActionData attackData = Shared.m_yhCharaAttackList.GetData(sideInfo.m_charaNo, actionNo);
            reiState.m_reiAmount = attackData.cost;
        }

        private void UpdateDamage(NativeArray<DamageState> damageStates, ref BattleSequencer seq)
        {
            for (int i = 0; i < damageStates.Length; i++)
            {
                var damageState = damageStates[i];
                bool isAttackSideA = (i != 0);
                if (isAttackSideA)
                {
                    SetDamage(ref seq.m_sideA, ref damageState);
                }
                else
                {
                    SetDamage(ref seq.m_sideB, ref damageState);
                }
                damageStates[i] = damageState;
            }
            m_queryChara.CopyFromComponentDataArray(damageStates);
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

        private static void SetDamage(ref SideState attackSideState, ref DamageState damageState)
        {

            YHActionData attackData = Shared.m_yhCharaAttackList.GetData(attackSideState.m_charaNo, attackSideState.m_actionNo);
            int damage = attackData.power;
            int balance = attackData.balance;

            switch (attackSideState.m_enemyDamageLv)
            {
                case EnumDamageLv.CleanHit:
                    break;
                case EnumDamageLv.Hit:
                    damage = (int)(damage * 0.7f);
                    balance = (int)(balance * 0.7f);
                    break;
                case EnumDamageLv.Tip:
                    damage = (int)(damage * 0.2f);
                    balance = (int)(balance * 0.2f);
                    break;
                case EnumDamageLv.NoDamage:
                    damage = 0;
                    balance = 0;
                    break;
            }

            damageState.m_lifeDamage = damage;
            damageState.m_balanceDamage = balance;
        }

        private static void JudgeDamageLv(ref SideInfo attackSideInfo, ref BattleSequencer seq, bool isStartAnim)
        {


            // TODO:確率算出
            int sideADamage = UnityEngine.Random.Range(0, 3);
            int sideBDamage = UnityEngine.Random.Range(0, 3);
            seq.m_sideA.m_enemyDamageLv = (EnumDamageLv)sideADamage;
            seq.m_sideB.m_enemyDamageLv = (EnumDamageLv)sideBDamage;
            // TODO:仮
            seq.m_sideA.m_enemyDamageLv = EnumDamageLv.Hit;
            seq.m_sideB.m_enemyDamageLv = EnumDamageLv.Hit;

            if (isStartAnim)
            {
                if (attackSideInfo.m_isSideA)
                {
                    seq.m_sideB.m_enemyDamageLv = EnumDamageLv.NoDamage;
                }
                else
                {
                    seq.m_sideA.m_enemyDamageLv = EnumDamageLv.NoDamage;
                }
            }


            // TODO:バランス値で変化させる
            seq.m_sideA.m_enemyDamageReaction = EnumDamageReaction.None;
            seq.m_sideB.m_enemyDamageReaction = EnumDamageReaction.None;
            // seq.m_sideA.m_enemyDamageReaction = EnumDamageReaction.Fly;
            // seq.m_sideB.m_enemyDamageReaction = EnumDamageReaction.Shaky;


            // Debug.Log(battleSequencer.m_sideA.m_enemyDamageLv);
            // Debug.Log(battleSequencer.m_sideB.m_enemyDamageLv);
        }

        private static void InitSequencer(ref BattleSequencer battleSequencer, bool isSideA)
        {
            battleSequencer.m_seqState = EnumBattleSequenceState.Start;
            battleSequencer.m_isLastSideA = isSideA;
            battleSequencer.m_isDistributeRei = false;

            InitSideState(ref battleSequencer.m_sideA);
            InitSideState(ref battleSequencer.m_sideB);
        }

        private static void InitSideState(ref SideState sideState)
        {
            sideState.m_actionType = EnumActionType.None;
            sideState.m_animStep = EnumAnimationStep.Sleep;
            sideState.m_isDefenceFinished = false;
            sideState.m_isStartDamage = false;
            sideState.m_isConsumeRei = false;
        }

        private static void InitActionSide(SideInfo attackSideInfo, ref SideState attackSideState,
            int actionNo, EnumActionType actionType, EnumDefenceType defenceType, bool isNeedDefence)
        {
            attackSideState.m_isSideA = attackSideInfo.m_isSideA;
            attackSideState.m_charaNo = attackSideInfo.m_charaNo;
            attackSideState.m_actionNo = actionNo;
            attackSideState.m_actionType = actionType;
            attackSideState.m_enemyDeffenceType = defenceType;
            attackSideState.m_isEnemyNeedDefence = isNeedDefence;
            attackSideState.m_animStep = EnumAnimationStep.WaitPageA;
            attackSideState.m_isDefenceFinished = false;
            attackSideState.m_isStartDamage = false;
            attackSideState.m_isConsumeRei = false;
        }

    }
}
