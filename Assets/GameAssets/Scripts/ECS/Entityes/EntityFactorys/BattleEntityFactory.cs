using Unity.Entities;

namespace YYHS
{
    public static class BattleEntityFactory
    {
        public static Entity CreateEntity(EntityManager entityManager)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.BattleComponentType);
            var entity = entityManager.CreateEntity(archetype);

            BattleSequencer seq = new BattleSequencer();
            seq.m_sideA.m_isSideA = true;
            seq.m_sideB.m_isSideA = false;
            entityManager.SetComponentData(entity, seq);

            ReiPool reiPool = new ReiPool();
            reiPool.m_reiState = EnumReiState.Idle;
            reiPool.m_amount = 4;
            entityManager.SetComponentData(entity, reiPool);

            return entity;
        }
    }
}
