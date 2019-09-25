
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using System.Collections.Generic;

namespace YYHS
{
    public static class YHAnimationUtils
    {

        public static void DrawYHAnimation(EnumAnimationName animName, int charaNo, int count, int basePosX, bool isSideA)
        {
            YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);


            foreach (YHAnimationParts item in anim.m_parts)
            {

                YHFrameData isActive = YHAnimationUtils.GetNowFrameData(count, item.m_isActive);

                if (isActive == null || !isActive.m_value)
                    continue;

                YHFrameData isBrink = YHAnimationUtils.GetNowFrameData(count, item.m_isBrink);
                if (isBrink != null && isBrink.m_value)
                {
                    if (count % 4 >= 2)
                        continue;
                }

                Mesh mesh = null;
                Material mat = null;
                int layer = 0;

                bool isBG = false;

                if (Shared.m_charaMeshMat.m_materialDict.ContainsKey(item.m_name))
                {
                    mesh = Shared.m_charaMeshMat.m_meshDict[item.m_name];
                    mat = Shared.m_charaMeshMat.m_materialDict[item.m_name];
                    layer = (int)EnumDrawLayer.Chara;
                }
                else if (Shared.m_commonMeshMat.m_materialDict.ContainsKey(item.m_name))
                {
                    mesh = Shared.m_commonMeshMat.m_meshDict[item.m_name];
                    mat = Shared.m_commonMeshMat.m_materialDict[item.m_name];
                    layer = (int)EnumDrawLayer.Chara;
                }
                else if (Shared.m_bgFrameMeshMat.m_materialDict.ContainsKey(item.m_name))
                {
                    mesh = Shared.m_bgFrameMeshMat.m_meshDict[item.m_name];
                    mat = Shared.m_bgFrameMeshMat.m_materialDict[item.m_name];
                    layer = (int)EnumDrawLayer.BackGround;
                    isBG = true;
                }
                else
                {
                    Debug.LogError($"Not Found Material or Mesh {item.m_name}");
                    continue;
                }

                Vector3 pos = YHAnimationUtils.EvalutePos(item, count, layer, basePosX, isSideA);
                Quaternion q = YHAnimationUtils.EvaluteQuaternion(item, count, isSideA);
                Vector3 scale = YHAnimationUtils.EvaluteScale(item, count);
                Draw(mesh, mat, q, scale, pos);

                // 不足背景追加描画
                if (isBG && pos.x != 0)
                {
                    int BgWidthHalf = Settings.Instance.DrawPos.BgWidth >> 1;
                    int sizeHalf = (int)(mesh.bounds.size.x * 0.5f);
                    if ((pos.x + BgWidthHalf - sizeHalf) > 0)
                    {
                        Vector3 posL = pos;
                        posL.x -= mesh.bounds.size.x;
                        Draw(mesh, mat, q, scale, posL);
                    }
                    else if ((pos.x + sizeHalf) < BgWidthHalf)
                    {
                        Vector3 posR = pos;
                        posR.x += mesh.bounds.size.x;
                        Draw(mesh, mat, q, scale, posR);
                    }
                }
            }
        }

        private static void Draw(Mesh mesh, Material mat, Quaternion q, Vector3 scale, Vector3 pos)
        {
            Matrix4x4 matrixes = Matrix4x4.TRS(pos, q, scale);
            Graphics.DrawMesh(mesh, matrixes, mat, 0);
        }

        public static Vector3 EvalutePos(YHAnimationParts item, int count, int layer, int basePosX, bool isSideA)
        {
            int sign = (isSideA)
                ? +1
                : -1;
            float posX = (item.m_positionX.length == 0)
                ? 0
                : item.m_positionX.Evaluate(count);

            float posY = (item.m_positionY.length == 0)
                ? 0
                : item.m_positionY.Evaluate(count) + Settings.Instance.DrawPos.BgScrollY;

            return new Vector3((sign * posX) + basePosX, posY, (float)layer + item.m_orderInLayer);
        }

        public static Quaternion EvaluteQuaternion(YHAnimationParts item, int count, bool isSideA)
        {

            YHFrameData isFlipX = GetNowFrameData(count, item.m_isFlipX);
            bool isFlipX2 = (isFlipX != null && isFlipX.m_value);
            int flipX = (isFlipX2 ^ !isSideA)
                ? 180
                : 0;

            YHFrameData isFlipY = GetNowFrameData(count, item.m_isFlipY);
            int flipY = (isFlipY != null && isFlipY.m_value)
                ? +90
                : -90;

            quaternion result = Quaternion.Euler(new Vector3(flipY, flipX, 0));

            float rotate = (item.m_rotation.length == 0)
                ? 0
                : item.m_rotation.Evaluate(count);
            if (rotate != 0)
            {
                Quaternion rot = Quaternion.AngleAxis(rotate, Vector3.down);
                result = result * rot;
            }

            return result;
        }

        public static Vector3 EvaluteScale(YHAnimationParts item, int count)
        {
            float scaleX = (item.m_scaleX.length == 0)
                ? 1
                : item.m_scaleX.Evaluate(count);
            float scaleY = (item.m_scaleY.length == 0)
                ? 1
                : item.m_scaleY.Evaluate(count);

            return new Vector3(scaleX, 1, scaleY);
        }

        public static YHFrameData GetNowFrameData(int count, List<YHFrameData> srcList)
        {
            YHFrameData result = null;
            foreach (var data in srcList)
            {
                if (data.m_frame > count)
                    break;
                result = data;
            }

            return result;
        }
    }
}
