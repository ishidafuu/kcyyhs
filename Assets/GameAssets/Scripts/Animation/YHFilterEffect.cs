using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    [CreateAssetMenu(menuName = "YYHS/YHFilterEffect", fileName = "YHFilterEffect_")]
    public class YHFilterEffect : ScriptableObject
    {
        public string imageName;
        public int width;
        public int height;
        public bool isOverChara;
        public int offsetY;
        public int moveX;
        public int moveY;
        public int flipInterval;
        public int flipCount;
        public Color[] color;
    }
}
