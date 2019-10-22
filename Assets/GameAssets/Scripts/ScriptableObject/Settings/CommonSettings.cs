using System;
using UnityEngine;

namespace YYHS
{
    /// <summary>
    /// 座標移動設定
    /// </summary>
    [CreateAssetMenu(menuName = "YYHS/Settings/CommonSettings", fileName = "CommonSettings")]
    public sealed class CommonSettings : ScriptableObject
    {
        public int PlayerCount;
        public int CharaCount;
        public int ButtonCount;
        public int FilterEffectCount;
        public int ReiPieceCount;
        public int ToukiMax;
        public int LifeMax;
        public int BalanceMax;
        public int ReiMax;
        public int DamageSpeed;
    }
}