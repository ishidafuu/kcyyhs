using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

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

                m_count = UnityEngine.Random.Range(0, 60),
                m_speed = UnityEngine.Random.Range(10f, 20f),
                m_width = UnityEngine.Random.Range(2f, 4f),
                m_basePos = new Vector2Int(
                    (int)(math.sin((math.PI * 2) / 6f * i) * 10),
                    (int)(math.cos((math.PI * 2) / 6f * i) * 10))

            });

            return entity;
        }
    }
}
