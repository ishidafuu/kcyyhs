using System;
using UnityEngine;

namespace YYHS
{
    /// <summary>
    /// 座標移動設定
    /// </summary>
    [CreateAssetMenu(menuName = "Settings/DrawPosSettings", fileName = "DrawPosSettings")]
    public sealed class DrawPosSettings : ScriptableObject
    {
        public int BgScrollWidth;
        public int BgScrollRangeFactor;
        public int BgScrollY;
        public int ToukiMeterX;
        public int ToukiMeterY;
    }
}