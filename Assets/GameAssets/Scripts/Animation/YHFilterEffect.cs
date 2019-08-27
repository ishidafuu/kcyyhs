using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHFilterEffect
    {
        public int width;
        public int height;
        public bool isOverChara;
        public int offsetY;
        public int moveX;
        public int moveY;
        public int flipInterval;
        public int flipCount;
    }
}
