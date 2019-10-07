using Unity.Entities;

namespace YYHS
{
    public static class ReiPieceEntityFactory
    {
        public static Entity CreateEntity(int i, EntityManager entityManager)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.ReiPieceComponentType);
            var entity = entityManager.CreateEntity(archetype);

            entityManager.SetComponentData(entity, new ReiPiece
            {
                m_id = i,
            });

            return entity;
        }
    }
}
