using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{

    public class AnimationEvent : MonoBehaviour
    {
        public void EventPlaySE(int seNo)
        {
            // 効果音再生
        }

        public void EventEffect(string effectName)
        {
            // 汎用エフェクト再生（無限）
        }

        public void EventFlash()
        {
            // 点滅１回
        }

        public void EventBlink(int frame)
        {
            // 連続点滅
        }

        public void EventColor(int colorNo, float frame)
        {
            // 色変え
        }

        public void EventPlE(Object asdf)
        {
            // 効果音再生
        }
    }
}
