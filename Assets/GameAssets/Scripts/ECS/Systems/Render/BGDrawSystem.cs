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
                Q = m_quaternion,
                ToukiWidth = Settings.Instance.DrawPos.ToukiWidth,
                ToukiMeterX = Settings.Instance.DrawPos.ToukiMeterX,
                ToukiMeterY = Settings.Instance.DrawPos.ToukiMeterY,
            };
            inputDeps = toukiMeterJob.Schedule(inputDeps);

            inputDeps.Complete();

            DrawBgScroll(toukiMeters);
            DrawFrame();
            DrawToukiMeter(toukiMeterJob);

            toukiMeters.Dispose();
            toukiMeterMatrixes.Dispose();

            return inputDeps;
        }


        // [BurstCompileAttribute]
        struct ToukiMeterJob : IJob
        {
            public NativeArray<Matrix4x4> toukiMeterMatrixes;
            [ReadOnly] public NativeArray<ToukiMeter> toukiMeters;
            [ReadOnly] public Quaternion Q;
            [ReadOnly] public int ToukiWidth;
            [ReadOnly] public int ToukiMeterX;
            [ReadOnly] public int ToukiMeterY;

            public void Execute()
            {
                for (int i = 0; i < toukiMeters.Length; i++)
                {
                    float width = (float)toukiMeters[i].value / (float)ToukiWidth;

                    float posX = (i == 0)
                    ? ToukiMeterX + ((float)toukiMeters[i].value / 2f)
                    : -ToukiMeterX - ((float)toukiMeters[i].value / 2f);

                    Matrix4x4 tmpMatrix = Matrix4x4.TRS(
                        new Vector3(posX, ToukiMeterY, 0),
                        Q, new Vector3(width, 1, 1));

                    toukiMeterMatrixes[i] = tmpMatrix;
                }
            }
        }

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
            Matrix4x4 frameTopMatrix = Matrix4x4.TRS(new Vector3(0, Settings.Instance.DrawPos.FrameTopY, 0), m_quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.commonMeshMat.meshDict[EnumBGPartsType.frame_top.ToString()],
                frameTopMatrix,
                Shared.commonMeshMat.material, 0);

            Matrix4x4 frameBottomMatrix = Matrix4x4.TRS(new Vector3(0, Settings.Instance.DrawPos.FrameBottomY, 0), m_quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.commonMeshMat.meshDict[EnumBGPartsType.frame_bottom.ToString()],
                frameBottomMatrix,
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



    }
}
