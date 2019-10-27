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
            m_query.AddDependency(inputDeps);
            NativeArray<Status> statuses = m_query.ToComponentDataArray<Status>(Allocator.TempJob);
            NativeArray<DownState> downStates = m_query.ToComponentDataArray<DownState>(Allocator.TempJob);

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

                    switch (m_seq.m_seqState)
                    {
                        case EnumBattleSequenceState.Idle:
                        case EnumBattleSequenceState.Start:
                            UpdateIdle(ref status, ref downState);
                            break;
                        case EnumBattleSequenceState.Play:
                            UpdatePlay(i, ref status, ref downState);
                            break;
                    }

                    m_statuses[i] = status;
                    m_downStates[i] = downState;
                }
            }

            private void UpdateIdle(ref Status status, ref DownState downState)
            {
                if (downState.m_state == EnumDownState.None
                    && status.m_balance == 0)
                {
                    downState.m_state = EnumDownState.Down;
                    downState.m_count = 0;
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
                                status.m_balance = BalanceMax;
                            }
                            break;
                        case EnumDownState.Reverse:
                            {
                                YHAnimation anim = Shared.m_yhCharaAnimList.GetCommonAnim(EnumAnimationName._Down01);
                                if (downState.m_count >= anim.m_length)
                                {
                                    downState.m_state = EnumDownState.None;
                                }
                            }
                            break;
                    }
                }
            }

            private void UpdatePlay(int i, ref Status status, ref DownState downState)
            {
                switch (downState.m_state)
                {
                    case EnumDownState.Reverse:
                        downState.m_state = EnumDownState.None;
                        break;
                    case EnumDownState.Down:
                        bool isReverse = (SideUtil.IsSideA(i))
                            ? m_seq.m_sideA.m_isReverse
                            : m_seq.m_sideB.m_isReverse;

                        if (isReverse)
                        {
                            downState.m_state = EnumDownState.None;
                            downState.m_count = 0;
                            status.m_balance = BalanceMax;
                        }
                        break;
                }
            }
        }
    }
}
