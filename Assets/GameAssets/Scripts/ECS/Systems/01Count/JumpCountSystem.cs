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
    public class JumpCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<JumpState>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (seq.m_seqState == EnumBattleSequenceState.Idle)
            {
                m_query.AddDependency(inputDeps);
                NativeArray<JumpState> jumpStates = m_query.ToComponentDataArray<JumpState>(Allocator.TempJob);
                // Vector2[] uv = Shared.m_bgFrameMeshMat.m_meshDict[EnumBGPartsType.bg00.ToString()].uv;
                var job = new CountToukiJob()
                {
                    m_jumpStates = jumpStates,
                    m_seq = seq,
                };

                inputDeps = job.Schedule(inputDeps);
                inputDeps.Complete();
                m_query.CopyFromComponentDataArray(jumpStates);
                jumpStates.Dispose();
            }


            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountToukiJob : IJob
        {
            public NativeArray<JumpState> m_jumpStates;
            [ReadOnly] public BattleSequencer m_seq;

            public void Execute()
            {
                for (int i = 0; i < m_jumpStates.Length; i++)
                {
                    var jumpState = m_jumpStates[i];

                    if (jumpState.m_state != EnumJumpState.None)
                    {
                        jumpState.m_totalCount++;
                        jumpState.m_stepCount++;
                        jumpState.m_animationCount++;

                        UpdateAnimation(ref jumpState);
                        UpdateFilterEffect(ref jumpState);
                        // if (seq.m_animation.m_count >= anim.m_length)
                    }

                    m_jumpStates[i] = jumpState;
                }
            }

            private void UpdateAnimation(ref JumpState jumpState)
            {
                switch (jumpState.m_state)
                {
                    case EnumJumpState.Jumping:
                        {
                            YHAnimation anim = Shared.m_yhCharaAnimList.GetCommonAnim(EnumAnimationName._Jump00);
                            if (jumpState.m_animationCount >= anim.m_length)
                            {
                                SetNextState(ref jumpState, EnumJumpState.Air);
                            }
                        }
                        break;
                    case EnumJumpState.Air:
                        {
                            if (jumpState.m_animationCount >= jumpState.m_charge)
                            {
                                SetNextState(ref jumpState, EnumJumpState.Falling);
                            }
                        }
                        break;
                    case EnumJumpState.Falling:
                        {
                            YHAnimation anim = Shared.m_yhCharaAnimList.GetCommonAnim(EnumAnimationName._Jump01);
                            if (jumpState.m_animationCount >= anim.m_length)
                            {
                                SetNextState(ref jumpState, EnumJumpState.None);
                                jumpState.m_effectStep = EnumJumpEffectStep.JumpStart;
                            }
                        }
                        break;
                }
            }

            private void SetNextState(ref JumpState jumpState, EnumJumpState nextState)
            {
                jumpState.m_state = nextState;
                jumpState.m_stepCount = 0;
                jumpState.m_animationCount = 0;
            }

            private void UpdateFilterEffect(ref JumpState jumpState)
            {
                EnumAnimationName animName = EnumAnimationName._Air00;

                bool isUpdate = false;
                switch (jumpState.m_state)
                {
                    case EnumJumpState.Jumping:
                        animName = EnumAnimationName._Jump00;
                        isUpdate = true;
                        break;
                    case EnumJumpState.Falling:
                        animName = EnumAnimationName._Jump01;
                        isUpdate = true;
                        break;
                }

                if (!isUpdate)
                    return;

                YHAnimation anim = Shared.m_yhCharaAnimList.GetCommonAnim(animName);
                foreach (var item in anim.m_events)
                {
                    if (item.m_frame != jumpState.m_animationCount)
                        continue;

                    if (item.m_functionName == EnumEventFunctionName.EventJump)
                    {
                        jumpState.m_effectStep = (EnumJumpEffectStep)item.m_intParameter;
                        jumpState.m_stepCount = 0;
                        break;
                    }
                }
            }
        }


    }
}
