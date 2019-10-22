using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    public struct DamageState : IComponentData
    {
        public int m_lifeDamage;
        public int m_balanceDamage;
    }
}