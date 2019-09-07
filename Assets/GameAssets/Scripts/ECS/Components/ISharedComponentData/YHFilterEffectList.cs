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
        Dictionary<string, int> m_indexDict;

        public void Init()
        {
            m_effects = new List<YHFilterEffectObject>();
            m_indexDict = new Dictionary<string, int>();

            var loadObjects = Resources.LoadAll<YHFilterEffectObject>("YHFilterEffect");
            if (loadObjects.Length == 0)
            {
                Debug.LogError("YHFilterEffect None");
                return;
            }

            int index = 0;
            foreach (var item in loadObjects)
            {
                Debug.Log(item.m_imageBaseName);
                m_effects.Add(item);
                m_indexDict[item.name] = index;
                index++;
            }
        }

        // public YHFilterEffectObject GetEffectObject(string effectName)
        // {
        //     int index = m_indexDict[effectName];
        //     return m_effects[index];
        // }

        public int GetEffectIndex(string effectName)
        {
            if (!m_indexDict.ContainsKey(effectName))
                Debug.LogError($"Not Found EffectName{effectName}");

            return m_indexDict[effectName];
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