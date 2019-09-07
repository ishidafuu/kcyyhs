﻿using Unity.Burst;
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
        Quaternion m_quaternion;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<ToukiMeter>()
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override void OnUpdate()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (seq.m_isPlay)
                return;

            var toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            DrawSpritBackGround(toukiMeters);
            toukiMeters.Dispose();
        }


        private void DrawSpritBackGround(NativeArray<ToukiMeter> toukiMeters)
        {
            Mesh baseMesh = Shared.m_bgFrameMeshMat.m_meshDict[EnumBGPartsType.bg00.ToString()];
            Material mat = Shared.m_bgFrameMeshMat.m_materialDict[EnumBGPartsType.bg00.ToString()];
            for (int i = 0; i < toukiMeters.Length; i++)
            {
                Matrix4x4 bgScrollMatrixes = Matrix4x4.TRS(new Vector3(-Settings.Instance.DrawPos.BgScrollX,
                        Settings.Instance.DrawPos.BgScrollY, (int)EnumDrawLayer.BackGround),
                    m_quaternion, new Vector3(0.5f, 1, 1));

                Mesh mesh = new Mesh()
                {
                    vertices = baseMesh.vertices,
                    uv = new Vector2[]
                    {
                    new Vector2(toukiMeters[i].m_bgScrollTextureUL, baseMesh.uv[0].y),
                    new Vector2(toukiMeters[i].m_bgScrollTextureUR, baseMesh.uv[1].y),
                    new Vector2(toukiMeters[i].m_bgScrollTextureUL, baseMesh.uv[2].y),
                    new Vector2(toukiMeters[i].m_bgScrollTextureUR, baseMesh.uv[3].y),
                    },
                    triangles = baseMesh.triangles,
                };
                Graphics.DrawMesh(mesh, bgScrollMatrixes, mat, 0);
            }
        }



    }
}