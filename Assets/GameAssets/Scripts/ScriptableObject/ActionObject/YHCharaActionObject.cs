using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace YYHS
{

    [ShowOdinSerializedPropertiesInInspector]
    [CreateAssetMenu(menuName = "YYHS/CharaAction", fileName = "yh_chara_action_")]
    public sealed class YHCharaActionObject : SerializedScriptableObject
    {
        public List<YHActionData> actionData;
    }

    public sealed class YHActionData
    {
        public string actionName;
        public EnumAttackRangeType rangeType;
        public EnumAttackEffectType effectType;
        [Range(1, 10)]
        public int cost;
        [Range(1, 256)]
        public int probability;
        [Range(1, 256)]
        public int dodge;
        [Range(1, 256)]
        public int power;
        [Range(0, 256)]
        public int balance;
    }
}