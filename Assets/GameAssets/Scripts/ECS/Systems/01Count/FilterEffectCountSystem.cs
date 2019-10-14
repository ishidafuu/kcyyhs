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
    [UpdateAfter(typeof(BattleSequencerCountSystem))]
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

            var job = new CountJob()
            {
                m_filterEffects = filterEffects,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(filterEffects);

            UpdateShaderFrame(filterEffects);

            filterEffects.Dispose();
            return inputDeps;
        }

        private static void UpdateShaderFrame(NativeArray<FilterEffect> filterEffects)
        {
            for (int i = 0; i < filterEffects.Length; i++)
            {
                var item = filterEffects[i];

                if (!item.m_isActive)
                    continue;

                MeshMat meshMat;
                switch (item.m_effectType)
                {
                    case EnumEffectType.EffectScreen:
                        meshMat = Shared.m_effectMeshMatList.m_effectScreenList[item.m_effectIndex];
                        break;
                    case EnumEffectType.EffectLarge:
                        meshMat = Shared.m_effectMeshMatList.m_effectLargeList[item.m_effectIndex];
                        break;
                    case EnumEffectType.EffectMedium:
                        meshMat = Shared.m_effectMeshMatList.m_effectMediumList[item.m_effectIndex];
                        break;
                    case EnumEffectType.EffectSmall:
                        meshMat = Shared.m_effectMeshMatList.m_effectSmallList[item.m_effectIndex];
                        break;
                    case EnumEffectType.FillterScreen:
                        meshMat = Shared.m_effectMeshMatList.m_filterScreenList[item.m_effectIndex];
                        break;
                    case EnumEffectType.EffectBG:
                        meshMat = Shared.m_effectMeshMatList.m_effectBGList[item.m_effectIndex];
                        break;
                    case EnumEffectType.EffectDamageBody:
                    case EnumEffectType.EffectDamageFace:
                        meshMat = Shared.m_effectMeshMatList.m_effectLargeList[(int)EnumEffectLarge.Damage];
                        break;
                    default:
                        continue;
                }

                meshMat.GetMaterial().SetInt("_Frame", item.m_count);
            }
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<FilterEffect> m_filterEffects;
            // [ReadOnly] public NativeArray<YHFilterEffect> m_yhFilterEffects;

            public void Execute()
            {
                for (int i = 0; i < m_filterEffects.Length; i++)
                {
                    var item = m_filterEffects[i];

                    if (!item.m_isActive)
                        continue;

                    item.m_count++;
                    // TODO:終わりのタイミングここで
                    // Debug.Log(SpriteUr.imageName);
                    m_filterEffects[i] = item;
                }
            }
        }
    }
}
