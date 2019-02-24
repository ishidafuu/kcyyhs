using System;
using UnityEngine;

namespace YYHS
{
    /// <summary>
    /// 座標移動設定
    /// </summary>
    [CreateAssetMenu(menuName = "Settings/CommonSettings", fileName = "CommonSettings")]
    public sealed class CommonSettings : ScriptableObject
    {
        public int PlayerNum;
        public int CharaNum;

    }
}