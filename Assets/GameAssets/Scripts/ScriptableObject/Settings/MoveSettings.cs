using System;
using UnityEngine;

namespace NKKD
{
    /// <summary>
    /// 座標移動設定
    /// </summary>
    [CreateAssetMenu(menuName = "Settings/MoveSettings", fileName = "MoveSettings")]
    public sealed class MoveSettings : ScriptableObject
    {
        public int WalkSpeed;
        public int DashSpeed;
        public int BrakeDelta;
    }
}