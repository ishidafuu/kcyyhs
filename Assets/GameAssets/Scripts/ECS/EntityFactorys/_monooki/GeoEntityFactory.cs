//using UnityEngine;
//using Unity.Entities;
//using Unity.Mathematics;
//using Unity.Transforms;
//using Unity.Transforms2D;
//using Unity.Collections;
////using toinfiniityandbeyond.Rendering2D;
//using UnityEditor;
//using UnityEngine.SceneManagement;
//using System.Collections.Generic;
//using System.Linq;
//using Unity.Jobs;
//using Unity.Rendering;

//namespace NKKD {
//	public static class GeoEntityFactory {

//		public static List<NativeArray<Matrix4x4>> matrixList;

//		public static void Init() {
//			matrixList = new List<NativeArray<Matrix4x4>>();
//		}

//		public static void Dispose() {
//			foreach (var item in matrixList) {
//				item.Dispose();
//			}
//		}

//		//地面エンティティ作成
//		public static Entity CreateEntity(int i, EntityManager entityManager, List<Entity> surfaceEntity, int TIPSIZE) {

//			var geoArchetype = entityManager.CreateArchetype(EntityArchetype.GeoType);
//			var entity = entityManager.CreateEntity(geoArchetype);
//			//ComponentDataのセット
//			var geoMeshMat = new GeoMeshMat {
//				material = Shared.geoMeshMat.materials[i],
//				mesh = Shared.geoMeshMat.meshs[i],
//			};
//			geoMeshMat.matrix = new NativeArray<Matrix4x4>(surfaceEntity.Count, Allocator.Persistent);
//			matrixList.Add(geoMeshMat.matrix);

//			for (int i2 = 0; i2 < surfaceEntity.Count; i2++) {
//				var tipPos = entityManager.GetComponentData<TipPos>(surfaceEntity[i2]);
//				geoMeshMat.matrix[i2] = Matrix4x4.TRS(new Vector3(tipPos.Value.x * TIPSIZE, 0, tipPos.Value.y * TIPSIZE),
//					Quaternion.identity,
//					Vector3.one);
//			}
//			entityManager.SetComponentData(entity, new GeoObj());
//			entityManager.SetSharedComponentData(entity, geoMeshMat);

//			return entity;
//		}

//	}
//}