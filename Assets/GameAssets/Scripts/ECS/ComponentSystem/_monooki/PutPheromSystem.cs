// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.Experimental.PlayerLoop;

// namespace NKKD
// {

// 	//フェロモンおいてく
// 	public class PutPheromSystem : ComponentSystem
// 	{
// 		struct CharaGroup
// 		{
// 			public int Length;
// 			public EntityArray entities;
// 			public ComponentDataArray<GridPos> ariGridPos;
// 			[ReadOnly] public ComponentDataArray<CharaObj> obj;
// 			[ReadOnly] public ComponentDataArray<Position> position;
// 		}

// 		[Inject] CharaGroup ariGroup;

// 		struct TipGroup
// 		{
// 			public int Length;
// 			public EntityArray entities;
// 			public ComponentDataArray<TipPheromL> tipPheromL;
// 			public ComponentDataArray<GridPos> tipGridPos;
// 			[ReadOnly] public ComponentDataArray<TipObj> obj;
// 		}

// 		[Inject] TipGroup tipGroup;

// 		[ComputeJobOptimization]
// 		struct PutJob : IJob
// 		{
// 			public ComponentDataArray<TipPheromL> tipPheromL;
// 			public ComponentDataArray<GridPos> ariGridPos;
// 			public NativeArray<Vector3Int> putPos;
// 			public NativeArray<int> putInfo;
// 			public NativeArray<int> putNum;
// 			[ReadOnly] public int gridSize;
// 			[ReadOnly] public int mapSize;
// 			[ReadOnly] public ComponentDataArray<Position> position;
// 			//[ReadOnly] public ComponentDataArray<GridPos> tipGridPos;

// 			public void Execute()
// 			{
// 				int tmpPutNum = 0;
// 				for (int i = 0; i < position.Length; i++)
// 				{
// 					int x = (int)(position[i].Value.x / gridSize);
// 					int y = (int)(position[i].Value.y / gridSize);
// 					//違うグリッドから来た
// 					if ((ariGridPos[i].Value.x != x) || (ariGridPos[i].Value.y != y))
// 					{
// 						int index = (y * mapSize) + x;
// 						GridPos newGridPos = ariGridPos[i];
// 						TipPheromL newTip = tipPheromL[index];
// 						//フェロモンの強さ
// 						if (newTip.info < 4)newTip.info += 1;
// 						//戻るべき方向（餌に向かう場合は、この向きを逆走する）
// 						newTip.vec = new Vector2Int(ariGridPos[i].Value.x - x, ariGridPos[i].Value.y - y);
// 						tipPheromL[index] = newTip;

// 						//新しくフェロモンチップを配置するための情報
// 						putPos[tmpPutNum] = new Vector3Int(x, y, 0);
// 						putInfo[tmpPutNum] = newTip.info;
// 						tmpPutNum++;

// 						//ここで現在グリッド位置を更新する（座標移動のときしないのはここで直前グリッド情報をつかうため）
// 						newGridPos.Value.x = x;
// 						newGridPos.Value.y = y;
// 						ariGridPos[i] = newGridPos;
// 					}
// 				}
// 				putNum[0] = tmpPutNum;
// 			}
// 		}

// 		protected override void OnUpdate()
// 		{

// 			var job = new PutJob()
// 			{
// 				putPos = new NativeArray<Vector3Int>(ariGroup.Length, Allocator.Temp),
// 					putInfo = new NativeArray<int>(ariGroup.Length, Allocator.Temp),
// 					putNum = new NativeArray<int>(1, Allocator.Temp),
// 					gridSize = Define.Instance.GRID_SIZE,
// 					mapSize = Define.Instance.MAP_GRID_NUM,
// 					position = ariGroup.position,
// 					ariGridPos = ariGroup.ariGridPos,
// 					//tipGridPos = tipGroup.tipGridPos,
// 					tipPheromL = tipGroup.tipPheromL,
// 			};
// 			var jobHandle = job.Schedule();
// 			jobHandle.Complete();

// 			for (int i = 0; i < job.putNum[0]; i++)
// 			{
// 				Cache.pheromMap.SetTile(job.putPos[i], Define.Instance.pheromTile[job.putInfo[i]]);
// 			}

// 			job.putNum.Dispose();
// 			job.putPos.Dispose();
// 			job.putInfo.Dispose();
// 		}
// 	}
// }