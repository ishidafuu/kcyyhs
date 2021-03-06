﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
    /// <summary>
    /// 中間モデル
    /// </summary>
    [Serializable]
    public class YHRawAnimation
    {
        public YHRawAnimationClip AnimationClip;
    }

    [Serializable]
    public class YHRawAnimationClip
    {
        public string m_Name;
        public YHRawVectorCurves[] m_PositionCurves;
        public YHRawVectorCurves[] m_ScaleCurves;
        public YHRawVectorCurves[] m_EulerCurves;
        public YHRawValueCurves[] m_FloatCurves;
        public YHRawEventFrame[] m_Events;
        public YHClipSettings m_AnimationClipSettings;
    }

    [Serializable]
    public class YHRawVectorCurves
    {
        public string path;
        public YHRawVectorCurve curve;
    }

    [Serializable]
    public class YHRawVectorCurve
    {
        public YHRawVectorFrame[] m_Curve;
    }

    [Serializable]
    public class YHRawVectorFrame
    {
        public float time;
        public Vector3 value;
        public StringVector3 inSlope;
        public StringVector3 outSlope;
        public int tangentMode;
        public int weightedMode;
        public Vector3 inWeight;
        public Vector3 outWeight;
    }

    [Serializable]
    public class YHRawValueCurves
    {
        public string path;
        public YHRawValueCurve curve;
        public string attribute;
    }

    [Serializable]
    public class YHRawValueCurve
    {
        public YHRawValueFrame[] m_Curve;
    }

    [Serializable]
    public class YHRawValueFrame
    {
        public float time;
        public float value;
    }

    [Serializable]
    public class YHRawEventCurves
    {
        public string path;
        public YHRawValueCurve curve;
        public string attribute;
    }

    [Serializable]
    public class YHRawEventFrame
    {
        public float time;
        public string functionName;
        public string data;
        public float floatParameter;
        public int intParameter;
    }

    [Serializable]
    public class YHClipSettings
    {
        public float m_StopTime;
    }

    [Serializable]
    public struct StringVector3
    {
        public string x;
        public string y;
        public string z;
    }
}
