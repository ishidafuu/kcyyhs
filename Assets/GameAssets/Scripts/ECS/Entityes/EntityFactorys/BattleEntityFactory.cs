﻿using Unity.Entities;

namespace YYHS
{
    public static class BattleEntityFactory
    {
        public static Entity CreateEntity(EntityManager entityManager, ref YHFilterEffectList effectList)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.BattleComponentType);
            var entity = entityManager.CreateEntity(archetype);

            BattleSequencer seq = new BattleSequencer();
            seq.m_sideA.m_isSideA = true;
            seq.m_sideB.m_isSideA = false;
            entityManager.SetComponentData(entity, seq);

            return entity;
        }
    }
}
