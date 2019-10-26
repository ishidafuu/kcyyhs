
using UnityEngine;
using Unity.Mathematics;
using Unity.Entities;
using System.Collections.Generic;

namespace YYHS
{
    public static class SideUtil
    {
        public static bool IsSideA(int index) => (index == 0);
        public static int Index(bool isSideA) => (isSideA) ? 0 : 1;
        public static int Sign(bool isSideA) => (isSideA) ? +1 : -1;
        public static int Sign(int index) => IsSideA(index) ? +1 : -1;
        public static int PosSign(bool isSideA) => (isSideA) ? -1 : +1;
        public static int PosSign(int index) => IsSideA(index) ? -1 : +1;
        public static int EnemyIndex(int index) => (index == 0) ? 1 : 0;
    }
}
