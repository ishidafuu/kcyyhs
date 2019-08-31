using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class BattleSequencerCountSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BattleSequencer>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (!seq.isPlay)
                return inputDeps;

            UpdateSequencer(ref seq);
            return inputDeps;
        }

        private void UpdateSequencer(ref BattleSequencer seq)
        {
            int charaNo = seq.animation.charaNo;
            EnumAnimationName animName = seq.animation.animName;
            YHAnimation anim = Shared.yhCharaAnimList.GetAnim(charaNo, animName);

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

            SetSingleton(seq);
        }

        private void UpdateSequencer2(ref BattleSequencer seq,
            ref SideState lastSide, ref SideState waitSide)
        {
            // ノーアクション以外
            if (waitSide.actionType != EnumActionType.None)
            {
                // 未始動であれば始動へ
                if (waitSide.animStep == EnumAnimationStep.Wait)
                {
                    NextStep(ref seq, ref waitSide);
                }
                else if (waitSide.animStep == EnumAnimationStep.Start
                        && lastSide.animStep == EnumAnimationStep.Start)
                {
                    // 直前のアクションが待ちアクションよりプライオリティが高ければ直前の方を再度進める
                    if (lastSide.actionType > waitSide.actionType)
                    {
                        NextStep(ref seq, ref lastSide);
                    }
                    else
                    {
                        NextStep(ref seq, ref waitSide);
                    }
                }
                else if (waitSide.animStep == EnumAnimationStep.Start
                        && lastSide.animStep == EnumAnimationStep.Fire)
                {
                    // 待ちアクションが直前の結果を待たずに発動できる場合は発動する
                    if (lastSide.actionType == waitSide.actionType
                        && waitSide.actionType != EnumActionType.ShortAttack)
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
                else if (waitSide.animStep == EnumAnimationStep.Fire
                        && lastSide.animStep == EnumAnimationStep.Fire)
                {
                    DeffenceStep(ref seq, ref waitSide, ref lastSide);
                }
                else if (waitSide.animStep == EnumAnimationStep.Fire
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
            seq.isLastSideA = nextSide.isSideA;
            nextSide.animStep++;
        }

        private void DeffenceStep(ref BattleSequencer seq, ref SideState attackSide, ref SideState deffenceSide)
        {
            seq.animation.charaNo = deffenceSide.charaNo;
            seq.animation.isSideA = deffenceSide.isSideA;
            seq.animation.animName = GetDeffenceName(attackSide.enemyDeffenceType, attackSide.enemyDamageLv);
            seq.isLastSideA = deffenceSide.isSideA;
            attackSide.animStep++;
        }

        private EnumAnimationName GetActionName(int actionNo, EnumAnimationStep animStep)
        {
            int step = 2;
            int index = (int)EnumAnimationName.Action00_00 + (actionNo * step) + (int)animStep;
            return (EnumAnimationName)index;
        }

        private EnumAnimationName GetDeffenceName(int deffenceType, EnumDamageLv damageLv)
        {
            int step = 3;
            int index = (int)EnumAnimationName.DefenceA00 + (deffenceType * step) + (int)damageLv;
            return (EnumAnimationName)index;
        }

    }
}
