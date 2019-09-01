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
            m_query.AddDependency(inputDeps);

            NativeArray<PadScan> padScans = m_query.ToComponentDataArray<PadScan>(Allocator.TempJob);
            NativeArray<ToukiMeter> toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var job = new InputToToukiJob()
            {
                padScans = padScans,
                toukiMeters = toukiMeters,
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
            public NativeArray<ToukiMeter> toukiMeters;
            [ReadOnly] public NativeArray<PadScan> padScans;

            public void Execute()
            {
                for (int i = 0; i < padScans.Length; i++)
                {
                    var toukiMeter = toukiMeters[i];
                    if (toukiMeter.state != EnumToukiMaterState.Active)
                        break;


                    if (toukiMeter.muki != padScans[i].GetPressCross())
                    {
                        toukiMeter.muki = padScans[i].GetPressCross();
                        toukiMeter.value = 0;
                    }

                    toukiMeters[i] = toukiMeter;
                }
            }

        }

    }
}
