using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHFilterEffectList : IEquatable<YHFilterEffectList>, ISharedComponentData
    {
        public Dictionary<string, YHFilterEffect> effectDict;

        public void Init()
        {
            effectDict = new Dictionary<string, YHFilterEffect>();

            var loadObjects = Resources.FindObjectsOfTypeAll<YHFilterEffect>();
            if (loadObjects.Length == 0)
            {
                Debug.LogError("YHFilterEffect None");
                return;
            }

            foreach (var item in loadObjects)
            {
                effectDict.Add(item.imageName, item as YHFilterEffect);
            }

            foreach (var item in effectDict)
            {
                Debug.Log(item.Key);

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