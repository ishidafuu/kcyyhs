﻿using Unity.Entities;

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

            entityManager.SetComponentData(entity, new StateMeter
            {
                m_life = Settings.Instance.Common.LifeMax / 3,
                m_balance = Settings.Instance.Common.BalanceMax / 2,
                m_rei = Settings.Instance.Common.ReiMax / 2,
            });

            entityManager.SetComponentData(entity, new JumpState
            {
                m_state = EnumJumpState.None,
                m_effectStep = EnumJumpEffectStep.JumpStart,
                m_totalCount = 0,
            });


            return entity;
        }
    }
}
