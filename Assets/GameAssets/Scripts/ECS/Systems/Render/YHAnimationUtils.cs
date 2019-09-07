
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


            foreach (YHAnimationParts item in anim.parts)
            {

                YHFrameData isActive = YHAnimationUtils.GetNowFrameData(count, item.isActive);

                if (isActive == null || !isActive.value)
                    continue;

                YHFrameData isBrink = YHAnimationUtils.GetNowFrameData(count, item.isBrink);
                if (isBrink != null && isBrink.value)
                {
                    if (count % 4 >= 2)
                        continue;
                }

                Mesh mesh = null;
                Material mat = null;
                int layer = 0;

                if (Shared.m_charaMeshMat.m_materialDict.ContainsKey(item.name))
                {
                    mesh = Shared.m_charaMeshMat.m_meshDict[item.name];
                    mat = Shared.m_charaMeshMat.m_materialDict[item.name];
                    layer = (int)EnumDrawLayer.Chara;
                }
                else if (Shared.m_commonMeshMat.m_materialDict.ContainsKey(item.name))
                {
                    mesh = Shared.m_commonMeshMat.m_meshDict[item.name];
                    mat = Shared.m_commonMeshMat.m_materialDict[item.name];
                    layer = (int)EnumDrawLayer.Chara;
                }
                else if (Shared.m_bgFrameMeshMat.m_materialDict.ContainsKey(item.name))
                {
                    mesh = Shared.m_bgFrameMeshMat.m_meshDict[item.name];
                    mat = Shared.m_bgFrameMeshMat.m_materialDict[item.name];
                    layer = (int)EnumDrawLayer.BackGround;
                }
                else
                {
                    Debug.LogError($"Not Found Material or Mesh {item.name}");
                    continue;
                }

                Vector3 pos = YHAnimationUtils.EvalutePos(item, count, layer, basePosX, isSideA);
                Quaternion q = YHAnimationUtils.EvaluteQuaternion(item, count, isSideA);
                Vector3 scale = YHAnimationUtils.EvaluteScale(item, count);

                Matrix4x4 matrixes = Matrix4x4.TRS(pos, q, scale);
                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }


        public static Vector3 EvalutePos(YHAnimationParts item, int count, int layer, int basePosX, bool isSideA)
        {
            int sign = (isSideA)
                ? +1
                : -1;
            float posX = (item.positionX.length == 0)
                ? 0
                : item.positionX.Evaluate(count);

            float posY = (item.positionY.length == 0)
                ? 0
                : item.positionY.Evaluate(count) + Settings.Instance.DrawPos.BgScrollY;

            return new Vector3((sign * posX) + basePosX, posY, (float)layer + item.orderInLayer);
        }

        public static Quaternion EvaluteQuaternion(YHAnimationParts item, int count, bool isSideA)
        {
            float rotate = (item.rotation.length == 0)
                ? 0
                : item.rotation.Evaluate(count);

            YHFrameData isFlipX = GetNowFrameData(count, item.isFlipX);
            bool isFlipX2 = (isFlipX != null && isFlipX.value);
            int flipX = (isFlipX2 ^ !isSideA)
                ? 180
                : 0;

            YHFrameData isFlipY = GetNowFrameData(count, item.isFlipY);
            int flipY = (isFlipY != null && isFlipY.value)
                ? +90
                : -90;

            return Quaternion.Euler(new Vector3(flipY, flipX, rotate));
        }

        public static Vector3 EvaluteScale(YHAnimationParts item, int count)
        {
            float scaleX = (item.scaleX.length == 0)
                ? 1
                : item.scaleX.Evaluate(count);
            float scaleY = (item.scaleY.length == 0)
                ? 1
                : item.scaleY.Evaluate(count);

            return new Vector3(scaleX, 1, scaleY);
        }

        public static YHFrameData GetNowFrameData(int count, List<YHFrameData> srcList)
        {
            YHFrameData result = null;
            foreach (var data in srcList)
            {
                if (data.frame > count)
                    break;
                result = data;
            }

            return result;
        }
    }
}
