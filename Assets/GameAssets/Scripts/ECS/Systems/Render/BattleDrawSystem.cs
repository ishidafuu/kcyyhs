
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

            if (!seq.isPlay || seq.isTransition)
                return;

            int charaNo = seq.animation.charaNo;
            EnumAnimationName animName = seq.animation.animName;
            // Debug.Log(animName);
            YHAnimation anim = Shared.yhCharaAnimList.GetAnim(charaNo, animName);
            int count = seq.animation.count;

            YHFrameData emptyFrameData = new YHFrameData();
            foreach (YHAnimationParts item in anim.parts)
            {

                // TODO:背景なども適切なメッシュから描画する
                if (!Shared.charaMeshMat.meshDict.ContainsKey(item.name))
                    continue;

                YHFrameData isActive = emptyFrameData;
                GetNowFrameData(count, item.isActive, ref isActive);

                if (!isActive.value)
                    continue;

                // Debug.Log(item.name);

                float posX = (item.positionX.length == 0)
                    ? 0
                    : item.positionX.Evaluate(count);
                float posY = (item.positionY.length == 0)
                    ? 0
                    : item.positionY.Evaluate(count) + Settings.Instance.DrawPos.BgScrollY;

                float scaleX = (item.scaleX.length == 0)
                    ? 1
                    : item.scaleX.Evaluate(count);
                float scaleY = (item.scaleY.length == 0)
                    ? 1
                    : item.scaleY.Evaluate(count);

                float rotate = (item.rotation.length == 0)
                    ? 0
                    : item.rotation.Evaluate(count);

                float layer = (int)EnumDrawLayer.Chara + item.orderInLayer;

                YHFrameData isFlipX = emptyFrameData;
                GetNowFrameData(count, item.isFlipX, ref isFlipX);
                int flipX = (isFlipX.value)
                    ? 180
                    : 0;

                YHFrameData isFlipY = emptyFrameData;
                GetNowFrameData(count, item.isFlipY, ref isFlipY);
                int flipY = (isFlipY.value)
                    ? +90
                    : -90;

                YHFrameData isBrink = emptyFrameData;
                GetNowFrameData(count, item.isBrink, ref isBrink);

                Mesh mesh = Shared.charaMeshMat.meshDict[item.name];
                Material mat = Shared.charaMeshMat.materialDict[item.name];
                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(posX, posY, layer),
                    Quaternion.Euler(new Vector3(flipY, flipX, rotate)),
                    new Vector3(scaleX, 1, scaleY));

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private static void GetNowFrameData(int count, List<YHFrameData> srcList, ref YHFrameData nowData)
        {
            foreach (var data in srcList)
            {
                if (data.frame > count)
                    break;
                nowData = data;
            }
        }
    }
}
