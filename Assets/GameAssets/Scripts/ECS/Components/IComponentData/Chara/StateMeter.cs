using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct StateMeter : IComponentData
    {
        public int m_life;
        public int m_balance;
    }
}