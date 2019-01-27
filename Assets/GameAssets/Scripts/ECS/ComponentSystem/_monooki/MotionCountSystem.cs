// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using UnityEngine;
// using UnityEngine.Experimental.PlayerLoop;

// namespace NKKD
// {
// 	//モーションのカウント進行
// 	public class MotionCountSystem : ComponentSystem
// 	{
// 		struct Group
// 		{
// 			public readonly int Length;
// 			//public EntityArray entities;
// 			public ComponentDataArray<CharaMotion> motion;
// 			[ReadOnly] public SharedComponentDataArray<AniScriptSheet> aniScriptSheet;
// 		}

// 		[Inject] Group group;

// 		[ComputeJobOptimization]
// 		struct PositionJob : IJobParallelFor
// 		{
// 			public ComponentDataArray<CharaMotion> motion;
// 			[ReadOnly] public NativeArray<int> framesCount;
// 			public void Execute(int i)
// 			{
// 				CharaMotion res = motion[i];
// 				res.count++;
// 				if ((res.count >> 2) >= framesCount[i])
// 				{
// 					res.count = 0;
// 				}
// 				motion[i] = res;
// 			}
// 		}

// 		protected override void OnUpdate()
// 		{
// 			var framesCount = new NativeArray<int>(group.Length, Allocator.TempJob);
// 			for (int i = 0; i < group.Length; i++)
// 			{
// 				framesCount[i] = Shared.aniScriptSheet.scripts[group.motion[i].motionNo].frames.Count;
// 			}

// 			var job = new PositionJob()
// 			{
// 				motion = group.motion,
// 					framesCount = framesCount,
// 			};
// 			var jobHandle = job.Schedule(group.Length, 10);
// 			jobHandle.Complete();
// 			framesCount.Dispose();
// 		}
// 	}
// }