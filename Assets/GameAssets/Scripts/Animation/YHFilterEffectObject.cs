using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    [CreateAssetMenu(menuName = "YYHS/YHFilterEffectObject", fileName = "YHFilterEffect_")]
    public class YHFilterEffectObject : ScriptableObject
    {
        public string imageName;
        public Color[] color;
        public YHFilterEffect data;
    }
}
