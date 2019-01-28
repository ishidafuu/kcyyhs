using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKKD.EDIT
{
    [Serializable]
    public class TackPointViewModel
    {
        public Texture2D tackBackTransparentTex_;
        public Texture2D tackColorTex_;
        public Vector2 distance_ = Vector2.zero;
        public int lastStart_;
        public int lastSpan_;
        public TackModifyMode mode_ = TackModifyMode.NONE;
        public Vector2 dragBeginPoint_;
        public GUIStyle labelStyle_;
    }
}