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
    public static class BattleEntityFactory
    {
        public static Entity CreateEntity(EntityManager entityManager, ref YHFilterEffectList effectList)
        {
            var archetype = entityManager.CreateArchetype(ComponentTypes.BattleComponentType);
            var entity = entityManager.CreateEntity(archetype);

            BattleSequencer seq = new BattleSequencer();
            seq.sideA.isSideA = true;
            seq.sideB.isSideA = false;
            entityManager.SetComponentData(entity, seq);


            return entity;
        }
    }
}
