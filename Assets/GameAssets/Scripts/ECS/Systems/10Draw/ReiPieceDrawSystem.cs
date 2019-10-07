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
            DrawToukiMeter();
        }


        private void DrawToukiMeter()
        {
            var reiPieces = m_query.ToComponentDataArray<ReiPiece>(Allocator.TempJob);

            for (int i = 0; i < reiPieces.Length; i++)
            {
                Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiPiece].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiPiece].GetMaterial(i);
                var reiPiece = reiPieces[i];
                int layer = (int)EnumDrawLayer.OverFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(i * 16, Settings.Instance.DrawPos.ReiPoolY + i * 16, layer),
                    m_Quaternion,
                    Vector3.one);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }

            reiPieces.Dispose();
        }


    }
}
