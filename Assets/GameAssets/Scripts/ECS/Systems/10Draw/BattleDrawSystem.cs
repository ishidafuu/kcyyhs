
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

            if (seq.m_seqState <= EnumBattleSequenceState.Start)
                return;

            bool isSideA = seq.m_animation.m_isSideA;
            int charaNo = seq.m_animation.m_charaNo;
            EnumAnimationName animName = seq.m_animation.m_animName;
            YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);
            int count = seq.m_animation.m_count;

            YHAnimationUtils.DrawYHAnimation(animName, charaNo, count, 0, isSideA, false);
        }
    }
}
