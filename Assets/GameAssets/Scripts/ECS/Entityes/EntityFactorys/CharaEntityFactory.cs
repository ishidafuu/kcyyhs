using Unity.Entities;

namespace YYHS
{
    public static class CharaEntityFactory
    {
        public static Entity CreateEntity(int i, EntityManager entityManager)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.CharaComponentType);
            var entity = entityManager.CreateEntity(archetype);

            // 必要なキャラのみインプットをつける
            if (i < Settings.Instance.Common.PlayerCount)
            {
                entityManager.AddComponent(entity, ComponentType.ReadWrite<PadScan>());
            }

            entityManager.SetComponentData(entity, new SideInfo
            {
                m_charaNo = 0,
                m_isSideA = (i == 0),
            });

            // 闘気メーター
            entityManager.SetComponentData(entity, new ToukiMeter
            {
                m_cross = EnumCrossType.None,
                m_value = 0,
            });

            return entity;
        }
    }
}
