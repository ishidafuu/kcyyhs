﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class ToukiMeterCountJobSystem : JobComponentSystem
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
            m_query.AddDependency(inputDeps);

            NativeArray<ToukiMeter> toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            Vector2[] uv = Shared.bgFrameMeshMat.meshDict[EnumBGPartsType.bg00.ToString()].uv;
            var job = new CountToukiJob()
            {
                toukiMeters = toukiMeters,
                BgScrollRange = Settings.Instance.DrawPos.BgScrollWidth << Settings.Instance.DrawPos.BgScrollRangeFactor,
                spriteUl = uv[0].x,
                spriteUr = uv[1].x,
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
            public NativeArray<ToukiMeter> toukiMeters;
            [ReadOnly] public int BgScrollRange;
            [ReadOnly] public float spriteUl;
            [ReadOnly] public float spriteUr;

            public void Execute()
            {
                float width = (spriteUr - spriteUl) / 2;

                for (int i = 0; i < toukiMeters.Length; i++)
                {
                    var toukiMeter = toukiMeters[i];
                    if (toukiMeter.muki != EnumCrossType.None)
                    {
                        toukiMeter.value++;
                        if (toukiMeter.value > 100)
                        {
                            toukiMeter.value = 100;
                        }
                    }

                    // 背景スクロール
                    switch (toukiMeter.muki)
                    {
                        case EnumCrossType.Left:
                        case EnumCrossType.Down:

                            toukiMeter.bgScroll--;
                            if (toukiMeter.bgScroll < 0)
                            {
                                toukiMeter.bgScroll = BgScrollRange;
                            }
                            break;
                        case EnumCrossType.Right:
                            toukiMeter.bgScroll++;
                            if (toukiMeter.bgScroll > BgScrollRange)
                            {
                                toukiMeter.bgScroll = 0;
                            }
                            break;
                    }

                    float u = (float)toukiMeter.bgScroll / (float)BgScrollRange;
                    toukiMeter.textureUl = spriteUl + (u * width);
                    toukiMeter.textureUr = toukiMeter.textureUl + width;
                    toukiMeters[i] = toukiMeter;
                }
            }
        }
    }
}
