using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{

    public class AnimationEvent : MonoBehaviour
    {
        // 効果音再生
        public void EventPlaySE(int seNo) { }
        // エフェクト
        public void EventEffectScreen(int effectNo) { }
        public void EventEffectScreenArg(string arg) { }
        public void EventEffectLarge(int effectNo) { }
        public void EventEffectMedium(int effectNo) { }
        public void EventEffectSmall(int effectNo) { }
        // 体位置にヒットマーク
        public void EventEffectDamageBody() { }
        // 顔位置にヒットマーク
        public void EventEffectDamageFace() { }
        // フィルター
        public void EventFillterScreen(int fillterNo) { }
        public void EventFillterScreenArg(string arg) { }
        public void EventFillterBG(int fillterNo) { }
        public void EventFillterBGArg(string arg) { }
        // 点滅１回
        public void EventFlash() { }
        // 連続点滅
        public void EventBlink(int frame) { }
        // 色変え
        public void EventColor(int colorNo, float frame) { }
    }
}
