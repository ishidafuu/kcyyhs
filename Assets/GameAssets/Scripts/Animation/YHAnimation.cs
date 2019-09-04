using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public class YHAnimation
    {
        public string name;
        public int length;
        public List<YHAnimationParts> parts = new List<YHAnimationParts>();
        public List<YHFrameEvent> events = new List<YHFrameEvent>();
    }

    [Serializable]
    public class YHAnimationParts
    {
        public string name;
        public float orderInLayer;
        public List<YHFrameData> frames = new List<YHFrameData>();
        public AnimationCurve positionX;
        public AnimationCurve positionY;
        public AnimationCurve scaleX;
        public AnimationCurve scaleY;
        public AnimationCurve rotation;
    }

    [Serializable]
    public class YHFrameData
    {
        public int frame;
        public bool isActive;
        public bool isFlipX;
        public bool isFlipY;
        public bool isBrink;

    }

    [Serializable]
    public class YHFramePosition
    {
        public int frame;
        public Vector2Int position;
        public Keyframe keyFrameX;
        public Keyframe keyFrameY;
    }

    [Serializable]
    public class YHFrameScale
    {
        public int frame;
        public Vector2 scale;
        public Keyframe keyFrameX;
        public Keyframe keyFrameY;
    }

    [Serializable]
    public class YHFrameRotation
    {
        public int frame;
        public float rotation;
        public Keyframe keyFrame;
    }

    [Serializable]
    public class YHFrameEvent
    {
        public int frame;
        public string functionName;
        public string data;
        public float floatParameter;
        public int intParameter;
    }

}
