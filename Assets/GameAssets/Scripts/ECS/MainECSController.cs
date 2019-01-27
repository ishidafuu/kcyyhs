// using System.Collections.Generic;
// using System.Linq;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.SceneManagement;
// using UnityEngine.Tilemaps;
// using UnityEngine.U2D;
// namespace NKKD
// {
// 	public class MainECSController : MonoBehaviour
// 	{
// 		List<Entity> entityList;
// 		EntityManager entityManager;
// 		const int ARINUM = 10;
// 		const int FOODNUM = 1000;
// 		//ここから起動
// 		private void Start()
// 		{
// 			//シーンの判定
// 			var scene = SceneManager.GetActiveScene();
// 			if (scene.name != "ECSSampleScene")
// 				return;

// 			//エンティティリスト作成
// 			entityList = new List<Entity>();

// 			// マネージャーを取得し、作るEntityのアーキタイプを作成
// 			entityManager = World.Active.GetOrCreateManager<EntityManager>();

// 			//SharedComponentDataの準備
// 			ReadySharedComponentData();

// 			//コンポーネントのキャッシュ
// 			ComponentCache();

// 			//アリ作成
// 			CreateCharaEntity();

// 			// //マップチップ作成
// 			// CreateMapTipEntity();

// 			// //食べ物作成
// 			// CreateFoodEntity();

// 			//マップタイル作成
// 			//Cache.CreateMapTile();

// 		}

// 		//各コンポーネントのキャッシュ
// 		void ComponentCache()
// 		{
// 			Cache.pixelPerfectCamera = FindObjectOfType<PixelPerfectCamera>();
// 			//Cache.tilemap = FindObjectOfType<Tilemap>();
// 			// var tileMaps = FindObjectsOfType<Tilemap>();
// 			// foreach (var item in tileMaps)
// 			// {
// 			// 	//Debug.Log(item.layoutGrid.name);
// 			// 	if (item.layoutGrid.name == "PheromGrid")
// 			// 	{
// 			// 		Cache.pheromMap = item;
// 			// 		Cache.pheromMap.ClearAllTiles();
// 			// 		Cache.pheromMap.size = new Vector3Int(Define.Instance.GRID_SIZE, Define.Instance.GRID_SIZE, 0);
// 			// 	}
// 			// }
// 		}

// 		//SharedComponentDataの読み込み
// 		void ReadySharedComponentData()
// 		{
// 			Shared.ReadySharedComponentData();
// 		}

// 		//アリエンティティ作成
// 		void CreateCharaEntity()
// 		{
// 			//アリエンティティ作成
// 			for (int i = 0; i < ARINUM; i++)
// 			{
// 				var entity = CharaEntityFactory.CreateEntity(i, entityManager,
// 					ref Shared.charaMeshMat, ref Shared.aniScriptSheet, ref Shared.aniBasePos);
// 				//エンティティリストに追加
// 				entityList.Add(entity);
// 			}
// 		}

// 		// //マップチップ作成
// 		// void CreateMapTipEntity()
// 		// {
// 		// 	var MAP_GRID_NUM = Define.Instance.MAP_GRID_NUM;
// 		// 	var maptipArchetype = entityManager.CreateArchetype(EntityArchetype.MapTipType);

// 		// 	var tipTypeNum = Shared.geoMeshMat.materials.Count;
// 		// 	//Debug.Log(tipTypeNum);
// 		// 	MapTipEntityFactory.MakeMap(MAP_GRID_NUM, tipTypeNum);

// 		// 	//List<List<Entity>> surfaceEntity = new List<List<Entity>>();
// 		// 	//for (int i = 0; i < tipTypeNum; i++) {
// 		// 	//	surfaceEntity.Add(new List<Entity>());
// 		// 	//}

// 		// 	var allTipNum = MAP_GRID_NUM * MAP_GRID_NUM;
// 		// 	//マップチップエンティティ作成
// 		// 	for (int i = 0; i < allTipNum; i++)
// 		// 	{
// 		// 		var entity = MapTipEntityFactory.CreateEntity(i, entityManager, MAP_GRID_NUM, MAP_GRID_NUM);
// 		// 		//var surface = entityManager.GetComponentData<TipSurface>(entity);
// 		// 		//surfaceEntity[surface.surfType].Add(entity);
// 		// 		//エンティティリストに追加
// 		// 		entityList.Add(entity);
// 		// 	}

// 		// 	//GeoEntityFactory.Init();
// 		// 	//for (int i = 0; i < tipTypeNum; i++) {
// 		// 	//	var entity = GeoEntityFactory.CreateEntity(i, entityManager, surfaceEntity[i], TIPSIZE);
// 		// 	//	//エンティティリストに追加
// 		// 	//	entityList.Add(entity);
// 		// 	//}

// 		// }

// 		// //食べ物エンティティ作成
// 		// void CreateFoodEntity()
// 		// {
// 		// 	var MAP_GRID_NUM = Define.Instance.MAP_GRID_NUM;
// 		// 	var GRID_SIZE = Define.Instance.GRID_SIZE;
// 		// 	float xf = Random.value * (float)MAP_GRID_NUM;
// 		// 	float yf = Random.value * (float)MAP_GRID_NUM;
// 		// 	var fDiv = 10;
// 		// 	var i = 0;
// 		// 	for (int x = 0; x < MAP_GRID_NUM; x++)
// 		// 	{
// 		// 		for (int y = 0; y < MAP_GRID_NUM; y++)
// 		// 		{

// 		// 			float noise = Mathf.PerlinNoise(((float)x + xf) / fDiv, ((float)y + yf) / fDiv);
// 		// 			//Debug.Log(noise);

// 		// 			if (noise < 0.8f)continue;

// 		// 			var entity = FoodEntityFactory.CreateEntity(i, entityManager,
// 		// 				ref Shared.foodMeshMat, new Vector3(x * GRID_SIZE, y * GRID_SIZE, 0), Random.Range(0, 9));

// 		// 			i++;

// 		// 			if (i > FOODNUM)
// 		// 			{
// 		// 				Debug.Log(i);
// 		// 				return;
// 		// 			}
// 		// 			//surfType.Add(surf);
// 		// 		}
// 		// 	}

// 		// 	//Debug.Log(i);
// 		// }

// 		void OnDestroy()
// 		{
// 			//aniFrames.frames.Dispose();
// 			//aniIndexs.indexs.Dispose();
// 			//GeoEntityFactory.Dispose();
// 			// 作ったEntityを削除
// 			if (World.Active != null)
// 			{
// 				entityManager = World.Active.GetExistingManager<EntityManager>();
// 				foreach (var item in entityList)
// 				{
// 					entityManager.DestroyEntity(item);
// 				}
// 			}
// 		}
// 	}
// }