// using UnityEngine;
// using Unity.Entities;
// using Unity.Mathematics;
// using Unity.Transforms;
// using Unity.Transforms2D;
// using Unity.Collections;
// //using toinfiniityandbeyond.Rendering2D;
// using UnityEditor;
// using UnityEngine.SceneManagement;
// using System.Collections.Generic;
// using System.Linq;
// using Unity.Jobs;

// namespace NKKD {
// 	public static class FoodEntityFactory {

// 		//アリエンティティ作成
// 		public static Entity CreateEntity(int i, EntityManager entityManager,
// 			ref MeshMatList foodMeshMat,
// 			Vector3 pos, int foodType
// 			) {
// 			var foodArchetype = entityManager.CreateArchetype(EntityArchetype.FoodType);

// 			var entity = entityManager.CreateEntity(foodArchetype);

// 			entityManager.SetComponentData(entity, new FoodObj());
// 			//ComponentDataのセット
// 			//var posL = Define.Instance.GetMapSize() / 2;
// 			//var posH = Define.Instance.GetMapSize() / 2;
// 			//位置
// 			entityManager.SetComponentData(entity, new Position {
// 				Value = new float3(pos.x, pos.y, pos.z)
// 			});

// 			//データ
// 			//Debug.Log(foodType);
// 			entityManager.SetComponentData(entity, new FoodData {
// 				foodType = foodType,
// 				volume = 100,
// 			});

// 			//SharedComponentDataのセット
// 			entityManager.AddSharedComponentData(entity, foodMeshMat);

// 			//Debug.Log(i);

// 			return entity;
// 		}
// 	}
// }