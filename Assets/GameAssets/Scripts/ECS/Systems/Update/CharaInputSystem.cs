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
    public class CharaInputSystem : JobComponentSystem
    {
        EntityQuery m_queryChara;
        EntityQuery m_queryBattle;

        protected override void OnCreate()
        {
            m_queryChara = GetEntityQuery(
                ComponentType.ReadOnly<PadScan>(),
                ComponentType.ReadOnly<ToukiMeter>()
            );

            m_queryBattle = GetEntityQuery(
                ComponentType.ReadWrite<BattleSequencer>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_queryChara.AddDependency(inputDeps);
            m_queryBattle.AddDependency(inputDeps);

            NativeArray<PadScan> padScans = m_queryChara.ToComponentDataArray<PadScan>(Allocator.TempJob);
            NativeArray<ToukiMeter> toukiMeters = m_queryChara.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            NativeArray<BattleSequencer> battleSequencers = m_queryBattle.ToComponentDataArray<BattleSequencer>(Allocator.TempJob);

            var job = new CharaInputJob()
            {
                padScans = padScans,
                toukiMeters = toukiMeters,
                battleSequencers = battleSequencers,
            };
            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();


            // m_query.AddDependency(inputDeps);
            m_queryBattle.CopyFromComponentDataArray(battleSequencers);

            padScans.Dispose();
            toukiMeters.Dispose();
            battleSequencers.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CharaInputJob : IJob
        {
            public NativeArray<ToukiMeter> toukiMeters;
            public NativeArray<BattleSequencer> battleSequencers;
            [ReadOnly] public NativeArray<PadScan> padScans;

            public void Execute()
            {
                var battleSequencer = battleSequencers[0];
                for (int i = 0; i < padScans.Length; i++)
                {
                    Debug.Log(battleSequencer.isPlay);
                    battleSequencer.isPlay = !battleSequencer.isPlay;
                    // var toukiMeter = toukiMeters[i];
                    // var a = true;
                    // var b = true;
                    // var c = true;
                    // var v = a || (b && c);
                    // if (toukiMeter.state != EnumToukiMaterState.Active)
                    // {
                    //     break;
                    // }

                    // if (toukiMeter.muki != padScans[i].GetPressCross())
                    // {
                    //     toukiMeter.muki = padScans[i].GetPressCross();
                    //     toukiMeter.value = 0;
                    // }

                    // toukiMeters[i] = toukiMeter;
                }
                battleSequencers[0] = battleSequencer;
            }

        }

    }
}
