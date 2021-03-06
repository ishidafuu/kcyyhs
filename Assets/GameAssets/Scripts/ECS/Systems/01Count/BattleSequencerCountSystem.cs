﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class BattleSequencerCountSystem : ComponentSystem
    {
        enum NextStepType
        {
            None,
            DamageReactionStep,
            DeffenceStepWaitSide,
            DeffenceStepLastSide,
            NextStepLastSide,
            NextStepWaitSide,
            EndAnimation,
        }


        EntityQuery m_queryFillter;

        static void DebugLog(string message)
        {
            Debug.Log(message);
        }

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BattleSequencer>();
            m_queryFillter = GetEntityQuery(
                ComponentType.ReadWrite<FilterEffect>()
            );
        }

        protected override void OnUpdate()
        {
            if (Settings.Instance.Debug.IsSkip())
                return;

            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (seq.m_seqState == EnumBattleSequenceState.Idle)
                return;

            UpdateSequencer(ref seq);
        }

        private void UpdateSequencer(ref BattleSequencer seq)
        {
            int charaNo = seq.m_animation.m_charaNo;
            EnumAnimationName animName = seq.m_animation.m_animName;
            YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);

            seq.m_animation.m_count++;

            bool isEndTransitionFilter = false;
            // 最初の入力から最初のアニメ開始もここで行う
            switch (seq.m_seqState)
            {
                case EnumBattleSequenceState.Start:
                    if (seq.m_animation.m_count > Settings.Instance.Animation.StartTransitionTime)
                    {
                        seq.m_seqState = EnumBattleSequenceState.Play;
                        seq.m_animation.m_count = 0;
                        if (seq.m_isLastSideA)
                        {
                            NextStep(ref seq, ref seq.m_sideA, ref seq.m_sideB);
                        }
                        else
                        {
                            NextStep(ref seq, ref seq.m_sideB, ref seq.m_sideA);
                        }
                        // seqが更新された直後にエフェクトの発生を行う
                        UpdateFilterEffect(ref seq, isEndTransitionFilter);
                    }
                    break;
                case EnumBattleSequenceState.Play:

                    if (seq.m_animation.m_count >= anim.m_length)
                    {
                        seq.m_animation.m_count = 0;

                        if (seq.m_isLastSideA)
                        {
                            NextStepType step = SelectNextStep(ref seq, ref seq.m_sideA, ref seq.m_sideB);
                            ShiftNextStep(step, ref seq, ref seq.m_sideA, ref seq.m_sideB);
                        }
                        else
                        {
                            NextStepType step = SelectNextStep(ref seq, ref seq.m_sideB, ref seq.m_sideA);
                            ShiftNextStep(step, ref seq, ref seq.m_sideB, ref seq.m_sideA);
                        }
                    }
                    // 切り替えフィルタ表示
                    else if (seq.m_animation.m_count == anim.m_length - Settings.Instance.Animation.EndTransitionTime)
                    {
                        NextStepType step = (seq.m_isLastSideA)
                            ? SelectNextStep(ref seq, ref seq.m_sideA, ref seq.m_sideB)
                            : SelectNextStep(ref seq, ref seq.m_sideB, ref seq.m_sideA);

                        isEndTransitionFilter = (step == NextStepType.EndAnimation);
                    }

                    // seqが更新された直後にエフェクトの発生を行う
                    UpdateFilterEffect(ref seq, isEndTransitionFilter);

                    break;
            }

            SetSingleton(seq);
        }

        private void ShiftNextStep(NextStepType step, ref BattleSequencer seq,
            ref SideState lastSide, ref SideState waitSide)
        {
            switch (step)
            {
                case NextStepType.DamageReactionStep:
                    DamageReactionStep(ref seq, ref waitSide, ref lastSide);
                    break;
                case NextStepType.DeffenceStepWaitSide:
                    DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    break;
                case NextStepType.DeffenceStepLastSide:
                    DeffenceStep(ref seq, ref waitSide, ref lastSide);
                    break;
                case NextStepType.NextStepLastSide:
                    NextStep(ref seq, ref lastSide, ref waitSide);
                    break;
                case NextStepType.NextStepWaitSide:
                    NextStep(ref seq, ref waitSide, ref lastSide);
                    break;
                case NextStepType.EndAnimation:
                    EndAnimation(ref seq);
                    break;
            }
        }

        private void UpdateFilterEffect(ref BattleSequencer seq, bool isEndTransitionFilter)
        {
            bool isEffectUpdate = false;
            NativeArray<FilterEffect> filterEffects = m_queryFillter.ToComponentDataArray<FilterEffect>(Allocator.TempJob);

            // アニメが切り替わったタイミングでフィルターエフェクトはすべてリセット
            if (seq.m_animation.m_count == 0)
            {
                ResetEffect(ref filterEffects);
                isEffectUpdate = true;
            }

            bool isSideA = seq.m_animation.m_isSideA;

            // End(Idle)まできたらエフェクトは発生しない
            if (seq.m_seqState != EnumBattleSequenceState.Idle)
            {
                int charaNo = seq.m_animation.m_charaNo;
                EnumAnimationName animName = seq.m_animation.m_animName;
                YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);
                foreach (var item in anim.m_events)
                {
                    if (item.m_frame != seq.m_animation.m_count)
                        continue;

                    // DebugLog($"{item.m_functionName}:{item.m_intParameter}");
                    switch (item.m_functionName)
                    {
                        case EnumEventFunctionName.EventEffectBG:
                            SetEffect(filterEffects, EnumEffectType.EffectBG, item.m_intParameter, isSideA);
                            isEffectUpdate = true;
                            break;
                        case EnumEventFunctionName.EventEffectScreen:
                            SetEffect(filterEffects, EnumEffectType.EffectScreen, item.m_intParameter, isSideA);
                            isEffectUpdate = true;
                            break;
                        case EnumEventFunctionName.EventEffectLarge:
                            SetEffect(filterEffects, EnumEffectType.EffectLarge, item.m_intParameter, isSideA);
                            isEffectUpdate = true;
                            break;
                        case EnumEventFunctionName.EventEffectMedium:
                            SetEffect(filterEffects, EnumEffectType.EffectMedium, item.m_intParameter, isSideA);
                            isEffectUpdate = true;
                            break;
                        case EnumEventFunctionName.EventEffectSmall:
                            SetEffect(filterEffects, EnumEffectType.EffectSmall, item.m_intParameter, isSideA);
                            isEffectUpdate = true;
                            break;
                        case EnumEventFunctionName.EventFillterScreen:
                            SetEffect(filterEffects, EnumEffectType.FillterScreen, item.m_intParameter, isSideA);
                            isEffectUpdate = true;
                            break;
                        case EnumEventFunctionName.EventEffectDamageBody:
                            StopOtherDamageEffect(filterEffects);
                            SetEffect(filterEffects, EnumEffectType.EffectDamageBody, (int)EnumEffectLarge.Damage, isSideA);
                            isEffectUpdate = true;
                            break;
                        case EnumEventFunctionName.EventEffectDamageFace:
                            StopOtherDamageEffect(filterEffects);
                            SetEffect(filterEffects, EnumEffectType.EffectDamageFace, (int)EnumEffectLarge.Damage, isSideA);
                            // ダメージ変換開始
                            // TODO:直撃以外
                            if (isSideA)
                            {
                                seq.m_sideA.m_isStartDamage = true;
                            }
                            else
                            {
                                seq.m_sideB.m_isStartDamage = true;
                            }
                            isEffectUpdate = true;
                            break;
                    }
                }
            }

            // バトルシーケンス終了フィルタ
            if (isEndTransitionFilter)
            {
                SetEffect(filterEffects, EnumEffectType.FillterScreen, (int)EnumFillter.EndBattleSequence, isSideA);
                isEffectUpdate = true;
            }
            // バトルシーケンスから分割画面への切り替えフィルタ
            else if (seq.m_seqState == EnumBattleSequenceState.Idle)
            {
                SetEffect(filterEffects, EnumEffectType.FillterScreen, (int)EnumFillter.SwitchSplitView, isSideA);
                isEffectUpdate = true;
            }

            if (isEffectUpdate)
            {
                m_queryFillter.CopyFromComponentDataArray(filterEffects);
            }
            filterEffects.Dispose();
        }

        private static void StopOtherDamageEffect(NativeArray<FilterEffect> filterEffects)
        {
            for (int i = 0; i < filterEffects.Length; i++)
            {
                var item = filterEffects[i];

                if (!item.m_isActive)
                    continue;

                switch (item.m_effectType)
                {
                    case EnumEffectType.EffectDamageBody:
                    case EnumEffectType.EffectDamageFace:
                        item.m_isActive = false;
                        break;
                    default:
                        continue;
                }

                DebugLog($"{item.m_effectType}:{i} isActive:{item.m_isActive}");
                filterEffects[i] = item;
            }
        }

        private static void SetEffect(NativeArray<FilterEffect> filterEffects,
            EnumEffectType effectType, int effectIndex, bool isSideA)
        {
            for (int i = 0; i < filterEffects.Length; i++)
            {
                FilterEffect effect = filterEffects[i];

                if (effect.m_isActive)
                    continue;

                effect.m_isActive = true;
                effect.m_effectType = effectType;
                effect.m_effectIndex = effectIndex;
                effect.m_isSideA = isSideA;
                effect.m_count = 0;
                filterEffects[i] = effect;
                // DebugLog($"{effect.m_effectType}:{i} isActive:{effect.m_isActive}");
                break;
            }
        }

        private static void ResetEffect(ref NativeArray<FilterEffect> filterEffects)
        {
            // DebugLog("ResetEffect");
            for (int i = 0; i < filterEffects.Length; i++)
            {
                FilterEffect effect = filterEffects[i];
                effect.m_isActive = false;
                effect.m_count = 0;
                filterEffects[i] = effect;
            }
        }

        private NextStepType SelectNextStep(ref BattleSequencer seq,
            ref SideState lastSide, ref SideState waitSide)
        {
            NextStepType step = NextStepType.None;

            // 直前がディフェンスアニメーションの場合（条件でダウンへ分岐なども行う）
            if (seq.m_animType != EnumAnimType.Action)
            {
                DebugLog("SelectNextStep1");
                // リアクションあり
                if (seq.m_animType == EnumAnimType.Defence
                    && waitSide.m_enemyDamageReaction != EnumDamageReaction.None)
                {
                    DebugLog("DamageReactionStep ");
                    step = NextStepType.DamageReactionStep;
                }
                // リアクションなし
                else
                {
                    // 直前側が攻撃完了していて相手のディフェンスが終わってない場合は、相手側のディフェンス
                    if (lastSide.m_animStep == EnumAnimationStep.Finished
                        && !waitSide.m_isDefenceFinished
                        && lastSide.m_isEnemyNeedDefence)
                    {
                        DebugLog("DeffenceStepA ");
                        step = NextStepType.DeffenceStepWaitSide;
                    }
                    // 直撃もしくはKO以外で、直前側が攻撃完了していない場合は、直前側の進行
                    else if (!waitSide.m_enemyDamageLv.IsHit()
                        && (lastSide.m_animStep == EnumAnimationStep.WaitPageA
                            || lastSide.m_animStep == EnumAnimationStep.WaitPageB))
                    {
                        step = NextStepType.NextStepLastSide;
                    }
                    else
                    {
                        DebugLog("EndAnimationA");
                        // アニメ終了
                        step = NextStepType.EndAnimation;
                    }
                }
            }
            // 待ち側のアクションがない場合は直前のサイドを連続させる
            else if (waitSide.m_actionType == EnumActionType.None)
            {
                DebugLog($"SelectNextStep2 {lastSide.m_isSideA} {lastSide.m_animStep}");
                if (lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    // ディフェンス
                    if (!waitSide.m_isDefenceFinished && lastSide.m_isEnemyNeedDefence)
                    {
                        DebugLog("DeffenceStepB ");
                        step = NextStepType.DeffenceStepWaitSide;
                    }
                    else
                    {
                        // ディフェンス不要の場合はアニメ終了
                        DebugLog("EndAnimationB");
                        step = NextStepType.EndAnimation;
                    }
                }
                else
                {
                    // 直前サイドを進行
                    step = NextStepType.NextStepLastSide;
                }
            }
            else
            {
                DebugLog($"SelectNextStep3 {waitSide.m_isSideA} {waitSide.m_animStep}");
                // 待ち側のアクション始動

                // 直前が近接攻撃の完了（完了まで出たら追いつけない）の場合
                // もしくは、直前が追いつき不可能攻撃の完了の場合は待ち側ディフェンス
                if (lastSide.m_animStep == EnumAnimationStep.Finished
                    && lastSide.m_isEnemyNeedDefence
                    && !waitSide.m_isDefenceFinished
                    && (lastSide.m_actionType.IsShort()
                        || lastSide.m_actionType.IsChaseable(waitSide.m_actionType))
                    )
                {
                    DebugLog("DeffenceStepC ");
                    step = NextStepType.DeffenceStepWaitSide;
                }
                // 未始動であれば始動へ
                else if (waitSide.m_animStep == EnumAnimationStep.WaitPageA)
                {
                    step = NextStepType.NextStepWaitSide;
                }
                // 両方の始動ステップが終わった
                else if (waitSide.m_animStep == EnumAnimationStep.WaitPageB
                        && lastSide.m_animStep == EnumAnimationStep.WaitPageB)
                {
                    // 直前のアクションが待ちアクションよりプライオリティが高ければ追い越して、直前を再度進める
                    if (lastSide.m_actionType.IsOvertakeable(waitSide.m_actionType))
                    {
                        step = NextStepType.NextStepLastSide;
                    }
                    else
                    {
                        // 待ち側の発動ステップ
                        step = NextStepType.NextStepWaitSide;
                    }
                }
                // 直前が発動、待ち側が始動
                else if (waitSide.m_animStep == EnumAnimationStep.WaitPageB
                        && lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    // 待ち側が追いつき可能な場合は発動する
                    if (waitSide.m_actionType.IsChaseable(lastSide.m_actionType))
                    {
                        // 直撃でなければ待ち側のアクションを進める
                        if (lastSide.m_enemyDamageLv >= EnumDamageLv.Hit)
                        {
                            step = NextStepType.NextStepWaitSide;
                        }
                        else
                        {
                            DebugLog("EndAnimationC");
                            step = NextStepType.EndAnimation;
                        }
                    }
                    else
                    {
                        DebugLog("DeffenceStepE ");
                        step = NextStepType.DeffenceStepWaitSide;
                    }
                }
                // 直前が始動、待ち側が発動完了（先手後手ともに飛び道具で、後手が追いかけるような場合）
                else if (waitSide.m_animStep == EnumAnimationStep.Finished
                        && lastSide.m_animStep == EnumAnimationStep.WaitPageB)
                {
                    // 直前側が追いつき可能な場合は連続する
                    // ディフェンスが完了している場合も連続する
                    if (lastSide.m_actionType.IsChaseable(waitSide.m_actionType)
                        || lastSide.m_isDefenceFinished)
                    {
                        step = NextStepType.NextStepLastSide;
                    }
                    else
                    {
                        DebugLog("DeffenceStepF ");
                        step = NextStepType.DeffenceStepLastSide;
                    }
                }
                // 近接攻撃を避けた後などの後手側アクションの後
                else if (waitSide.m_animStep == EnumAnimationStep.Finished
                    && lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    if (waitSide.m_isEnemyNeedDefence && !lastSide.m_isDefenceFinished)
                    {
                        DebugLog("DeffenceStepGS ");
                        step = NextStepType.DeffenceStepLastSide;
                    }
                    else if (lastSide.m_isEnemyNeedDefence && !waitSide.m_isDefenceFinished)
                    {
                        DebugLog("DeffenceStepH ");
                        step = NextStepType.DeffenceStepWaitSide;
                    }
                    else
                    {
                        DebugLog("EndAnimationD");
                        // アニメ終了
                        step = NextStepType.EndAnimation;
                    }
                }
                else
                {
                    Debug.LogError($"SelectNextStepError waitStep:{waitSide.m_animStep} lastStep:{lastSide.m_animStep}");
                }

            }

            return step;
        }

        private void EndAnimation(ref BattleSequencer seq)
        {
            seq.m_seqState = EnumBattleSequenceState.Idle;
            seq.m_animation.m_count = 0;
            seq.m_sideA.m_animStep = EnumAnimationStep.Sleep;
            seq.m_sideB.m_animStep = EnumAnimationStep.Sleep;
            seq.m_isDistributeRei = true;
        }

        private void NextStep(ref BattleSequencer seq, ref SideState nextSide, ref SideState waitSide)
        {
            bool isReverse = false;
            bool isWait = false;
            DebugLog($"NextStepactionNo:{nextSide.m_actionNo} isSideA:{nextSide.m_isSideA} animStep:{nextSide.m_animStep} animName:{seq.m_animation.m_animName}");
            seq.m_animation.m_charaNo = nextSide.m_charaNo;
            seq.m_animation.m_isSideA = nextSide.m_isSideA;

            switch (nextSide.m_actionType)
            {
                case EnumActionType.Reverse:
                    isReverse = true;
                    seq.m_animation.m_animName = EnumAnimationName._Reverse;
                    break;
                case EnumActionType.Jump:
                case EnumActionType.Guard:
                    isWait = true;
                    if (nextSide.m_animStep == EnumAnimationStep.WaitPageA)
                    {
                        seq.m_animation.m_animName = EnumAnimationName._Wait;
                    }
                    else
                    {
                        seq.m_animation.m_animName = EnumAnimationName._JumpAction;
                    }
                    break;
                default:
                    seq.m_animation.m_animName = GetActionName(nextSide.m_actionNo, nextSide.m_animStep);
                    break;
            }

            seq.m_animType = EnumAnimType.Action;
            seq.m_isLastSideA = nextSide.m_isSideA;
            if (isReverse)
            {
                nextSide.m_animStep = EnumAnimationStep.Finished;
                Reverse(ref seq, ref nextSide);
            }
            else if (isWait)
            {
                // 相手が攻撃モーションの場合は、ディフェンス（避けジャンプ）モーションがあるので完了にする
                if (waitSide.m_isEnemyNeedDefence)
                {
                    nextSide.m_animStep = EnumAnimationStep.Finished;
                }
            }
            else
            {
                nextSide.m_animStep++;
            }

            if (nextSide.m_animStep == EnumAnimationStep.Finished)
            {
                ConsumeRei(ref seq, ref nextSide);
            }
        }

        private static void ConsumeRei(ref BattleSequencer seq, ref SideState nextSide)
        {
            if (seq.m_animation.m_isSideA)
            {
                seq.m_sideA.m_isConsumeRei = true;
            }
            else
            {
                seq.m_sideB.m_isConsumeRei = true;
            }
        }

        private static void Reverse(ref BattleSequencer seq, ref SideState nextSide)
        {
            if (seq.m_animation.m_isSideA)
            {
                seq.m_sideA.m_isReverse = true;
            }
            else
            {
                seq.m_sideB.m_isReverse = true;
            }
        }

        private void DeffenceStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            seq.m_animation.m_charaNo = deffenceSide.m_charaNo;
            seq.m_animation.m_isSideA = deffenceSide.m_isSideA;
            bool isJumping = deffenceSide.m_actionType == EnumActionType.Jump;
            seq.m_animation.m_animName = GetDeffenceName(attackSide.m_enemyDeffenceType, attackSide.m_enemyDamageLv, isJumping);
            DebugLog($"DeffenceStep isSideA:{deffenceSide.m_isSideA} animName:{seq.m_animation.m_animName}");
            seq.m_animType = EnumAnimType.Defence;
            seq.m_isLastSideA = deffenceSide.m_isSideA;
            deffenceSide.m_isDefenceFinished = true;
        }

        private void DamageReactionStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            DebugLog($"DamageReactionStep{seq.m_animation.m_count}");
            seq.m_animation.m_charaNo = deffenceSide.m_charaNo;
            seq.m_animation.m_isSideA = deffenceSide.m_isSideA;
            seq.m_animation.m_animName = GetDamageReactionName(attackSide.m_enemyDamageReaction);
            seq.m_animType = EnumAnimType.DefenceReaction;
            seq.m_isLastSideA = deffenceSide.m_isSideA;
        }

        private EnumAnimationName GetActionName(int actionNo, EnumAnimationStep animStep)
        {
            int step = 2;
            int index = (int)EnumAnimationName.Action00_00 + (actionNo * step) + (int)(animStep - EnumAnimationStep.WaitPageA);
            return (EnumAnimationName)index;
        }

        private EnumAnimationName GetDeffenceName(EnumDefenceType deffenceType, EnumDamageLv damageLv, bool isJumping)
        {
            int step = 3;
            int index = (isJumping)
                ? (int)EnumAnimationName._DefenceC00 + (int)damageLv
                : (int)EnumAnimationName._DefenceA00 + ((int)deffenceType * step) + (int)damageLv;
            return (EnumAnimationName)index;
        }

        private EnumAnimationName GetDamageReactionName(EnumDamageReaction reactionType)
        {
            switch (reactionType)
            {
                case EnumDamageReaction.Shaky:
                    return EnumAnimationName._Shaky;
                case EnumDamageReaction.Fly:
                    return EnumAnimationName._Fly;
            }

            return EnumAnimationName._Fly;
        }


    }
}
