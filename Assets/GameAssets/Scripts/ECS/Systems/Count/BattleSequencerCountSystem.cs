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
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BattleSequencer>();
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

                if (seq.m_animation.m_count >= anim.length)
                {
                    seq.m_sequenceStep++;
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

            SetSingleton(seq);
        }

        private void SelectNextStep(ref BattleSequencer seq,
            ref SideState lastSide, ref SideState waitSide)
        {
            Debug.Log("SelectNextStep");
            // 直前がディフェンスアニメーションの場合（条件でダウンへ分岐なども行う）
            if (seq.m_animType != EnumAnimType.Action)
            {
                Debug.Log("SelectNextStep 1");
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
                        && !waitSide.m_isEndDefence
                        && lastSide.m_isNeedDefence)
                    {
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                    // 直撃もしくはOK以外で、直前側が攻撃完了していない場合は、直前側の進行
                    else if (waitSide.m_enemyDamageLv <= EnumDamageLv.Tip
                     && (lastSide.m_animStep == EnumAnimationStep.Ready || lastSide.m_animStep == EnumAnimationStep.Start))
                    {
                        NextStep(ref seq, ref lastSide);
                    }
                    else
                    {
                        // アニメ終了
                        EndAnimation(ref seq);
                    }
                }
            }
            // 待ち側のアクションがない場合は直前のサイドを連続させる
            else if (waitSide.m_actionType == EnumActionType.None)
            {
                Debug.Log("SelectNextStep 2" + lastSide.m_animStep);
                if (lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    // ディフェンス
                    if (!waitSide.m_isEndDefence && lastSide.m_isNeedDefence)
                    {
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                    else
                    {
                        // ディフェンス不要の場合はアニメ終了
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
                Debug.Log("SelectNextStep 3");
                // 待ち側のアクション始動

                // 未始動であれば始動へ
                if (waitSide.m_animStep == EnumAnimationStep.Ready)
                {
                    NextStep(ref seq, ref waitSide);
                }
                // 両方の始動ステップが終わった
                else if (waitSide.m_animStep == EnumAnimationStep.Start
                        && lastSide.m_animStep == EnumAnimationStep.Start)
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
                else if (waitSide.m_animStep == EnumAnimationStep.Start
                        && lastSide.m_animStep == EnumAnimationStep.Fire)
                {
                    // 待ちアクションが直前の結果を待たずに発動できる場合は発動する
                    // 通常、同一プライオリティは同着を許すが、直接攻撃同士は先手解決後に発動する
                    if (!lastSide.m_isNeedDefence
                     || waitSide.m_actionType > lastSide.m_actionType
                     || (waitSide.m_actionType == lastSide.m_actionType && waitSide.m_actionType != EnumActionType.ShortAttack))
                    {
                        NextStep(ref seq, ref waitSide);
                    }
                    else
                    {
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                }
                else if (waitSide.m_animStep == EnumAnimationStep.Start
                    && lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    // 直撃でなければ待ち側のアクションを進める
                    if (lastSide.m_enemyDamageLv < EnumDamageLv.Hit)
                    {
                        NextStep(ref seq, ref waitSide);
                    }
                    else
                    {
                        EndAnimation(ref seq);
                    }
                }
                // 飛び道具同士など、先手側の攻撃の後手側ディフェンス
                else if (waitSide.m_isNeedDefence
                    && waitSide.m_animStep == EnumAnimationStep.Fire
                    && lastSide.m_animStep == EnumAnimationStep.Fire)
                {
                    DeffenceStep(ref seq, ref waitSide, ref lastSide);
                }
                // 飛び道具同士など、後手側の攻撃の先手側ディフェンス
                else if (waitSide.m_isNeedDefence
                    && waitSide.m_animStep == EnumAnimationStep.Fire
                    && lastSide.m_animStep == EnumAnimationStep.Finished)
                {
                    DeffenceStep(ref seq, ref waitSide, ref lastSide);
                }
                else
                {
                    // アニメ終了
                    EndAnimation(ref seq);
                }

            }
        }

        private void EndAnimation(ref BattleSequencer seq)
        {
            Debug.Log("EndAnimation");
            seq.m_isPlay = false;
            seq.m_sideA.m_animStep = EnumAnimationStep.Sleep;
            seq.m_sideB.m_animStep = EnumAnimationStep.Sleep;
        }

        private void NextStep(ref BattleSequencer seq, ref SideState nextSide)
        {
            Debug.Log("NextStep");
            seq.m_animation.m_charaNo = nextSide.m_charaNo;
            seq.m_animation.m_isSideA = nextSide.m_isSideA;
            seq.m_animation.m_animName = GetActionName(nextSide.m_actionNo, nextSide.m_animStep);
            Debug.Log($"actionNo{nextSide.m_actionNo} animStep{nextSide.m_animStep} animName{seq.m_animation.m_animName}");
            seq.m_animType = EnumAnimType.Action;
            seq.m_isLastSideA = nextSide.m_isSideA;
            nextSide.m_animStep++;
        }

        private void DeffenceStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            Debug.Log("DeffenceStep");
            seq.m_animation.m_charaNo = deffenceSide.m_charaNo;
            seq.m_animation.m_isSideA = deffenceSide.m_isSideA;
            seq.m_animation.m_animName = GetDeffenceName(attackSide.m_enemyDeffenceType, attackSide.m_enemyDamageLv);
            seq.m_animType = EnumAnimType.Defence;
            seq.m_isLastSideA = deffenceSide.m_isSideA;
            // 攻撃側のみ進める、防御側は進めない
            attackSide.m_animStep++;
            deffenceSide.m_isEndDefence = true;
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
            int index = (int)EnumAnimationName.Action00_00 + (actionNo * step) + (int)(animStep - EnumAnimationStep.Start);
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
