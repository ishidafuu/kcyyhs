using System;
using UnityEngine;

namespace YYHS
{

    [CreateAssetMenu(menuName = "YYHS/Settings/AnimationSettings", fileName = "AnimationSettings")]
    public sealed class AnimationSettings : ScriptableObject
    {
        public int StartTransitionTime;
        public int EndTransitionTime;
        public int ToukiAnimationInterval;
        public int HighScrollSpeed;
        public int LowScrollSpeed;
        public int DecideScrollSpeed;
        public int JumpFadeFrame;
        public int FillterScreenEndFrame;
    }
}