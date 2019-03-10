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
        Quaternion m_quaternion;

        protected override void OnCreateManager()
        {
            m_group = GetComponentGroup(
                ComponentType.ReadOnly<ToukiMeter>()
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_group.AddDependency(inputDeps);

            var toukiMeters = m_group.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var toukiMeterMatrixs = new NativeArray<Matrix4x4>(toukiMeters.Length, Allocator.TempJob);
            ToukiMeterJob toukiMeterJob = DoToukiMeterJob(ref inputDeps, toukiMeters, toukiMeterMatrixs);

            DrawFrame();
            DrawToukiMeter(toukiMeterJob);

            // NativeArrayの開放
            toukiMeterMatrixs.Dispose();

            return inputDeps;
        }

        private ToukiMeterJob DoToukiMeterJob(ref JobHandle inputDeps, NativeArray<ToukiMeter> toukiMeters, NativeArray<Matrix4x4> toukiMeterMatrixs)
        {
            var toukiMeterJob = new ToukiMeterJob()
            {
                toukiMeters = toukiMeters,
                toukiMeterMatrixs = toukiMeterMatrixs,
                q = m_quaternion,
            };
            inputDeps = toukiMeterJob.Schedule(inputDeps);
            m_group.AddDependency(inputDeps);
            inputDeps.Complete();
            return toukiMeterJob;
        }

        private void DrawToukiMeter(ToukiMeterJob toukiMeterJob)
        {
            for (int i = 0; i < toukiMeterJob.toukiMeterMatrixs.Length; i++)
            {
                Graphics.DrawMesh(Shared.bgFrameMeshMat.meshs["meter02"],
                    toukiMeterJob.toukiMeterMatrixs[i],
                    Shared.bgFrameMeshMat.material, 0);
            }
        }

        private void DrawFrame()
        {
            Matrix4x4 bgFrameMatrix = Matrix4x4.TRS(new Vector3(0, 0, 1), m_quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.bgFrameMeshMat.meshs["frame"],
                bgFrameMatrix,
                Shared.bgFrameMeshMat.material, 0);
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