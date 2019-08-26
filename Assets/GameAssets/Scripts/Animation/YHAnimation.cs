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
        public List<YHFrameData> frames = new List<YHFrameData>();
        public List<YHFramePosition> positions = new List<YHFramePosition>();
        public List<YHFrameScale> scales = new List<YHFrameScale>();
        public List<YHFrameRotation> rotations = new List<YHFrameRotation>();

    }

    [Serializable]
    public class YHFrameData
    {
        public int frame;
        public bool isActive;
        public bool isFlipX;
        public bool isFlipY;
    }

    [Serializable]
    public class YHFramePosition
    {
        public int frame;
        public Vector2Int position;
    }

    [Serializable]
    public class YHFrameScale
    {
        public int frame;
        public Vector2 scale;
    }

    [Serializable]
    public class YHFrameRotation
    {
        public int frame;
        public float rotation;
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
