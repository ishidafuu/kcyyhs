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
    public class BGDrawSystem : JobComponentSystem
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
                toukiMeters = toukiMeters,
                toukiMeterMatrixes = toukiMeterMatrixes,
                q = m_quaternion,
                ToukiMeterX = Settings.Instance.DrawPos.ToukiMeterX,
                ToukiMeterY = Settings.Instance.DrawPos.ToukiMeterY,
            };
            inputDeps = toukiMeterJob.Schedule(inputDeps);
            // var bgScrolls = m_group.ToComponentDataArray<BgScroll>(Allocator.TempJob);
            // var bgScrollsMatrixes = new NativeArray<Matrix4x4>(toukiMeters.Length, Allocator.TempJob);
            // BgScrollJob bgScrollJob = DoBgScrollJob(ref inputDeps, toukiMeters, bgScrollsMatrixes);

            inputDeps.Complete();
            // m_query.CopyFromComponentDataArray(toukiMeters);

            DrawBgScroll(toukiMeters);
            DrawFrame();
            DrawToukiMeter(toukiMeterJob);

            toukiMeters.Dispose();
            toukiMeterMatrixes.Dispose();

            return inputDeps;
        }

        private ToukiMeterJob DoToukiMeterJob(ref JobHandle inputDeps, NativeArray<ToukiMeter> toukiMeters, NativeArray<Matrix4x4> toukiMeterMatrixes)
        {
            var toukiMeterJob = new ToukiMeterJob()
            {
                toukiMeters = toukiMeters,
                toukiMeterMatrixes = toukiMeterMatrixes,
                q = m_quaternion,
                ToukiMeterX = Settings.Instance.DrawPos.ToukiMeterX,
                ToukiMeterY = Settings.Instance.DrawPos.ToukiMeterY,
            };
            inputDeps = toukiMeterJob.Schedule(inputDeps);
            m_query.AddDependency(inputDeps);
            return toukiMeterJob;
        }

        // private BgScrollJob DoBgScrollJob(ref JobHandle inputDeps, NativeArray<ToukiMeter> toukiMeters, NativeArray<Matrix4x4> bgScrollMatrixs)
        // {
        //     var bgScrollJob = new BgScrollJob()
        //     {
        //         toukiMeters = toukiMeters,
        //         bgScrollMatrixes = bgScrollMatrixes,
        //         q = m_quaternion,
        //         BgScrollWidth = Settings.Instance.DrawPos.BgScrollWidth,
        //         BgScrollRangeFactor = Settings.Instance.DrawPos.BgScrollRangeFactor,
        //         BgScrollX = -Settings.Instance.DrawPos.BgScrollX,
        //         BgScrollY = Settings.Instance.DrawPos.BgScrollY,
        //     };
        //     inputDeps = bgScrollJob.Schedule(inputDeps);
        //     m_group.AddDependency(inputDeps);
        //     return bgScrollJob;
        // }

        private void DrawBgScroll(NativeArray<ToukiMeter> toukiMeters)
        {
            for (int i = 0; i < toukiMeters.Length; i++)
            {
                Matrix4x4 bgScrollMatrixes = Matrix4x4.TRS(new Vector3(-Settings.Instance.DrawPos.BgScrollX,
                        Settings.Instance.DrawPos.BgScrollY, 0),
                    m_quaternion, new Vector3(0.5f, 1, 1));

                Mesh baseMesh = Shared.bgFrameMeshMat.meshDict[EnumBGPartsType.bg00.ToString()];
                Mesh mesh = new Mesh()
                {
                    vertices = baseMesh.vertices,
                    uv = new Vector2[]
                    {
                    new Vector2(toukiMeters[i].textureUl, baseMesh.uv[0].y),
                    new Vector2(toukiMeters[i].textureUr, baseMesh.uv[1].y),
                    new Vector2(toukiMeters[i].textureUl, baseMesh.uv[2].y),
                    new Vector2(toukiMeters[i].textureUr, baseMesh.uv[3].y),
                    },
                    triangles = baseMesh.triangles,
                };
                Graphics.DrawMesh(mesh,
                    bgScrollMatrixes,
                    Shared.bgFrameMeshMat.material, 0);
            }
        }

        private void DrawFrame()
        {
            Matrix4x4 frameMatrix = Matrix4x4.TRS(new Vector3(0, 0, 0), m_quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.commonMeshMat.meshDict[EnumBGPartsType.frame_bottom.ToString()],
                frameMatrix,
                Shared.commonMeshMat.material, 0);
        }

        private void DrawToukiMeter(ToukiMeterJob toukiMeterJob)
        {
            for (int i = 0; i < toukiMeterJob.toukiMeterMatrixes.Length; i++)
            {
                Graphics.DrawMesh(Shared.commonMeshMat.meshDict[EnumBGPartsType.meter02.ToString()],
                    toukiMeterJob.toukiMeterMatrixes[i],
                    Shared.commonMeshMat.material, 0);
            }
        }

        // [BurstCompileAttribute]
        struct ToukiMeterJob : IJob
        {
            public NativeArray<Matrix4x4> toukiMeterMatrixes;
            [ReadOnly] public NativeArray<ToukiMeter> toukiMeters;
            [ReadOnly] public Quaternion q;
            [ReadOnly] public int ToukiMeterX;
            [ReadOnly] public int ToukiMeterY;

            public void Execute()
            {
                for (int i = 0; i < toukiMeters.Length; i++)
                {
                    var width = (float)toukiMeters[i].value / 100;
                    Matrix4x4 tmpMatrix = Matrix4x4.TRS(
                        new Vector3(ToukiMeterX, ToukiMeterY, 0),
                        q, new Vector3(width, 1, 1));

                    toukiMeterMatrixes[i] = tmpMatrix;
                }
            }
        }

        // [BurstCompileAttribute]
        // struct BgScrollJob : IJob
        // {
        //     [ReadOnly]
        //     // [DeallocateOnJobCompletion]
        //     public NativeArray<ToukiMeter> toukiMeters;
        //     [ReadOnly]
        //     public Quaternion q;
        //     [ReadOnly]
        //     public int BgScrollWidth;
        //     [ReadOnly]
        //     public int BgScrollRangeFactor;
        //     [ReadOnly]
        //     public int BgScrollX;
        //     [ReadOnly]
        //     public int BgScrollY;
        //     public NativeArray<Matrix4x4> bgScrollMatrixes;

        //     public void Execute()
        //     {
        //         for (int i = 0; i < toukiMeters.Length; i++)
        //         {
        //             // var index = i * 2;
        //             var posX = BgScrollX + (toukiMeters[i].bgScroll >> BgScrollRangeFactor);
        //             var posX2 = (posX - BgScrollWidth);

        //             Matrix4x4 tmpMatrix = Matrix4x4.TRS(new Vector3(posX, BgScrollY, 0),
        //                 q, Vector3.one);

        //             bgScrollMatrixes[i] = tmpMatrix;

        //             // Matrix4x4 tmpMatrix2 = Matrix4x4.TRS(new Vector3(posX2, BgScrollY, 0),
        //             //     q, Vector3.one);

        //             // bgScrollMatrixs[index + 1] = tmpMatrix2;
        //         }
        //     }
        // }

    }
}
