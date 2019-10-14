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
                        jumpState.m_count++;
                        jumpState.m_stepCount++;
                    }

                    m_jumpStates[i] = jumpState;
                }
            }
        }

        private void UpdateFilterEffect(ref JumpState jumpState)
        {
            EnumAnimationName animName = EnumAnimationName._Jump00;

            switch (jumpState.m_state)
            {
                case EnumJumpState.Jumping:
                    animName = EnumAnimationName._Jump00;
                    break;
                case EnumJumpState.Falling:
                    animName = EnumAnimationName._Jump01;
                    break;
            }

            // Jumpは共通アクションなのでcharaNoは実質使われない
            int commonCharaNo = 0;
            YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(commonCharaNo, animName);
            foreach (var item in anim.m_events)
            {
                if (item.m_frame != jumpState.m_count)
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
