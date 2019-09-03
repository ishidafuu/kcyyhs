using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHFilterEffectList : IEquatable<YHFilterEffectList>, ISharedComponentData
    {
        // public Dictionary<string, YHFilterEffect> effectDict;
        public List<YHFilterEffectObject> effects;
        public void Init()
        {
            effects = new List<YHFilterEffectObject>();

            var loadObjects = Resources.LoadAll<YHFilterEffectObject>("YHFilterEffect");
            if (loadObjects.Length == 0)
            {
                Debug.LogError("YHFilterEffect None");
                return;
            }

            foreach (var item in loadObjects)
            {
                // int effectNo = 0;
                // Int32.TryParse(item.name.Remove(0, item.name.IndexOf("_") + 1), out effectNo);
                // Debug.Log(effectNo);
                effects.Add(item);
            }
        }

        public bool Equals(YHFilterEffectList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}