// #if ENABLE_DEBUG

using System;
using UnityEngine;

namespace YYHS
{
    [CreateAssetMenu(menuName = "YYHS/Settings/DebugViewerSettings", fileName = "DebugViewerSettings")]
    public sealed class DebugViewerSettings : ScriptableObject
    {
        public bool IsPause;
        [Range(0, 10)]
        public int GameWait;

        [HeaderAttribute("ShaderDebug")]
        public bool IsShaderView;
        public EnumShaderType ShaderType;
        [Range(0, 32)]
        public int ShaderNo;
        [Range(0, 256)]
        public int ShaderFrame;
        [Range(0, 10)]
        public float ShaderValue;
        [HeaderAttribute("CharaDebug")]
        public bool IsCharaView;
        [Range(0, 1)]
        public int CharaNo;
        public EnumAnimationName AnimName;
        [Range(0, 256)]
        public int AnimFrame;

        int NowCount = 0;

        public void InitCount()
        {
            NowCount = 0;
        }

        public void IncCount()
        {
            NowCount++;
        }

        public bool IsSkip()
        {
            if (IsPause)
                return true;

            return (NowCount % (GameWait + 1) != 0);
        }
    }
}

// #endif