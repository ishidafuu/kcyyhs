using Unity.Entities;

namespace YYHS
{
    public static class FilterEffectEntityFactory
    {
        public static Entity CreateEntity(int i, EntityManager entityManager, ref YHFilterEffectList effectList)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.FilterEffectComponentType);
            var entity = entityManager.CreateEntity(archetype);

            entityManager.SetComponentData(entity, new FilterEffect
            {

            });

            // entityManager.SetSharedComponentData(entity, effectList);

            return entity;
        }
    }
}
