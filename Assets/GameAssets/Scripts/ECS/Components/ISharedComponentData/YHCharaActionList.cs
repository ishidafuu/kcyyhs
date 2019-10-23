using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct YHCharaActionList : IEquatable<YHCharaActionList>, ISharedComponentData
    {
        List<YHCharaActionObject> m_charaAttackList;
        public void Init()
        {
            m_charaAttackList = new List<YHCharaActionObject>();

            var loadObjects = Resources.LoadAll<YHCharaActionObject>(PathSettings.YHCharaAction);
            if (loadObjects.Length == 0)
            {
                Debug.LogError("YHCharaAttackData None");
                return;
            }

            foreach (var item in loadObjects)
            {
                // Debug.Log(item.name);
                m_charaAttackList.Add(item);
            }
        }

        public YHActionData GetData(int charaNo, EnumCrossType cross, EnumButtonType buttonType)
        {
            int actionNo = (int)buttonType - 1;
            return GetData(charaNo, actionNo);
        }

        public YHActionData GetData(int charaNo, int actionNo)
        {

            if (actionNo >= m_charaAttackList[charaNo].actionData.Count)
            {
                Debug.LogError($"Out Of Range attackData count:{m_charaAttackList[charaNo].actionData.Count} attackData:{(int)actionNo}({actionNo})");
                return null;
            }

            return m_charaAttackList[charaNo].actionData[actionNo];
        }

        public bool Equals(YHCharaActionList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}