using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YYHS
{
    public static class PathSettings
    {
        public static readonly string YHCharaAnim = "YHCharaAnim";
        public static readonly string YHCharaAnimCommon = "common";
        public static readonly string CommonSprite = "Sprites/common";
        public static readonly string CharaSprite = "Sprites/Character/chara_{0}";
        public static readonly string BackGroundSprite = "Sprites/BackGround/back_ground_{0}";
        public static readonly string EffectBGShader = "YHShader/EffectBG{0}";
        public static readonly string EffectScreenShader = "YHShader/EffectScreen{0}";
        public static readonly string EffectSmallShader = "YHShader/EffectSmall{0}";
        public static readonly string EffectMediumShader = "YHShader/EffectMedium{0}";
        public static readonly string EffectLargeShader = "YHShader/EffectLarge{0}";
        public static readonly string EffectSideShader = "YHShader/EffectSide{0}";
        public static readonly string FillterScreenShader = "YHShader/FillterScreen{0}";
        public static readonly string FramePartsShader = "YHShader/FrameParts{0}";
    }
}
