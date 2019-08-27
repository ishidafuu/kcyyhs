using System;
using UnityEngine;

namespace YYHS
{
    /// <summary>
    /// 座標移動設定
    /// </summary>
    [CreateAssetMenu(menuName = "YYHS/Settings/DrawPosSettings", fileName = "DrawPosSettings")]
    public sealed class DrawPosSettings : ScriptableObject
    {
        public int BgWidth;
        public int BgHeight;
        public int BgScrollRangeFactor;
        public int BgScrollX;
        public int BgScrollY;
        public int ToukiWidth;
        public int ToukiMeterX;
        public int ToukiMeterY;
        public int FrameTopY;
        public int FrameBottomY;

    }
}