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
    public class ReiPieceDrawSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<ReiPiece>()
            );
        }

        protected override void OnUpdate()
        {
            DrawReiPiece();
        }


        private void DrawReiPiece()
        {
            var reiPieces = m_query.ToComponentDataArray<ReiPiece>(Allocator.TempJob);

            for (int i = 0; i < reiPieces.Length; i++)
            {
                var reiPiece = reiPieces[i];
                int layer = (int)EnumDrawLayer.OverFrame;
                int posX = reiPiece.m_basePos.x + reiPiece.m_movePos.x;
                int posY = reiPiece.m_basePos.y + reiPiece.m_movePos.y + Settings.Instance.DrawPos.ReiPoolY;

                if (reiPiece.m_reiState == EnumReiState.Idle
                    || reiPiece.m_reiState == EnumReiState.Wait
                    || reiPiece.m_reiState == EnumReiState.Born)
                {
                    Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiPiece].m_mesh;
                    Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiPiece].GetMaterial(i);
                    Matrix4x4 matrixes = Matrix4x4.TRS(new Vector3(posX, posY, layer), m_Quaternion, Vector3.one);
                    float alpha = 1f;
                    switch (reiPiece.m_reiState)
                    {
                        case EnumReiState.Wait:
                            alpha = (1f - ((float)reiPiece.m_count / (float)Settings.Instance.Animation.ReiWaitFrame));
                            break;
                        case EnumReiState.Born:
                            alpha = ((float)reiPiece.m_count / (float)Settings.Instance.Animation.ReiWaitFrame);
                            break;
                    }
                    mat.SetFloat(EnumShaderParam._Alpha.ToString(), Mathf.Clamp01(alpha));
                    Graphics.DrawMesh(mesh, matrixes, mat, 0);
                }

                if (reiPiece.m_reiState == EnumReiState.Distribute
                    || reiPiece.m_reiState == EnumReiState.Wait)
                {
                    Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiPieceDistribute].m_mesh;
                    Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiPieceDistribute].GetMaterial(i);
                    Matrix4x4 matrixes = Matrix4x4.TRS(new Vector3(posX, posY, layer), m_Quaternion, Vector3.one);
                    float alpha = (reiPiece.m_reiState == EnumReiState.Wait)
                    ? ((float)reiPiece.m_count / (float)Settings.Instance.Animation.ReiWaitFrame)
                    : 1;
                    mat.SetFloat(EnumShaderParam._Alpha.ToString(), Mathf.Clamp01(alpha));
                    Graphics.DrawMesh(mesh, matrixes, mat, 0);
                }
            }

            reiPieces.Dispose();
        }
    }
}
