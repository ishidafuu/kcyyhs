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
// using Unity.Rendering;

// namespace NKKD {
// 	public static class MapTipEntityFactory {
// 		public static List<int> surfType;
// 		//マップエンティティ作成
// 		public static void MakeMap(int mapSize, int tipTypeNum) {

// 			surfType = new List<int>(mapSize * mapSize);
// 			float fDiv = 50f;
// 			float xf = Random.value * mapSize;
// 			float yf = Random.value * mapSize;
// 			for (int x = 0; x < mapSize; x++) {
// 				for (int y = 0; y < mapSize; y++) {
// 					int index = (y * mapSize) + x;
// 					float fRand = Random.value;
// 					float noise = Mathf.PerlinNoise((x + xf) / fDiv, (y + yf) / fDiv);
// 					for (int i = 0; i < tipTypeNum; i++) {
// 						// 0~1の間の値であとは自由に分岐して配置	
// 						 if ((noise <= ((float)(i + 1) / tipTypeNum))
// 							|| (i == (tipTypeNum - 1))) {
// 							surfType.Add(i);
// 							break;
// 						}
// 					}

// 				}
// 			}
// 			//Debug.Log(surfType.Count);
// 		}

// 		//マップエンティティ作成
// 		public static Entity CreateEntity(int i, EntityManager entityManager, int mapSize, int tipSize) {
// 			var maptipArchetype = entityManager.CreateArchetype(EntityArchetype.MapTipType);
// 			var entity = entityManager.CreateEntity(maptipArchetype);
// 			//ComponentDataのセット
// 			//チップのインデックス座標
// 			entityManager.SetComponentData(entity, new GridPos {
// 				Value = new Vector2Int((i / mapSize), (i % mapSize))
// 			});

// 			//entityManager.SetComponentData(entity, new Position2D {
// 			//	Value = new float2((i / mapSize) * tipSize, (i % mapSize) * tipSize)
// 			//});

// 			//ComponentType.FixedArray(typeof(PolygonId), 10)

// 			entityManager.SetComponentData(entity, new TipSurface {
// 				surfType = surfType[i]
// 			});

// 			entityManager.SetComponentData(entity, new TipPheromL());
// 			entityManager.SetComponentData(entity, new TipPheromH());
// 			//entityManager.SetComponentData(entity, new TransformMatrix ());

// 			//// 渡すマテリアルはGPU Instancingに対応させる必要がある
// 			//var meshInstanceRenderer = new MeshInstanceRenderer();
// 			//meshInstanceRenderer.mesh = Shared.tipMeshMat.meshs[surfType[i]];
// 			//meshInstanceRenderer.material = Shared.tipMeshMat.materials[surfType[i]];
// 			//entityManager.SetSharedComponentData(entity, meshInstanceRenderer);
// 			//entityManager.AddSharedComponentData(entity, Shared.tipMeshMat);
// 			return entity;
// 		}

// 	}
// }