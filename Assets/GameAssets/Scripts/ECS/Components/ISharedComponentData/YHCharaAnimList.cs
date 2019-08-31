using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHCharaAnimList : IEquatable<YHCharaAnimList>, ISharedComponentData
    {
        // public Dictionary<string, YHFilterEffect> effectDict;
        public List<YHAnimationsObject> animations;
        public void Init()
        {
            animations = new List<YHAnimationsObject>();

            var loadObjects = Resources.FindObjectsOfTypeAll<YHAnimationsObject>();
            if (loadObjects.Length == 0)
            {
                Debug.LogError("YHAnimationsObject None");
                return;
            }

            foreach (var item in loadObjects)
            {
                // int effectNo = 0;
                // Int32.TryParse(item.name.Remove(0, item.name.IndexOf("_") + 1), out effectNo);
                // Debug.Log(effectNo);
                animations.Add(item);
            }
        }



        public bool Equals(YHCharaAnimList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}