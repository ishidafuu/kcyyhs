using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKKD.EDIT
{
    [Serializable]
    public class TackPointModel
    {
        [SerializeField]
        public string TackId;

        [SerializeField]
        public string ParentTimelineId;

        [SerializeField]
        public int Index;

        [SerializeField]
        public bool Active;

        [SerializeField]
        public bool IsExistTack;

        [SerializeField]
        public int Start;

        [SerializeField]
        public int Span;

        [SerializeField]
        public int TimelineType;

        [SerializeField]
        public MotionData MotionData;

        [SerializeField]
        public Texture2D TackBackTransparentTex;

        [SerializeField]
        public Texture2D TackColorTex;
    }
}