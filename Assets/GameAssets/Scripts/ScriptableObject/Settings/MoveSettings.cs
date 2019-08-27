using System;
using UnityEngine;

namespace YYHS
{

    [CreateAssetMenu(menuName = "YYHS/Settings/MoveSettings", fileName = "MoveSettings")]
    public sealed class MoveSettings : ScriptableObject
    {
        public int WalkSpeed;
        public int DashSpeed;
        public int BrakeDelta;
    }
}