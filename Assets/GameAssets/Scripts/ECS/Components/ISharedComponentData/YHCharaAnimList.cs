using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHCharaAnimList : IEquatable<YHCharaAnimList>, ISharedComponentData
    {
        YHAnimationsObject m_charaAnimCommon;
        List<YHAnimationsObject> m_charaAnimList;
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
                if (item.name.IndexOf("Common") >= 0)
                {
                    m_charaAnimCommon = item;
                }
                else
                {
                    m_charaAnimList.Add(item);
                }
            }

        }

        public YHAnimation GetAnim(int sideNo, EnumAnimationName animName)
        {
            int animeNo = (int)animName;
            if (animeNo < m_charaAnimCommon.animations.Count)
            {
                return m_charaAnimCommon.animations[animeNo];
            }

            animeNo -= m_charaAnimCommon.animations.Count;

            if (animeNo >= m_charaAnimList[sideNo].animations.Count)
                Debug.LogError($"Out Of Range  animations count:{m_charaAnimList[sideNo].animations.Count} animName:{(int)animName}({animName})");

            return m_charaAnimList[sideNo].animations[animeNo];
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