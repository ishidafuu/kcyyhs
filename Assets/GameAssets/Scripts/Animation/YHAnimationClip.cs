using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{

    [Serializable]
    public class YHAnimationClip
    {
        public string m_Name;
        public int serializedVersion;
        public YHPositionCurves[] m_PositionCurves;
        public YHPositionCurves[] m_ScaleCurves;
    }
}
