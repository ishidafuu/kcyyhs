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
        public string m_imageBaseName;
        public Color[] m_color;
        public YHFilterEffect m_data;
    }
}
