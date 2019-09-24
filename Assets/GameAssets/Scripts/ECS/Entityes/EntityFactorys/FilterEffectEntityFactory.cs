using Unity.Entities;

namespace YYHS
{
    public static class FilterEffectEntityFactory
    {
        public static Entity CreateEntity(int i, EntityManager entityManager)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.FilterEffectComponentType);
            var entity = entityManager.CreateEntity(archetype);

            entityManager.SetComponentData(entity, new FilterEffect
            {
                // m_isActive = (i == 0)   
            });

            // entityManager.SetSharedComponentData(entity, effectList);

            return entity;
        }
    }
}
