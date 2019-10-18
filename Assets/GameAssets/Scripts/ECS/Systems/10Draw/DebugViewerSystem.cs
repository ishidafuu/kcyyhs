using Unity.Burst;
using Unity.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Experimental.PlayerLoop;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    [AlwaysUpdateSystem]
    public class DebugViewerSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        Quaternion m_QuaternionRev = Quaternion.Euler(new Vector3(-90, 180, 0));

        protected override void OnCreate()
        {
            // m_query = GetEntityQuery(
            //     ComponentType.ReadOnly<FilterEffect>()
            // );
        }

        protected override void OnUpdate()
        {
            if (Settings.Instance.Debug.IsShaderView)
            {
                DrawShaderGraphTest();
            }

            if (Settings.Instance.Debug.IsCharaView)
            {
                DrawChara();
            }
        }


        private void DrawShaderGraphTest()
        {
            List<MeshMat> listMeshMat = Shared.m_effectMeshMatList.m_effectBGList;
            int posX = 0;
            int layer = (int)EnumDrawLayer.OverChara;
            switch (Settings.Instance.Debug.ShaderType)
            {
                case EnumShaderType.EffectBG:
                    listMeshMat = Shared.m_effectMeshMatList.m_effectBGList;
                    layer = (int)EnumDrawLayer.OverBackGround;
                    break;
                case EnumShaderType.EffectLarge:
                    listMeshMat = Shared.m_effectMeshMatList.m_effectLargeList;
                    break;
                case EnumShaderType.EffectMedium:
                    listMeshMat = Shared.m_effectMeshMatList.m_effectMediumList;
                    break;
                case EnumShaderType.EffectScreen:
                    listMeshMat = Shared.m_effectMeshMatList.m_effectScreenList;
                    break;
                case EnumShaderType.EffectSmall:
                    listMeshMat = Shared.m_effectMeshMatList.m_effectSmallList;
                    break;
                case EnumShaderType.FilterScreen:
                    listMeshMat = Shared.m_effectMeshMatList.m_filterScreenList;
                    break;
                case EnumShaderType.FrameParts:
                    listMeshMat = Shared.m_effectMeshMatList.m_framePartsList;
                    posX = -Settings.Instance.DrawPos.BgScrollX;
                    break;
            }

            int shaderNo = Settings.Instance.Debug.ShaderNo;
            if (shaderNo < 0 || shaderNo >= listMeshMat.Count)
            {
                return;
            }

            Mesh mesh = listMeshMat[shaderNo].m_mesh;
            Material mat = listMeshMat[shaderNo].GetMaterial();



            Matrix4x4 matrixes = Matrix4x4.TRS(
                new Vector3(posX, Settings.Instance.DrawPos.BgScrollY, layer),
                 m_QuaternionRev, Vector3.one);

            mat.SetInt("_Frame", Settings.Instance.Debug.ShaderFrame);
            mat.SetFloat("_Value", Settings.Instance.Debug.ShaderValue);

            Graphics.DrawMesh(mesh, matrixes, mat, 0);
        }
        private void DrawChara()
        {
            YHAnimationUtils.DrawYHAnimation(
                Settings.Instance.Debug.AnimName,
                Settings.Instance.Debug.CharaNo,
                Settings.Instance.Debug.AnimFrame,
                0, (Settings.Instance.Debug.CharaNo == 0));
        }
    }
}
