using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct Status : IComponentData
    {
        public int m_life;
        public int m_balance;
        public int m_rei;
    }
}