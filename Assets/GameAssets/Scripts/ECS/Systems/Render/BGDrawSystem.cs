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
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_query.AddDependency(inputDeps);

            // TODO:攻撃アニメーション状態に入ったら、処理を変える
            var toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            DrawBackGround(toukiMeters);
            DrawFrame();

            toukiMeters.Dispose();

            return inputDeps;
        }


        private void DrawBackGround(NativeArray<ToukiMeter> toukiMeters)
        {
            for (int i = 0; i < toukiMeters.Length; i++)
            {
                Matrix4x4 bgScrollMatrixes = Matrix4x4.TRS(new Vector3(-Settings.Instance.DrawPos.BgScrollX,
                        Settings.Instance.DrawPos.BgScrollY, (int)EnumDrawLayer.BackGround),
                    m_quaternion, new Vector3(0.5f, 1, 1));

                Mesh baseMesh = Shared.bgFrameMeshMat.meshDict[EnumBGPartsType.bg00.ToString()];
                Mesh mesh = new Mesh()
                {
                    vertices = baseMesh.vertices,
                    uv = new Vector2[]
                    {
                    new Vector2(toukiMeters[i].bgScrollTextureUL, baseMesh.uv[0].y),
                    new Vector2(toukiMeters[i].bgScrollTextureUR, baseMesh.uv[1].y),
                    new Vector2(toukiMeters[i].bgScrollTextureUL, baseMesh.uv[2].y),
                    new Vector2(toukiMeters[i].bgScrollTextureUR, baseMesh.uv[3].y),
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
            Matrix4x4 frameTopMatrix = Matrix4x4.TRS(new Vector3(0, Settings.Instance.DrawPos.FrameTopY, (int)EnumDrawLayer.Frame),
                m_quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.commonMeshMat.meshDict[EnumBGPartsType.frame_top.ToString()],
                frameTopMatrix,
                Shared.commonMeshMat.material, 0);

            Matrix4x4 frameBottomMatrix = Matrix4x4.TRS(new Vector3(0, Settings.Instance.DrawPos.FrameBottomY, (int)EnumDrawLayer.Frame),
                m_quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.commonMeshMat.meshDict[EnumBGPartsType.frame_bottom.ToString()],
                frameBottomMatrix,
                Shared.commonMeshMat.material, 0);
        }
    }
}
