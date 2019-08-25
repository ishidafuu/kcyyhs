using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YYHS
{

    [Serializable]
    public class YHAnimationsObject : ScriptableObject
    {
        public List<YHAnimation> animations = new List<YHAnimation>();
    }
}
