using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHFilterEffect
    {
        public int m_width;
        public int m_height;
        public bool m_isOverChara;
        public int m_offsetY;
        public int m_moveX;
        public int m_moveY;
        public int m_flipInterval;
        public int m_flipCount;
    }
}
