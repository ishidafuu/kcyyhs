﻿using System.Collections.Generic;
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
        public static readonly string CommonSprite = "Sprites/common";
        public static readonly string CharaSprite = "Sprites/Character/chara{0}";
        public static readonly string BackGroundSprite = "Sprites/BackGround/bg{0}";

        public static readonly string EffectSprite = "Sprites/Effect/effect{0}";
        public static readonly string EffectMaterial = "Materials/Effect/effect{0}";
        public static readonly string EffectShader = "YHShader/effect{0}";
        public static readonly string FillterShader = "YHShader/fillter{0}";
    }
}
