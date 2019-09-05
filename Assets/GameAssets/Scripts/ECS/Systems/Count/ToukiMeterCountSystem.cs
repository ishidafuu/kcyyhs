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
    public class ToukiMeterCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<ToukiMeter>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();
            if (seq.m_isPlay)
                return inputDeps;

            m_query.AddDependency(inputDeps);

            NativeArray<ToukiMeter> toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            Vector2[] uv = Shared.m_bgFrameMeshMat.m_meshDict[EnumBGPartsType.bg00.ToString()].uv;
            var job = new CountToukiJob()
            {
                m_toukiMeters = toukiMeters,
                BgScrollRange = Settings.Instance.DrawPos.BgWidth << (Settings.Instance.DrawPos.BgScrollRangeFactor - 1),
                ToukiWidth = Settings.Instance.DrawPos.ToukiWidth,
                SpriteUl = uv[0].x,
                SpriteUr = uv[1].x,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(toukiMeters);
            toukiMeters.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountToukiJob : IJob
        {
            public NativeArray<ToukiMeter> m_toukiMeters;
            [ReadOnly] public int BgScrollRange;
            [ReadOnly] public int ToukiWidth;
            [ReadOnly] public float SpriteUl;
            [ReadOnly] public float SpriteUr;

            public void Execute()
            {
                float width = (SpriteUr - SpriteUl) / 2;

                for (int i = 0; i < m_toukiMeters.Length; i++)
                {
                    var toukiMeter = m_toukiMeters[i];
                    if (toukiMeter.m_muki != EnumCrossType.None)
                    {
                        toukiMeter.m_value++;
                        if (toukiMeter.m_value > ToukiWidth)
                        {
                            toukiMeter.m_value = ToukiWidth;
                        }
                    }

                    // 背景スクロール
                    switch (toukiMeter.m_muki)
                    {
                        case EnumCrossType.Left:
                        case EnumCrossType.Down:

                            toukiMeter.m_bgScroll--;
                            if (toukiMeter.m_bgScroll < 0)
                            {
                                toukiMeter.m_bgScroll = BgScrollRange;
                            }
                            break;
                        case EnumCrossType.Right:
                            toukiMeter.m_bgScroll++;
                            if (toukiMeter.m_bgScroll > BgScrollRange)
                            {
                                toukiMeter.m_bgScroll = 0;
                            }
                            break;
                    }

                    float u = (float)toukiMeter.m_bgScroll / (float)BgScrollRange;
                    toukiMeter.m_bgScrollTextureUL = SpriteUl + (u * width);
                    toukiMeter.m_bgScrollTextureUR = toukiMeter.m_bgScrollTextureUL + width;
                    m_toukiMeters[i] = toukiMeter;
                }
            }
        }
    }
}
