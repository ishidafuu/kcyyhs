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

            if (!seq.isPlay)
                return;

            UpdateSequencer(ref seq);
        }

        private void UpdateSequencer(ref BattleSequencer seq)
        {
            int charaNo = seq.animation.charaNo;
            EnumAnimationName animName = seq.animation.animName;
            YHAnimation anim = Shared.yhCharaAnimList.GetAnim(charaNo, animName);

            // TODO:最初の入力から最初のアニメ開始もここで行う
            if (seq.isTransition)
            {
                seq.isTransition = false;
            }
            else
            {
                seq.animation.count++;

                if (seq.animation.count >= anim.length)
                {
                    seq.sequenceStep++;
                    seq.animation.count = 0;
                    if (seq.isLastSideA)
                    {
                        UpdateSequencer2(ref seq, ref seq.sideA, ref seq.sideB);
                    }
                    else
                    {
                        UpdateSequencer2(ref seq, ref seq.sideB, ref seq.sideA);
                    }
                }
            }

            SetSingleton(seq);
        }

        private void UpdateSequencer2(ref BattleSequencer seq,
            ref SideState lastSide, ref SideState waitSide)
        {
            // 直前がディフェンスアニメーションの場合（条件でダウンへ分岐なども行う）
            if (seq.animType != EnumAnimType.Action)
            {
                // リアクションあり
                if (seq.animType == EnumAnimType.Defence
                    && waitSide.enemyDamageReaction != EnumDamageReaction.None)
                {
                    DamageReactionStep(ref seq, ref lastSide, ref waitSide);
                }
                // リアクションなし
                else
                {
                    // 直前側が攻撃完了していて相手のディフェンスが終わってない場合は、相手側のディフェンス
                    if (lastSide.animStep == EnumAnimationStep.Finished
                        && !waitSide.isEndDefence
                        && lastSide.isNeedDefence)
                    {
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                    // 直撃もしくはOK以外で、直前側が攻撃完了していない場合は、直前側の進行
                    else if (waitSide.enemyDamageLv <= EnumDamageLv.Tip
                     && (lastSide.animStep == EnumAnimationStep.Ready || lastSide.animStep == EnumAnimationStep.Start))
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
            else if (waitSide.actionType == EnumActionType.None)
            {
                switch (lastSide.animStep)
                {
                    // 発動ステップの場合はディフェンス
                    case EnumAnimationStep.Fire:
                        if (lastSide.isNeedDefence)
                        {
                            DeffenceStep(ref seq, ref lastSide, ref waitSide);
                        }
                        else
                        {
                            EndAnimation(ref seq);
                        }
                        break;
                    // 完了ステップの場合はアニメ終了
                    case EnumAnimationStep.Finished:
                        EndAnimation(ref seq);
                        break;
                    // 直前サイドを進行
                    default:
                        NextStep(ref seq, ref lastSide);
                        break;
                }
            }
            else
            {
                // 待ち側のアクション始動

                // 未始動であれば始動へ
                if (waitSide.animStep == EnumAnimationStep.Ready)
                {
                    NextStep(ref seq, ref waitSide);
                }
                // 両方の始動ステップが終わった
                else if (waitSide.animStep == EnumAnimationStep.Start
                        && lastSide.animStep == EnumAnimationStep.Start)
                {
                    // 直前のアクションが待ちアクションよりプライオリティが高ければ追い越して、直前を再度進める
                    if (lastSide.actionType > waitSide.actionType)
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
                else if (waitSide.animStep == EnumAnimationStep.Start
                        && lastSide.animStep == EnumAnimationStep.Fire)
                {
                    // 待ちアクションが直前の結果を待たずに発動できる場合は発動する
                    // 通常、同一プライオリティは同着を許すが、直接攻撃同士は先手解決後に発動する
                    if (!lastSide.isNeedDefence
                     || waitSide.actionType > lastSide.actionType
                     || (waitSide.actionType == lastSide.actionType && waitSide.actionType != EnumActionType.ShortAttack))
                    {
                        NextStep(ref seq, ref waitSide);
                    }
                    else
                    {
                        DeffenceStep(ref seq, ref lastSide, ref waitSide);
                    }
                }
                else if (waitSide.animStep == EnumAnimationStep.Start
                    && lastSide.animStep == EnumAnimationStep.Finished)
                {
                    // 直撃でなければ待ち側のアクションを進める
                    if (lastSide.enemyDamageLv < EnumDamageLv.Hit)
                    {
                        NextStep(ref seq, ref waitSide);
                    }
                    else
                    {
                        EndAnimation(ref seq);
                    }
                }
                // 飛び道具同士など、先手側の攻撃の後手側ディフェンス
                else if (waitSide.isNeedDefence
                    && waitSide.animStep == EnumAnimationStep.Fire
                    && lastSide.animStep == EnumAnimationStep.Fire)
                {
                    DeffenceStep(ref seq, ref waitSide, ref lastSide);
                }
                // 飛び道具同士など、後手側の攻撃の先手側ディフェンス
                else if (waitSide.isNeedDefence
                    && waitSide.animStep == EnumAnimationStep.Fire
                    && lastSide.animStep == EnumAnimationStep.Finished)
                {
                    DeffenceStep(ref seq, ref waitSide, ref lastSide);
                }
                else
                {
                    EndAnimation(ref seq);
                }

            }
        }

        private void EndAnimation(ref BattleSequencer seq)
        {
            seq.isPlay = false;
        }

        private void NextStep(ref BattleSequencer seq, ref SideState nextSide)
        {
            seq.animation.charaNo = nextSide.charaNo;
            seq.animation.animName = GetActionName(nextSide.actionNo, nextSide.animStep);
            seq.animType = EnumAnimType.Action;
            seq.isLastSideA = nextSide.isSideA;
            nextSide.animStep++;
        }

        private void DeffenceStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            seq.animation.charaNo = deffenceSide.charaNo;
            seq.animation.isSideA = deffenceSide.isSideA;
            seq.animation.animName = GetDeffenceName(attackSide.enemyDeffenceType, attackSide.enemyDamageLv);
            seq.animType = EnumAnimType.Defence;
            seq.isLastSideA = deffenceSide.isSideA;
            // 攻撃側のみ進める、防御側は進めない
            attackSide.animStep++;
            deffenceSide.isEndDefence = true;
        }

        private void DamageReactionStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            seq.animation.charaNo = deffenceSide.charaNo;
            seq.animation.isSideA = deffenceSide.isSideA;
            seq.animation.animName = GetDamageReactionName(attackSide.enemyDamageReaction);
            seq.animType = EnumAnimType.DefenceReaction;
            seq.isLastSideA = deffenceSide.isSideA;
        }

        private EnumAnimationName GetActionName(int actionNo, EnumAnimationStep animStep)
        {
            int step = 2;
            int index = (int)EnumAnimationName.Action00_00 + (actionNo * step) + (int)animStep;
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
