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
            for (int i = 0; i < Shared.yhFilterEffectList.effects.Count; i++)
            {
                yhFilterEffects[i] = Shared.yhFilterEffectList.effects[i].data;
            }

            var uv = Shared.yhFilterEffectList.effects[0];
            var job = new CountJob()
            {
                filterEffects = filterEffects,
                yhFilterEffects = yhFilterEffects,
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
            public NativeArray<FilterEffect> filterEffects;
            [ReadOnly] public NativeArray<YHFilterEffect> yhFilterEffects;

            public void Execute()
            {
                for (int i = 0; i < filterEffects.Length; i++)
                {
                    var item = filterEffects[i];
                    if (!item.isActive)
                        continue;

                    item.count++;


                    // TODO:終わりのタイミングここで
                    // item.c

                    // filterEffect.count



                    // Debug.Log(SpriteUr.imageName);
                    filterEffects[i] = item;
                }
            }
        }
    }
}
