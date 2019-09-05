using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    public class MeterDrawSystem : JobComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_quaternion;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<ToukiMeter>()
            // ComponentType.ReadOnly<BgScroll>()
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_query.AddDependency(inputDeps);

            var toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var toukiMeterMatrixes = new NativeArray<Matrix4x4>(toukiMeters.Length, Allocator.TempJob);

            var toukiMeterJob = new ToukiMeterJob()
            {
                m_toukiMeters = toukiMeters,
                m_toukiMeterMatrixes = toukiMeterMatrixes,
                Q = m_quaternion,
                ToukiWidth = Settings.Instance.DrawPos.ToukiWidth,
                ToukiMeterX = Settings.Instance.DrawPos.ToukiMeterX,
                ToukiMeterY = Settings.Instance.DrawPos.ToukiMeterY,
            };
            inputDeps = toukiMeterJob.Schedule(inputDeps);

            inputDeps.Complete();

            DrawToukiMeter(toukiMeterJob);

            toukiMeters.Dispose();
            toukiMeterMatrixes.Dispose();

            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct ToukiMeterJob : IJob
        {
            public NativeArray<Matrix4x4> m_toukiMeterMatrixes;
            [ReadOnly] public NativeArray<ToukiMeter> m_toukiMeters;
            [ReadOnly] public Quaternion Q;
            [ReadOnly] public int ToukiWidth;
            [ReadOnly] public int ToukiMeterX;
            [ReadOnly] public int ToukiMeterY;

            public void Execute()
            {
                for (int i = 0; i < m_toukiMeters.Length; i++)
                {
                    float width = (float)m_toukiMeters[i].m_value / (float)ToukiWidth;

                    float posX = (i == 0)
                        ? ToukiMeterX + ((float)m_toukiMeters[i].m_value / 2f)
                        : -ToukiMeterX - ((float)m_toukiMeters[i].m_value / 2f);

                    Matrix4x4 tmpMatrix = Matrix4x4.TRS(
                        new Vector3(posX, ToukiMeterY, (int)EnumDrawLayer.OverFrame),
                        Q, new Vector3(width, 1, 1));

                    m_toukiMeterMatrixes[i] = tmpMatrix;
                }
            }
        }

        private void DrawToukiMeter(ToukiMeterJob toukiMeterJob)
        {
            string meter02 = EnumCommonPartsType.meter02.ToString();
            for (int i = 0; i < toukiMeterJob.m_toukiMeterMatrixes.Length; i++)
            {

                Graphics.DrawMesh(
                    Shared.m_commonMeshMat.m_meshDict[meter02],
                    toukiMeterJob.m_toukiMeterMatrixes[i],
                    Shared.m_commonMeshMat.m_materialDict[meter02], 0);
            }
        }
    }
}
