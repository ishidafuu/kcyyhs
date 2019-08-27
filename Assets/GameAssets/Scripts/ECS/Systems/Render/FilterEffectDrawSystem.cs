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
    public class FilterEffectDrawSystem : JobComponentSystem
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

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            m_query.AddDependency(inputDeps);

            NativeArray<FilterEffect> filterEffects = m_query.ToComponentDataArray<FilterEffect>(Allocator.TempJob);
            DrawFilterEffect(filterEffects);
            filterEffects.Dispose();
            return inputDeps;
        }

        private void DrawFilterEffect(NativeArray<FilterEffect> filterEffects)
        {
            int BgWidthHalf = (Settings.Instance.DrawPos.BgWidth >> 1);
            int BgHeightHalf = (Settings.Instance.DrawPos.BgHeight >> 1);
            for (int i = 0; i < filterEffects.Length; i++)
            {
                var filterEffect = filterEffects[i];
                if (!filterEffect.isActive)
                    continue;

                var data = Shared.yhFilterEffectList.effects[filterEffect.id].data;
                int flipNo = (data.flipCount >= 2 && data.flipInterval > 0)
                    ? ((filterEffect.count / data.flipInterval) % data.flipCount)
                    : 0;

                string imageName = $"{Shared.yhFilterEffectList.effects[filterEffect.id].imageName}_{flipNo.ToString("d2")}";
                Mesh mesh = Shared.commonMeshMat.meshDict[imageName];
                Material mat = Shared.commonMeshMat.SetColor(imageName, new Color(1f, 0.5f, 0.5f, 0.5f));

                int layer = (data.isOverChara)
                    ? (int)EnumDrawLayer.OverChara
                    : (int)EnumDrawLayer.OverBackGround;

                int intervalWidth = (data.offsetY == 0)
                    ? data.width
                    : data.width * (data.height / data.offsetY);

                int centerX = (filterEffect.count * data.moveX) % intervalWidth;
                int centerY = (filterEffect.count * data.moveY) % data.height;
                int leftCount = (int)math.ceil((float)(centerX + BgWidthHalf) / data.width) + 1;
                int rightCount = (int)math.ceil((float)(BgWidthHalf - centerX) / data.width) + 1;

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
            var data = Shared.yhFilterEffectList.effects[filterEffect.id].data;
            int posX = centerX + (data.width * x);
            int baseY = centerY + (data.offsetY * x);
            int topCount = (int)math.ceil((float)(BgHeightHalf - baseY) / data.height) + 1;
            int bottomCount = (int)math.ceil((float)(baseY + BgHeightHalf) / data.height) + 1;

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
            int posY = baseY + (data.height * y);

            Matrix4x4 matrixes = Matrix4x4.TRS(
                new Vector3(posX, posY + Settings.Instance.DrawPos.BgScrollY, layer),
                m_quaternion, Vector3.one);


            // Shared.commonMeshMat.propertyBlock.Clear();
            // Shared.commonMeshMat.propertyBlock.SetColor(Shared.commonMeshMat.colorPropertyId, Color.red);
            // Graphics.DrawMesh(ObjMesh, Obj.WorldPosition + new Vector3(ofst, 0, ofst), Obj.Rotation, Material1, 0, MainCamera, 0, _PropertyBlock);

            Graphics.DrawMesh(mesh, matrixes, mat, 0);
        }
    }
}
