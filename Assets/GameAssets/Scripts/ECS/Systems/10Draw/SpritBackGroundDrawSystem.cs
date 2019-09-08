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
    public class SpritBackGroundDrawSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        Vector3 m_Scale = new Vector3(0.5f, 1, 1);

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<ToukiMeter>(),
                ComponentType.ReadOnly<SideInfo>()
            );
        }

        protected override void OnUpdate()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (seq.m_isPlay)
                return;

            var toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var sideInfos = m_query.ToComponentDataArray<SideInfo>(Allocator.TempJob);

            DrawChara(toukiMeters, sideInfos);
            DrawSpritBackGround(toukiMeters);
            toukiMeters.Dispose();
            sideInfos.Dispose();
        }


        private void DrawChara(NativeArray<ToukiMeter> toukiMeters, NativeArray<SideInfo> sideInfos)
        {
            for (int i = 0; i < toukiMeters.Length; i++)
            {
                var toukiMeter = toukiMeters[i];
                var sideInfo = sideInfos[i];

                int charaNo = sideInfo.m_charaNo;

                int basePosX = (i == 0)
                    ? -Settings.Instance.DrawPos.BgScrollX
                    : +Settings.Instance.DrawPos.BgScrollX;

                EnumAnimationName animName = EnumAnimationName._Stand;

                if (toukiMeter.m_muki == EnumCrossType.Right && sideInfo.m_isSideA
                 || toukiMeter.m_muki == EnumCrossType.Left && !sideInfo.m_isSideA
                 || toukiMeter.m_muki == EnumCrossType.Down)
                {
                    animName = EnumAnimationName._Charge00;
                }
                else if (toukiMeter.m_muki == EnumCrossType.Right && !sideInfo.m_isSideA
                 || toukiMeter.m_muki == EnumCrossType.Left && sideInfo.m_isSideA
                 || toukiMeter.m_muki == EnumCrossType.Up)
                {
                    animName = EnumAnimationName._Charge01;
                }

                int count = toukiMeter.m_count;
                YHAnimationUtils.DrawYHAnimation(animName, charaNo, count, basePosX, sideInfo.m_isSideA);
            }
        }


        private void DrawSpritBackGround(NativeArray<ToukiMeter> toukiMeters)
        {
            string bgName = EnumBGPartsType.bg00.ToString();
            Mesh baseMesh = Shared.m_bgFrameMeshMat.m_meshDict[bgName];
            Material mat = Shared.m_bgFrameMeshMat.m_materialDict[bgName];

            for (int i = 0; i < toukiMeters.Length; i++)
            {
                var toukiMeter = toukiMeters[i];

                int posX = (i == 0)
                    ? -Settings.Instance.DrawPos.BgScrollX
                    : +Settings.Instance.DrawPos.BgScrollX;
                Vector3 pos = new Vector3(posX, Settings.Instance.DrawPos.BgScrollY,
                        (int)EnumDrawLayer.BackGround);

                Matrix4x4 bgScrollMatrixes = Matrix4x4.TRS(pos, m_Quaternion, m_Scale);

                Mesh mesh = new Mesh()
                {
                    vertices = baseMesh.vertices,
                    uv = new Vector2[]
                    {
                    new Vector2(toukiMeter.m_bgScrollTextureUL, baseMesh.uv[0].y),
                    new Vector2(toukiMeter.m_bgScrollTextureUR, baseMesh.uv[1].y),
                    new Vector2(toukiMeter.m_bgScrollTextureUL, baseMesh.uv[2].y),
                    new Vector2(toukiMeter.m_bgScrollTextureUR, baseMesh.uv[3].y),
                    },
                    triangles = baseMesh.triangles,
                };
                Graphics.DrawMesh(mesh, bgScrollMatrixes, mat, 0);
            }
        }
    }
}
