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
