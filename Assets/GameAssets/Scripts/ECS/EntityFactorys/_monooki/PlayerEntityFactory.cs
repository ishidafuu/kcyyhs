// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;
// //using Unity.Transforms2D;
// using Unity.Collections;
// //using toinfiniityandbeyond.Rendering2D;
// using System.Collections.Generic;
// using System.Linq;
// using Unity.Jobs;
// using UnityEditor;
// using UnityEngine.SceneManagement;

// namespace NKKD
// {
// 	public static class PlayerEntityFactory
// 	{

// 		/// <summary>
// 		/// プレーヤーエンティティ作成
// 		/// </summary>
// 		/// <param name="i"></param>
// 		/// <param name="entityManager"></param>
// 		/// <param name="ariMeshMat"></param>
// 		/// <param name="aniScriptSheet"></param>
// 		/// <param name="aniBasePos"></param>
// 		/// <returns></returns>
// 		public static Entity CreateEntity(int i, EntityManager entityManager)
// 		{
// 			var archetype = entityManager.CreateArchetype(ComponentTypes.PlayerComponentType);
// 			var entity = entityManager.CreateEntity(archetype);
// 			//Tag
// 			// entityManager.SetComponentData(entity, new PlayerTag());

// 			//ID
// 			entityManager.SetComponentData(entity, new PlayerId
// 			{
// 				myId = i
// 			});

// 			return entity;
// 		}
// 	}
// }