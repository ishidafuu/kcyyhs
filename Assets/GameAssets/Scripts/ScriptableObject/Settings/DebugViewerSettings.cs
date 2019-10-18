// #if ENABLE_DEBUG

using System;
using UnityEngine;

namespace YYHS
{
    [CreateAssetMenu(menuName = "YYHS/Settings/DebugViewerSettings", fileName = "DebugViewerSettings")]
    public sealed class DebugViewerSettings : ScriptableObject
    {
        [SerializeField, HeaderAttribute("ShaderDebug")]
        public bool IsShaderView;
        public EnumShaderType ShaderType;
        [Range(0, 32)]
        public int ShaderNo;
        [Range(0, 256)]
        public int ShaderFrame;
        [Range(0, 10)]
        public float ShaderValue;
        [SerializeField, HeaderAttribute("CharaDebug")]
        public bool IsCharaView;
        [Range(0, 1)]
        public int CharaNo;
        public EnumAnimationName AnimName;
        [Range(0, 256)]
        public int AnimFrame;
    }
}

// #endif