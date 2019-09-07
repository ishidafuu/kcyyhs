using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public class YHAnimation
    {
        public string m_name;
        public int m_length;
        public List<YHAnimationParts> m_parts = new List<YHAnimationParts>();
        public List<YHFrameEvent> m_events = new List<YHFrameEvent>();
    }

    [Serializable]
    public class YHAnimationParts
    {
        public string m_name;
        public float m_orderInLayer;
        public List<YHFrameData> m_isActive = new List<YHFrameData>();
        public List<YHFrameData> m_isFlipX = new List<YHFrameData>();
        public List<YHFrameData> m_isFlipY = new List<YHFrameData>();
        public List<YHFrameData> m_isBrink = new List<YHFrameData>();
        public AnimationCurve m_positionX;
        public AnimationCurve m_positionY;
        public AnimationCurve m_scaleX;
        public AnimationCurve m_scaleY;
        public AnimationCurve m_rotation;
    }

    [Serializable]
    public class YHFrameData
    {
        public int m_frame;
        public bool m_value;
    }

    [Serializable]
    public class YHFramePosition
    {
        public int m_frame;
        public Vector2Int m_position;
        public Keyframe m_keyFrameX;
        public Keyframe m_keyFrameY;
    }

    [Serializable]
    public class YHFrameScale
    {
        public int m_frame;
        public Vector2 m_scale;
        public Keyframe m_keyFrameX;
        public Keyframe m_keyFrameY;
    }

    [Serializable]
    public class YHFrameRotation
    {
        public int m_frame;
        public float m_rotation;
        public Keyframe m_keyFrame;
    }

    [Serializable]
    public class YHFrameEvent
    {
        public int m_frame;
        public EnumEventFunctionName m_functionName;
        public string m_stringParameter;
        public float m_floatParameter;
        public int m_intParameter;
    }

}
