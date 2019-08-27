using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct FilterEffect : IComponentData
    {
        public int id;
        public bool isActive;
        public int effectNo;
        public int count;
    }
}