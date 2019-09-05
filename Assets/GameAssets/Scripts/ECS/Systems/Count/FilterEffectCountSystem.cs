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
    public class FilterEffectCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<FilterEffect>()
            );

        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_query.AddDependency(inputDeps);

            NativeArray<FilterEffect> filterEffects = m_query.ToComponentDataArray<FilterEffect>(Allocator.TempJob);
            NativeArray<YHFilterEffect> yhFilterEffects
                = new NativeArray<YHFilterEffect>(Settings.Instance.Common.FilterEffectCount, Allocator.TempJob);
            for (int i = 0; i < Shared.m_yhFilterEffectList.m_effects.Count; i++)
            {
                yhFilterEffects[i] = Shared.m_yhFilterEffectList.m_effects[i].data;
            }

            var uv = Shared.m_yhFilterEffectList.m_effects[0];
            var job = new CountJob()
            {
                m_filterEffects = filterEffects,
                m_yhFilterEffects = yhFilterEffects,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(filterEffects);

            filterEffects.Dispose();
            yhFilterEffects.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<FilterEffect> m_filterEffects;
            [ReadOnly] public NativeArray<YHFilterEffect> m_yhFilterEffects;

            public void Execute()
            {
                for (int i = 0; i < m_filterEffects.Length; i++)
                {
                    var item = m_filterEffects[i];
                    if (!item.m_isActive)
                        continue;

                    item.m_count++;


                    // TODO:終わりのタイミングここで
                    // item.c

                    // filterEffect.count



                    // Debug.Log(SpriteUr.imageName);
                    m_filterEffects[i] = item;
                }
            }
        }
    }
}
