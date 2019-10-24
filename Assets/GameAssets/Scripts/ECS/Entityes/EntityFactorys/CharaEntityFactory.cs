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
                m_charaNo = i,
                m_isSideA = (i == 0),
            });

            // 闘気メーター
            entityManager.SetComponentData(entity, new ToukiMeter
            {
                m_cross = EnumCrossType.None,
                m_value = 0,
            });

            entityManager.SetComponentData(entity, new Status
            {
                m_life = Settings.Instance.Common.LifeMax,
                m_balance = Settings.Instance.Common.BalanceMax,
                m_rei = Settings.Instance.Common.ReiMax,
            });

            entityManager.SetComponentData(entity, new JumpState
            {
                m_state = EnumJumpState.None,
                m_effectStep = EnumJumpEffectStep.JumpStart,
                m_totalCount = 0,
            });

            entityManager.SetComponentData(entity, new DamageState
            {
            });

            entityManager.SetComponentData(entity, new ReiState
            {
            });



            return entity;
        }
    }
}
