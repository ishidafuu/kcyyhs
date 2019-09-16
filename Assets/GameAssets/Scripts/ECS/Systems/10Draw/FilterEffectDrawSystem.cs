using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Experimental.PlayerLoop;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    public class FilterEffectDrawSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_quaternion;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<FilterEffect>()
            // ComponentType.ReadOnly<BgScroll>()
            );
            m_quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        }

        protected override void OnUpdate()
        {

            NativeArray<FilterEffect> filterEffects = m_query.ToComponentDataArray<FilterEffect>(Allocator.TempJob);
            // DrawFilterEffect(filterEffects);
            DrawShaderGraphTest();
            filterEffects.Dispose();
        }


        private void DrawShaderGraphTest()
        {
            int effectNo = 6;
            Mesh mesh = Shared.m_effectMeshMatList.m_meshMatList[effectNo].m_mesh;
            Material mat = Shared.m_effectMeshMatList.m_meshMatList[effectNo].m_material;

            int layer = (int)EnumDrawLayer.OverBackGround;

            Matrix4x4 matrixes = Matrix4x4.TRS(
                new Vector3(0, Settings.Instance.DrawPos.BgScrollY, layer),
                m_quaternion, Vector3.one);

            Graphics.DrawMesh(mesh, matrixes, mat, 0);
        }

        private void DrawFilterEffect(NativeArray<FilterEffect> filterEffects)
        {
            int BgWidthHalf = (Settings.Instance.DrawPos.BgWidth >> 1);
            int BgHeightHalf = (Settings.Instance.DrawPos.BgHeight >> 1);
            for (int i = 0; i < filterEffects.Length; i++)
            {
                var filterEffect = filterEffects[i];
                if (!filterEffect.m_isActive)
                    continue;

                var data = Shared.m_yhFilterEffectList.m_effects[filterEffect.m_effectIndex].m_data;
                int flipNo = (data.m_flipCount >= 2 && data.m_flipInterval > 0)
                    ? ((filterEffect.m_count / data.m_flipInterval) % data.m_flipCount)
                    : 0;

                string imageName = $"{Shared.m_yhFilterEffectList.m_effects[filterEffect.m_effectIndex].m_imageBaseName}_{flipNo.ToString("d2")}";
                if (!Shared.m_commonMeshMat.m_meshDict.ContainsKey(imageName))
                    Debug.LogError($"Not Found ImageName:{imageName}");

                Mesh mesh = Shared.m_commonMeshMat.m_meshDict[imageName];
                Material mat = Shared.m_commonMeshMat.SetColor(imageName, new Color(1f, 0.5f, 0.5f, 0.5f));

                int layer = (data.m_isOverChara)
                    ? (int)EnumDrawLayer.OverChara
                    : (int)EnumDrawLayer.OverBackGround;

                int intervalWidth = (data.m_offsetY == 0)
                    ? data.m_width
                    : data.m_width * (data.m_height / data.m_offsetY);

                int centerX = (filterEffect.m_count * data.m_moveX) % intervalWidth;
                int centerY = (filterEffect.m_count * data.m_moveY) % data.m_height;
                int leftCount = (int)math.ceil((float)(centerX + BgWidthHalf) / data.m_width) + 1;
                int rightCount = (int)math.ceil((float)(BgWidthHalf - centerX) / data.m_width) + 1;

                if (rightCount == 0)
                {
                    rightCount = 1;
                }

                for (int x = 0; x < rightCount; x++)
                {
                    DrawYLine(ref filterEffect, mesh, mat, BgHeightHalf, centerX, centerY, +x, layer);
                }

                for (int x = 0; x < leftCount; x++)
                {
                    if (x == 0)
                        continue;
                    DrawYLine(ref filterEffect, mesh, mat, BgHeightHalf, centerX, centerY, -x, layer);
                }
            }
        }

        private void DrawYLine(ref FilterEffect filterEffect, Mesh mesh, Material mat,
             int BgHeightHalf, int centerX, int centerY, int x, int layer)
        {
            var data = Shared.m_yhFilterEffectList.m_effects[filterEffect.m_effectIndex].m_data;
            int posX = centerX + (data.m_width * x);
            int baseY = centerY + (data.m_offsetY * x);
            int topCount = (int)math.ceil((float)(BgHeightHalf - baseY) / data.m_height) + 1;
            int bottomCount = (int)math.ceil((float)(baseY + BgHeightHalf) / data.m_height) + 1;

            if (topCount == 0)
            {
                topCount = 1;
            }

            for (int y = 0; y < topCount; y++)
            {
                Draw(mesh, mat, data, posX, baseY, +y, layer);
            }

            for (int y = 0; y < bottomCount; y++)
            {
                if (y == 0)
                    continue;

                Draw(mesh, mat, data, posX, baseY, -y, layer);
            }
        }

        private void Draw(Mesh mesh, Material mat, YHFilterEffect data, int posX, int baseY, int y, int layer)
        {
            int posY = baseY + (data.m_height * y);

            Matrix4x4 matrixes = Matrix4x4.TRS(
                new Vector3(posX, posY + Settings.Instance.DrawPos.BgScrollY, layer),
                m_quaternion, Vector3.one);

            Graphics.DrawMesh(mesh, matrixes, mat, 0);
        }
    }
}
