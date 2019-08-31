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
        public List<YHAnimationsObject> charaAnimList;
        public void Init()
        {
            charaAnimList = new List<YHAnimationsObject>();

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
                charaAnimList.Add(item);
            }
        }

        public YHAnimation GetAnim(int charaNo, EnumAnimationName animName)
        {
            return charaAnimList[charaNo].animations[(int)animName];
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