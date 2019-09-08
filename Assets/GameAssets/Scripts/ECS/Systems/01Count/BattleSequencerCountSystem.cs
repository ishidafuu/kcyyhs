using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class BattleSequencerCountSystem : ComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BattleSequencer>();
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<FilterEffect>()
            );
        }

        protected override void OnUpdate()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (!seq.m_isPlay)
                return;

            UpdateSequencer(ref seq);
        }

        private void UpdateSequencer(ref BattleSequencer seq)
        {
            int charaNo = seq.m_animation.m_charaNo;
            EnumAnimationName animName = seq.m_animation.m_animName;
            YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);

            // 最初の入力から最初のアニメ開始もここで行う
            if (seq.m_isTransition)
            {
                seq.m_isTransition = false;
                if (seq.m_isLastSideA)
                {
                    NextStep(ref seq, ref seq.m_sideA);
                }
                else
                {
                    NextStep(ref seq, ref seq.m_sideB);
                }
            }
            else
            {
                seq.m_animation.m_count++;

                if (seq.m_animation.m_count >= anim.m_length)
                {
                    seq.m_animation.m_count = 0;
                    if (seq.m_isLastSideA)
                    {
                        SelectNextStep(ref seq, ref seq.m_sideA, ref seq.m_sideB);
                    }
                    else
                    {
                        SelectNextStep(ref seq, ref seq.m_sideB, ref seq.m_sideA);
                    }
                }
            }

            // seqが更新された直後に行う
            UpdateFilterEffect(seq);

            SetSingleton(seq);
        }

        private void UpdateFilterEffect(BattleSequencer seq)
        {
            bool isEffectUpdate = false;
            NativeArray<FilterEffect> filterEffects = m_query.ToComponentDataArray<FilterEffect>(Allocator.TempJob);

            // アニメが切り替わったタイミングでフィルターエフェクトはすべてリセット
            if (seq.m_animation.m_count == 0)
            {
                for (int i = 0; i < filterEffects.Length; i++)
                {
                    FilterEffect effect = filterEffects[i];
                    effect.m_isActive = false;
                    effect.m_count = 0;
                    filterEffects[i] = effect;
                }
                isEffectUpdate = true;
            }


            if (seq.m_isPlay)
            {
                int charaNo = seq.m_animation.m_charaNo;
                EnumAnimationName animName = seq.m_animation.m_animName;
                YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);
                foreach (var item in anim.m_events)
                {
                    if (item.m_frame != seq.m_animation.m_count)
                        continue;

                    if (item.m_functionName == EnumEventFunctionName.EventEffect)
                    {
                        for (int i = 0; i < filterEffects.Length; i++)
                        {
                            FilterEffect effect = filterEffects[i];

                            if (effect.m_isActive)
                                continue;

                            effect.m_isActive = true;
                            effect.m_effectIndex = Shared.m_yhFilterEffectList.GetEffectIndex(item.m_stringParameter);
                            effect.m_count = 0;
                            filterEffects[i] = effect;

                            isEffectUpdate = true;
                        }
                    }
                }
            }

            if (isEffectUpdate)
            {
                m_query.CopyFromComponentDataArray(filterEffects);
            }
            filterEffects.Dispose();
        }

        private void SelectNextStep(ref BattleSequencer seq,
            ref SideState lastSide, ref SideState waitSide)
        {
            // 直前がディフェンスアニメーションの場合（条件でダウンへ分岐なども行う）
            if (seq.m_animType != EnumAnimType.Action)
            {
                Debug.Log("SelectNextStep1");
                // リアクションあり
                if (seq.m_animType == EnumAnimType.Defence
                    && waitSide.m_enemyDamageReaction != EnumDamageReaction.None)
                {
                    DamageReactionStep(ref seq, ref lastSide, ref waitSide);
                }
                // リアクションなし
                else
                {
                    // 直前側が攻撃完了していて相手のディフェンスが終わってない場合は、相手側のディフェンス
                    if (lastSide.m_animStep == EnumAnimationStep.Finished
                        && !waitSide.m_isDefenceFinished
                        && lastSide.m_isEnemyNeedDefence)
                    {
                        Debug.Log("DeffenceStepA ");
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                    // 直撃もしくはKO以外で、直前側が攻撃完了していない場合は、直前側の進行
                    else if (waitSide.m_enemyDamageLv <= EnumDamageLv.Tip
                        && (lastSide.m_animStep == EnumAnimationStep.WaitPageA
                            || lastSide.m_animStep == EnumAnimationStep.WaitPageB))
                    {
                        NextStep(ref seq, ref lastSide);
                    }
                    else
                    {
                        Debug.Log("EndAnimationA");
                        // アニメ終了
                        EndAnimation(ref seq);
                    }
                }
            }
            // 待ち側のアクションがない場合は直前のサイドを連続させる
            else if (waitSide.m_actionType == EnumActionType.None)
            {
                Debug.Log($"SelectNextStep2 {lastSide.m_isSideA} {lastSide.m_animStep}");
                if (lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    // ディフェンス
                    if (!waitSide.m_isDefenceFinished && lastSide.m_isEnemyNeedDefence)
                    {
                        Debug.Log("DeffenceStepB ");
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                    else
                    {
                        // ディフェンス不要の場合はアニメ終了
                        Debug.Log("EndAnimationB");
                        EndAnimation(ref seq);
                    }
                }
                else
                {
                    // 直前サイドを進行
                    NextStep(ref seq, ref lastSide);
                }
            }
            else
            {
                Debug.Log($"SelectNextStep3 {waitSide.m_isSideA} {waitSide.m_animStep}");
                // 待ち側のアクション始動

                // 未始動であれば始動へ
                if (waitSide.m_animStep == EnumAnimationStep.WaitPageA)
                {
                    NextStep(ref seq, ref waitSide);
                }
                // 両方の始動ステップが終わった
                else if (waitSide.m_animStep == EnumAnimationStep.WaitPageB
                        && lastSide.m_animStep == EnumAnimationStep.WaitPageB)
                {
                    // 直前のアクションが待ちアクションよりプライオリティが高ければ追い越して、直前を再度進める
                    if (lastSide.m_actionType > waitSide.m_actionType)
                    {
                        NextStep(ref seq, ref lastSide);
                    }
                    else
                    {
                        // 待ち側の発動ステップ
                        NextStep(ref seq, ref waitSide);
                    }
                }
                // 直前が発動、待ち側が始動
                else if (waitSide.m_animStep == EnumAnimationStep.WaitPageB
                        && lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    // 待ち側が追いつき可能な場合は発動する
                    // 通常、同一プライオリティは同着を許すが、直接攻撃同士は先手解決後に発動する
                    if (!lastSide.m_isEnemyNeedDefence
                        || CheckChasable(waitSide.m_actionType, lastSide.m_actionType))
                    {
                        // 直撃でなければ待ち側のアクションを進める
                        if (lastSide.m_enemyDamageLv < EnumDamageLv.Hit)
                        {
                            NextStep(ref seq, ref waitSide);
                        }
                        else
                        {
                            Debug.Log("EndAnimationC");
                            EndAnimation(ref seq);
                        }
                    }
                    else
                    {
                        Debug.Log("DeffenceStepC ");
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                }
                // 直前が始動、待ち側が発動完了（先手後手ともに飛び道具で、後手が追いかけるような場合）
                else if (waitSide.m_animStep == EnumAnimationStep.Finished
                        && lastSide.m_animStep == EnumAnimationStep.WaitPageB)
                {
                    // 直前側が追いつき可能な場合は、連続する
                    if (CheckChasable(lastSide.m_actionType, waitSide.m_actionType))
                    {
                        NextStep(ref seq, ref lastSide);
                    }
                    else
                    {
                        DeffenceStep(ref seq, ref waitSide, ref lastSide);
                    }
                }
                // 近接攻撃を避けた後などの後手側アクションの後
                else if (waitSide.m_animStep == EnumAnimationStep.Finished
                    && lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    if (waitSide.m_isEnemyNeedDefence && !lastSide.m_isDefenceFinished)
                    {
                        Debug.Log("DeffenceStepD ");
                        DeffenceStep(ref seq, ref waitSide, ref lastSide);
                    }
                    else if (lastSide.m_isEnemyNeedDefence && !waitSide.m_isDefenceFinished)
                    {
                        Debug.Log("DeffenceStepE ");
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                    else
                    {
                        Debug.Log("EndAnimationD");
                        // アニメ終了
                        EndAnimation(ref seq);
                    }
                }
                else
                {
                    Debug.LogError($"SelectNextStepError waitStep:{waitSide.m_animStep} lastStep:{lastSide.m_animStep}");
                }

            }
        }

        private bool CheckChasable(EnumActionType chaseType, EnumActionType targetType)
        {
            return (chaseType > targetType
                || (chaseType == targetType && chaseType != EnumActionType.ShortAttack));
        }


        private void EndAnimation(ref BattleSequencer seq)
        {
            seq.m_isPlay = false;
            seq.m_sideA.m_animStep = EnumAnimationStep.Sleep;
            seq.m_sideB.m_animStep = EnumAnimationStep.Sleep;
        }

        private void NextStep(ref BattleSequencer seq, ref SideState nextSide)
        {
            Debug.Log($"NextStepactionNo:{nextSide.m_actionNo} isSideA:{nextSide.m_isSideA} animStep:{nextSide.m_animStep} animName:{seq.m_animation.m_animName}");
            seq.m_animation.m_charaNo = nextSide.m_charaNo;
            seq.m_animation.m_isSideA = nextSide.m_isSideA;
            seq.m_animation.m_animName = GetActionName(nextSide.m_actionNo, nextSide.m_animStep);
            seq.m_animType = EnumAnimType.Action;
            seq.m_isLastSideA = nextSide.m_isSideA;
            nextSide.m_animStep++;
        }

        private void DeffenceStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            seq.m_animation.m_charaNo = deffenceSide.m_charaNo;
            seq.m_animation.m_isSideA = deffenceSide.m_isSideA;
            seq.m_animation.m_animName = GetDeffenceName(attackSide.m_enemyDeffenceType, attackSide.m_enemyDamageLv);
            Debug.Log($"DeffenceStep isSideA:{deffenceSide.m_isSideA} animName:{seq.m_animation.m_animName}");
            seq.m_animType = EnumAnimType.Defence;
            seq.m_isLastSideA = deffenceSide.m_isSideA;
            deffenceSide.m_isDefenceFinished = true;
        }

        private void DamageReactionStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            Debug.Log("DamageReactionStep");
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

        private EnumAnimationName GetDeffenceName(EnumDefenceType deffenceType, EnumDamageLv damageLv)
        {
            int step = 3;
            int index = (int)EnumAnimationName.DefenceA00 + ((int)deffenceType * step) + (int)damageLv;
            return (EnumAnimationName)index;
        }

        private EnumAnimationName GetDamageReactionName(EnumDamageReaction reactionType)
        {
            switch (reactionType)
            {
                case EnumDamageReaction.Shaky:
                    return EnumAnimationName.Shaky;
                case EnumDamageReaction.Down:
                    return EnumAnimationName.Down;
            }

            return EnumAnimationName.Down;
        }


    }
}
