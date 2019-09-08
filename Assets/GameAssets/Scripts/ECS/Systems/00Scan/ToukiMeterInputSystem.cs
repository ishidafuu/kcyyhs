using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(ScanGroup))]
    public class ToukiMeterInputSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<PadScan>(),
                ComponentType.ReadWrite<ToukiMeter>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            m_query.AddDependency(inputDeps);

            NativeArray<PadScan> padScans = m_query.ToComponentDataArray<PadScan>(Allocator.TempJob);
            NativeArray<ToukiMeter> toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var job = new InputToToukiJob()
            {
                m_padScans = padScans,
                m_toukiMeters = toukiMeters,
                m_seq = seq,
            };
            inputDeps = job.Schedule(inputDeps);

            // m_query.AddDependency(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(toukiMeters);

            padScans.Dispose();
            toukiMeters.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct InputToToukiJob : IJob
        {
            public NativeArray<ToukiMeter> m_toukiMeters;
            [ReadOnly] public NativeArray<PadScan> m_padScans;
            [ReadOnly] public BattleSequencer m_seq;


            public void Execute()
            {
                for (int i = 0; i < m_padScans.Length; i++)
                {
                    var toukiMeter = m_toukiMeters[i];
                    var padScan = m_padScans[i];

                    if (i == 0)
                    {
                        if (m_seq.m_sideA.m_animStep != EnumAnimationStep.Sleep)
                            continue;
                    }
                    else
                    {
                        if (m_seq.m_sideB.m_animStep != EnumAnimationStep.Sleep)
                            continue;
                    }

                    if (toukiMeter.m_cross != padScan.GetPressCross())
                    {
                        toukiMeter.m_cross = padScan.GetPressCross();
                        toukiMeter.m_value = 0;
                        toukiMeter.m_count = 0;
                    }

                    m_toukiMeters[i] = toukiMeter;
                }
            }

        }

    }
}
