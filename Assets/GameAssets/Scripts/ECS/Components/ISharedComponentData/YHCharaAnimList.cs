using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHCharaAnimList : IEquatable<YHCharaAnimList>, ISharedComponentData
    {
        public List<YHAnimationsObject> m_charaAnimList;
        public void Init()
        {
            m_charaAnimList = new List<YHAnimationsObject>();

            var loadObjects = Resources.LoadAll<YHAnimationsObject>("YHCharaAnim");
            if (loadObjects.Length == 0)
            {
                Debug.LogError("YHAnimationsObject None");
                return;
            }

            foreach (var item in loadObjects)
            {
                m_charaAnimList.Add(item);
            }
        }

        public YHAnimation GetAnim(int charaNo, EnumAnimationName animName)
        {
            return m_charaAnimList[charaNo].animations[(int)animName];
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