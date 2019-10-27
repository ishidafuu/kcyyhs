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
    public class DownCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<Status>(),
                ComponentType.ReadWrite<DownState>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {

            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (seq.m_seqState == EnumBattleSequenceState.Idle)
            {
                m_query.AddDependency(inputDeps);
                NativeArray<Status> statuses = m_query.ToComponentDataArray<Status>(Allocator.TempJob);
                NativeArray<DownState> downStates = m_query.ToComponentDataArray<DownState>(Allocator.TempJob);
                // Vector2[] uv = Shared.m_bgFrameMeshMat.m_meshDict[EnumBGPartsType.bg00.ToString()].uv;
                var job = new CountJob()
                {
                    m_downStates = downStates,
                    m_statuses = statuses,
                    m_seq = seq,
                    ReverseFrame = Settings.Instance.Common.ReverseFrame,
                    BalanceMax = Settings.Instance.Common.BalanceMax,
                };


                inputDeps = job.Schedule(inputDeps);
                inputDeps.Complete();
                m_query.CopyFromComponentDataArray(statuses);
                m_query.CopyFromComponentDataArray(downStates);
                statuses.Dispose();
                downStates.Dispose();
            }


            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<Status> m_statuses;
            public NativeArray<DownState> m_downStates;
            [ReadOnly] public BattleSequencer m_seq;
            public int ReverseFrame;
            public int BalanceMax;

            public void Execute()
            {
                for (int i = 0; i < m_downStates.Length; i++)
                {

                    var status = m_statuses[i];
                    var downState = m_downStates[i];


                    if (downState.m_state == EnumDownState.None
                        && status.m_balance == 0)
                    {
                        downState.m_state = EnumDownState.Down;
                    }

                    if (downState.m_state != EnumDownState.None)
                    {
                        downState.m_count++;

                        switch (downState.m_state)
                        {
                            case EnumDownState.Down:
                                if (downState.m_count > ReverseFrame)
                                {
                                    downState.m_state = EnumDownState.Reverse;
                                    downState.m_count = 0;
                                }
                                break;
                            case EnumDownState.Reverse:
                                {
                                    YHAnimation anim = Shared.m_yhCharaAnimList.GetCommonAnim(EnumAnimationName._Down01);
                                    if (downState.m_count >= anim.m_length)
                                    {
                                        downState.m_state = EnumDownState.None;
                                        status.m_balance = BalanceMax;
                                    }
                                }
                                break;
                        }
                    }

                    m_statuses[i] = status;
                    m_downStates[i] = downState;
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
