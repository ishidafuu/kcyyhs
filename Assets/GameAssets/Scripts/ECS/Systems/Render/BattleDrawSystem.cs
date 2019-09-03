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
    public class BattleDrawDrawSystem : ComponentSystem
    {

        protected override void OnCreate()
        {
            RequireSingletonForUpdate<BattleSequencer>();
        }

        protected override void OnUpdate()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (!seq.isPlay)
                return;

            int charaNo = seq.animation.charaNo;
            EnumAnimationName animName = seq.animation.animName;
            Debug.Log(animName);
            YHAnimation anim = Shared.yhCharaAnimList.GetAnim(charaNo, animName);
            int count = seq.animation.count;

            YHFrameData emptyFrameData = new YHFrameData();
            foreach (YHAnimationParts item in anim.parts)
            {
                YHFrameData targetFrame = emptyFrameData;
                foreach (YHFrameData frame in item.frames)
                {
                    if (frame.frame > count)
                        break;

                    targetFrame = frame;
                }

                if (!targetFrame.isActive)
                    continue;


                Debug.Log(item.name);
                // TODO:背景なども適切なメッシュから描画する
                if (!Shared.charaMeshMat.meshDict.ContainsKey(item.name))
                    continue;

                Mesh mesh = Shared.charaMeshMat.meshDict[item.name];
                Material mat = Shared.charaMeshMat.materialDict[item.name];
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
                float rotate = (item.scaleX.length == 0)
                    ? 0
                    : item.rotation.Evaluate(count);

                int layer = (int)EnumDrawLayer.Chara;

                int flipX = (targetFrame.isFlipX)
                    ? 180
                    : 0;

                int flipY = (targetFrame.isFlipY)
                    ? +90
                    : -90;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(posX, posY, layer),
                    Quaternion.Euler(new Vector3(flipY, flipX, rotate)),
                    new Vector3(scaleX, scaleY, 1));

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }
    }
}
