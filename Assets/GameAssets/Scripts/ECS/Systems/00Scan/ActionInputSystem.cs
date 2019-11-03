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
                ComponentType.ReadWrite<ReiState>(),
                ComponentType.ReadWrite<JumpState>(),
                ComponentType.ReadWrite<DownState>(),
                ComponentType.ReadWrite<DamageState>(),
                ComponentType.ReadOnly<ToukiMeter>(),
                ComponentType.ReadOnly<Status>(),
                ComponentType.ReadOnly<SideInfo>(),
                ComponentType.ReadOnly<PadScan>()
            );
        }

        protected override void OnUpdate()
        {
            NativeArray<ToukiMeter> toukiMeters = m_queryChara.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            NativeArray<Status> statuses = m_queryChara.ToComponentDataArray<Status>(Allocator.TempJob);
            NativeArray<SideInfo> sideInfos = m_queryChara.ToComponentDataArray<SideInfo>(Allocator.TempJob);
            NativeArray<ReiState> reiStates = m_queryChara.ToComponentDataArray<ReiState>(Allocator.TempJob);
            NativeArray<PadScan> padScans = m_queryChara.ToComponentDataArray<PadScan>(Allocator.TempJob);
            NativeArray<JumpState> jumpStates = m_queryChara.ToComponentDataArray<JumpState>(Allocator.TempJob);
            NativeArray<DownState> downStates = m_queryChara.ToComponentDataArray<DownState>(Allocator.TempJob);
            NativeArray<DamageState> damageStates = m_queryChara.ToComponentDataArray<DamageState>(Allocator.TempJob);

            var seq = GetSingleton<BattleSequencer>();

            bool isJumpUpdate = false;
            bool isReiDamageUpdate = false;
            bool isToukiMeterUpdate = false;

            for (int i = 0; i < padScans.Length; i++)
            {
                var toukiMeter = toukiMeters[i];
                var sideInfo = sideInfos[i];
                var reiState = reiStates[i];
                var padScan = padScans[i];
                var jumpState = jumpStates[i];

                bool isSideA = SideUtil.IsSideA(i);

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
                    actionNo = (((int)toukiMeter.m_cross - 1) * 4) + ((int)padScan.GetPressButton() - 1);
                    YHActionData attackData = Shared.m_yhCharaAttackList.GetData(sideInfo.m_charaNo, toukiMeter.m_cross, padScan.GetPressButton());
                    actionType = GetActionType(attackData);
                    // TODO:仮
                    actionType = EnumActionType.ShortAttack;
                    actionNo = 0;
                }


                // 霊力アップなどの相手にディフェンスステップが発生しない場合はfalseにする
                bool isNeedDefence = GetNeedDefence(actionType);

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

                    if (isIdleSeq)
                    {
                        InitSequencer(ref seq, downStates, sideInfo.m_isSideA);
                    }

                    EnumDefenceType defenceType = GetDefenceType(ref jumpStates, i, actionType);

                    bool isChangeDefenceJump = (!isIdleSeq && actionType == EnumActionType.Jump);

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

                    var enemyJumpState = jumpStates[SideUtil.EnemyIndex(i)];
                    UpdateDamageLv(ref seq, ref sideInfo, actionType, enemyJumpState);
                    UpdateDamage(ref seq, damageStates, statuses, jumpStates);
                    UpdateReiAmount(ref reiState, ref sideInfo, actionNo);

                    reiStates[i] = reiState;
                    isReiDamageUpdate = true;
                }

                toukiMeter.m_isDecided = true;
                toukiMeters[i] = toukiMeter;
                isToukiMeterUpdate = true;

                SetSingleton<BattleSequencer>(seq);
            }

            if (isReiDamageUpdate)
            {
                m_queryChara.CopyFromComponentDataArray(reiStates);
                m_queryChara.CopyFromComponentDataArray(damageStates);
            }

            if (isJumpUpdate)
            {
                m_queryChara.CopyFromComponentDataArray(jumpStates);
            }

            if (isToukiMeterUpdate)
            {
                m_queryChara.CopyFromComponentDataArray(toukiMeters);
            }

            toukiMeters.Dispose();
            statuses.Dispose();
            sideInfos.Dispose();
            reiStates.Dispose();
            padScans.Dispose();
            jumpStates.Dispose();
            downStates.Dispose();
            damageStates.Dispose();

        }

        private static bool GetNeedDefence(EnumActionType actionType)
        {
            bool isNeedDefence = false;
            switch (actionType)
            {
                case EnumActionType.ShortAttack:
                case EnumActionType.MiddleAttack:
                case EnumActionType.LongAttack:
                case EnumActionType.WaveAttack:
                case EnumActionType.GroundAttack:
                    isNeedDefence = true;
                    break;
            }

            return isNeedDefence;
        }

        private static BattleSequencer UpdateDamageLv(ref BattleSequencer seq, ref SideInfo sideInfo, EnumActionType actionType, JumpState enemyJumpState)
        {
            bool isEnemyAir = (enemyJumpState.m_state == EnumJumpState.Jumping
                    || enemyJumpState.m_state == EnumJumpState.Air);

            // 空振り
            if (isEnemyAir
                && actionType == EnumActionType.ShortAttack)
            {
                if (sideInfo.m_isSideA)
                {
                    seq.m_sideA.m_enemyDamageLv = EnumDamageLv.Air;
                }
                else
                {
                    seq.m_sideB.m_enemyDamageLv = EnumDamageLv.Air;
                }
            }
            else if (actionType == EnumActionType.Jump
            || actionType == EnumActionType.Guard)
            {
                if (sideInfo.m_isSideA)
                {
                    seq.m_sideA.m_enemyDamageLv = EnumDamageLv.NoDamage;
                }
                else
                {

                    seq.m_sideB.m_enemyDamageLv = EnumDamageLv.NoDamage;
                }
            }
            else
            {
                // TODO:確率算出
                if (sideInfo.m_isSideA)
                {
                    int sideADamage = UnityEngine.Random.Range(0, 3);
                    seq.m_sideA.m_enemyDamageLv = (EnumDamageLv)sideADamage;
                    seq.m_sideA.m_enemyDamageLv = EnumDamageLv.Hit;
                }
                else
                {
                    int sideBDamage = UnityEngine.Random.Range(0, 3);
                    seq.m_sideB.m_enemyDamageLv = (EnumDamageLv)sideBDamage;
                    seq.m_sideB.m_enemyDamageLv = EnumDamageLv.Hit;
                }
            }

            return seq;
        }

        private static EnumDefenceType GetDefenceType(ref NativeArray<JumpState> jumpStates, int i, EnumActionType actionType)
        {
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

            return defenceType;
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

        private void UpdateDamage(ref BattleSequencer seq, NativeArray<DamageState> damageStates, NativeArray<Status> statuses,
            NativeArray<JumpState> jumpStates)
        {
            for (int i = 0; i < damageStates.Length; i++)
            {
                var damageState = damageStates[i];
                var jumpState = jumpStates[i];
                var status = statuses[i];

                bool isAttackSideA = !SideUtil.IsSideA(i);
                if (isAttackSideA)
                {
                    SetDamage(ref seq.m_sideA, ref damageState, status, jumpState);
                }
                else
                {
                    SetDamage(ref seq.m_sideB, ref damageState, status, jumpState);
                }

                damageStates[i] = damageState;
            }
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

        private static void SetDamage(ref SideState attackSideState, ref DamageState damageState, in Status status, in JumpState jumpState)
        {

            YHActionData attackData = Shared.m_yhCharaAttackList.GetData(attackSideState.m_charaNo, attackSideState.m_actionNo);
            int damage = attackData.power;
            int balance = attackData.balance;

            switch (attackSideState.m_enemyDamageLv)
            {
                // case EnumDamageLv.CleanHit:
                // break;
                case EnumDamageLv.Hit:
                    break;
                case EnumDamageLv.Tip:
                    damage = (int)(damage * Settings.Instance.Common.TipMag);
                    balance = (int)(balance * Settings.Instance.Common.TipMag);
                    break;
                case EnumDamageLv.NoDamage:
                case EnumDamageLv.Air:
                    damage = 0;
                    balance = 0;
                    break;
            }

            damageState.m_lifeDamage = damage;
            damageState.m_balanceDamage = balance;

            int afterBalance = (status.m_balance - balance);
            int afterLife = (status.m_life - damage);
            bool isJumpHit = (jumpState.m_state != EnumJumpState.None
                && attackSideState.m_enemyDamageLv >= EnumDamageLv.Hit);

            bool isFly = (afterLife <= 0 || afterBalance <= 0 || isJumpHit);
            bool isShakey = (afterBalance <= Settings.Instance.Common.ShakeyBorder);

            if (isFly)
            {
                attackSideState.m_enemyDamageReaction = EnumDamageReaction.Fly;
            }
            else if (isShakey)
            {
                attackSideState.m_enemyDamageReaction = EnumDamageReaction.Shaky;
            }
            else
            {
                attackSideState.m_enemyDamageReaction = EnumDamageReaction.None;
            }
        }

        private static void InitSequencer(ref BattleSequencer battleSequencer, NativeArray<DownState> downStates, bool isSideA)
        {
            battleSequencer.m_seqState = EnumBattleSequenceState.Start;
            battleSequencer.m_isLastSideA = isSideA;
            battleSequencer.m_isDistributeRei = false;

            InitSideState(ref battleSequencer.m_sideA, downStates[0]);
            InitSideState(ref battleSequencer.m_sideB, downStates[1]);

        }

        private static void InitSideState(ref SideState sideState, in DownState damageState)
        {
            bool isDown = damageState.m_state == EnumDownState.Down;

            sideState.m_actionType = (isDown)
                ? EnumActionType.Reverse
                : EnumActionType.None;

            sideState.m_animStep = (isDown)
                ? EnumAnimationStep.WaitPageA
                : EnumAnimationStep.Sleep;

            sideState.m_enemyDamageLv = EnumDamageLv.NoDamage;
            sideState.m_isDefenceFinished = false;
            sideState.m_isStartDamage = false;
            sideState.m_isConsumeRei = false;
            sideState.m_isReverse = false;
        }

        private static void InitActionSide(in SideInfo attackSideInfo, ref SideState attackSideState,
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
            attackSideState.m_isReverse = false;
        }

    }
}
