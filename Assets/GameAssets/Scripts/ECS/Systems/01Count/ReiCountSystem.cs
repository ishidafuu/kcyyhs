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
    public class ReiCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<ReiState>(),
                ComponentType.ReadWrite<Status>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Settings.Instance.Debug.IsSkip())
                return inputDeps;

            BattleSequencer seq = GetSingleton<BattleSequencer>();


            m_query.AddDependency(inputDeps);
            NativeArray<ReiState> reiStates = m_query.ToComponentDataArray<ReiState>(Allocator.TempJob);
            NativeArray<Status> statuses = m_query.ToComponentDataArray<Status>(Allocator.TempJob);


            var job = new CountJob()
            {
                m_reiStates = reiStates,
                m_statuses = statuses,
                m_seq = seq,
                MaxRei = Settings.Instance.Common.ReiMax,
                IsDebugHeal = Settings.Instance.Debug.IsHeal,
                AmountSpeed = Settings.Instance.Common.ReiAmountSpeed,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(reiStates);
            m_query.CopyFromComponentDataArray(statuses);
            reiStates.Dispose();
            statuses.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<ReiState> m_reiStates;
            public NativeArray<Status> m_statuses;
            [ReadOnly] public BattleSequencer m_seq;
            public int MaxRei;
            public bool IsDebugHeal;
            public int AmountSpeed;

            public void Execute()
            {
                for (int i = 0; i < m_reiStates.Length; i++)
                {
                    var reiState = m_reiStates[i];
                    var status = m_statuses[i];
                    if (m_seq.m_seqState == EnumBattleSequenceState.Idle)
                    {
                        if (!IsDebugHeal)
                            continue;

                        if (status.m_rei < MaxRei)
                        {
                            status.m_rei += AmountSpeed;
                            if (status.m_rei > MaxRei)
                            {
                                status.m_rei = MaxRei;
                            }
                            m_statuses[i] = status;
                        }

                    }
                    else
                    {
                        if (SideUtil.IsSideA(i))
                        {
                            if (!m_seq.m_sideA.m_isConsumeRei)
                                continue;
                        }
                        else
                        {
                            if (!m_seq.m_sideB.m_isConsumeRei)
                                continue;
                        }

                        if (reiState.m_reiAmount > 0)
                        {
                            reiState.m_reiAmount -= AmountSpeed;
                            status.m_rei -= AmountSpeed;
                            if (status.m_rei < 0)
                            {
                                status.m_rei = 0;
                            }
                        }

                        m_statuses[i] = status;
                        m_reiStates[i] = reiState;
                    }

                }
            }
        }
    }
}
