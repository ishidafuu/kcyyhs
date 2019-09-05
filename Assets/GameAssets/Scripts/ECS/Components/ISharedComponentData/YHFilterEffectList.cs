using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHFilterEffectList : IEquatable<YHFilterEffectList>, ISharedComponentData
    {
        public List<YHFilterEffectObject> m_effects;
        public void Init()
        {
            m_effects = new List<YHFilterEffectObject>();

            var loadObjects = Resources.LoadAll<YHFilterEffectObject>("YHFilterEffect");
            if (loadObjects.Length == 0)
            {
                Debug.LogError("YHFilterEffect None");
                return;
            }

            foreach (var item in loadObjects)
            {
                m_effects.Add(item);
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