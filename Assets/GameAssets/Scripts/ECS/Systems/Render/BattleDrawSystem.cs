﻿
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using System.Collections.Generic;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    public class BattleDrawDrawSystem : ComponentSystem
    {

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BattleSequencer>();
        }

        protected override void OnUpdate()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (!seq.m_isPlay || seq.m_isTransition)
                return;

            int charaNo = seq.m_animation.m_charaNo;
            EnumAnimationName animName = seq.m_animation.m_animName;
            // Debug.Log(animName);
            YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);
            int count = seq.m_animation.m_count;

            YHFrameData emptyFrameData = new YHFrameData();
            foreach (YHAnimationParts item in anim.parts)
            {

                YHFrameData isActive = GetNowFrameData(count, item.isActive);

                if (isActive == null || !isActive.value)
                    continue;

                YHFrameData isBrink = GetNowFrameData(count, item.isBrink);
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

                Vector3 pos = EvalutePos(item, count, layer);
                Quaternion q = EvaluteQuaternion(item, count);
                Vector3 scale = EvaluteScale(item, count);

                Matrix4x4 matrixes = Matrix4x4.TRS(pos, q, scale);
                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private static Vector3 EvalutePos(YHAnimationParts item, int count, int layer)
        {
            float posX = (item.positionX.length == 0)
                ? 0
                : item.positionX.Evaluate(count);
            float posY = (item.positionY.length == 0)
                ? 0
                : item.positionY.Evaluate(count) + Settings.Instance.DrawPos.BgScrollY;

            return new Vector3(posX, posY, (float)layer + item.orderInLayer);
        }

        private static Quaternion EvaluteQuaternion(YHAnimationParts item, int count)
        {
            float rotate = (item.rotation.length == 0)
                ? 0
                : item.rotation.Evaluate(count);

            YHFrameData isFlipX = GetNowFrameData(count, item.isFlipX);
            int flipX = (isFlipX != null && isFlipX.value)
                ? 180
                : 0;

            YHFrameData isFlipY = GetNowFrameData(count, item.isFlipY);
            int flipY = (isFlipY != null && isFlipY.value)
                ? +90
                : -90;

            return Quaternion.Euler(new Vector3(flipY, flipX, rotate));
        }

        private static Vector3 EvaluteScale(YHAnimationParts item, int count)
        {
            float scaleX = (item.scaleX.length == 0)
                ? 1
                : item.scaleX.Evaluate(count);
            float scaleY = (item.scaleY.length == 0)
                ? 1
                : item.scaleY.Evaluate(count);

            return new Vector3(scaleX, 1, scaleY);
        }

        private static YHFrameData GetNowFrameData(int count, List<YHFrameData> srcList)
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
