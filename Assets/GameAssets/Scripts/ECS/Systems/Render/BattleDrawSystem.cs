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
            YHAnimation anim = Shared.yhCharaAnimList.GetAnim(charaNo, animName);
            int count = seq.animation.count;

            foreach (YHAnimationParts item in anim.parts)
            {
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

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(posX, posY, layer),
                    Quaternion.Euler(new Vector3(-90, 0, rotate)),
                    new Vector3(scaleX, scaleY, 1));

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }
    }
}
