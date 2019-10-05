// #if ENABLE_DEBUG

using System;
using UnityEngine;

namespace YYHS
{
    /// <summary>
    /// ゲーム設定
    /// </summary>
    [CreateAssetMenu(menuName = "YYHS/Settings/DebugSettings", fileName = "DebugSettings")]
    public sealed class DebugSettings : ScriptableObject
    {
        public int ShaderFrame;
        public float ShaderValue;
    }
}

// #endif