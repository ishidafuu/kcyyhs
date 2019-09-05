using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
// using Unity.Transforms2D;
using Unity.Collections;
// using toinfiniityandbeyond.Rendering2D;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.SceneManagement;
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
                charaNo = 0,
                isSideA = (i == 0),
            });

            // 闘気メーター
            entityManager.SetComponentData(entity, new ToukiMeter
            {
                m_muki = EnumCrossType.None,
                m_value = 0,
            });

            return entity;
        }
    }
}
