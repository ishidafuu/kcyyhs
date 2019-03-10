using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    [UpdateAfter(typeof(CountGroup))]
    [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
    public class BGDrawSystem : JobComponentSystem
    {
        ComponentGroup m_group;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.ReadOnly<ToukiMeter>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_group.AddDependency(inputDeps);

            var toukiMeters = m_group.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);

            var length = toukiMeters.Length;

            var toukiMeterMatrixs = new NativeArray<Matrix4x4>(length, Allocator.TempJob);

            var quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
            var toukiMeterJob = new ToukiMeterJob()
            {
                toukiMeters = toukiMeters,
                toukiMeterMatrixs = toukiMeterMatrixs,
                q = quaternion,
            };
            // m_group.AddDependency(inputDeps);
            inputDeps = toukiMeterJob.Schedule(inputDeps);
            m_group.AddDependency(inputDeps);
            inputDeps.Complete();

            Matrix4x4 bgFrameMatrix = Matrix4x4.TRS(new Vector3(0, 0, 1), quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.bgFrameMeshMat.meshs[0],
                bgFrameMatrix,
                Shared.bgFrameMeshMat.materials[0], 0);

            for (int i = 0; i < toukiMeterJob.toukiMeterMatrixs.Length; i++)
            {
                var framesCount = Shared.charaMeshMat;
                Graphics.DrawMesh(Shared.meterMeshMat.meshs[1],
                    toukiMeterJob.toukiMeterMatrixs[i],
                    Shared.meterMeshMat.materials[0], 0);
            }

            // NativeArrayの開放
            toukiMeterMatrixs.Dispose();

            return inputDeps;
        }

        [BurstCompileAttribute]
        struct ToukiMeterJob : IJob
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<ToukiMeter> toukiMeters;
            [ReadOnly]
            public Quaternion q;
            public NativeArray<Matrix4x4> toukiMeterMatrixs;

            public void Execute()
            {
                for (int i = 0; i < toukiMeters.Length; i++)
                {
                    // var toukiMetersMatrix = toukiMeterMatrixs[i];

                    var width = (float)toukiMeters[i].value / 100;

                    Matrix4x4 tmpMatrix = Matrix4x4.TRS(new Vector3(-128 + 8, -35, 0),
                        q, new Vector3(width, 1, 1));

                    toukiMeterMatrixs[i] = tmpMatrix;
                }
            }
        }

    }
}