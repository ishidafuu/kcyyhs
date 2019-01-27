// using System;
// using System.Collections.ObjectModel;
// using HedgehogTeam.EasyTouch;
// using Unity.Burst;
// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
// using UnityEngine;

// namespace NKKD
// {
// 	/// <summary>
// 	/// モーションの時間進行システム
// 	/// </summary>
// 	public class CountMotionJobSystem : JobComponentSystem
// 	{
// 		ComponentGroup group;

// 		protected override void OnCreateManager()
// 		{
// 			group = GetComponentGroup(new EntityArchetypeQuery()
// 			{
// 				None = Array.Empty<ComponentType>(),
// 					Any = Array.Empty<ComponentType>(),
// 					All = new []
// 					{
// 						//キャラモーション
// 						ComponentType.Create<CharaMotion>()
// 					},
// 			});
// 		}

// 		protected override JobHandle OnUpdate(JobHandle inputDeps)
// 		{
// 			var job = new CountJob()
// 			{
// 				typeCharaMotion = GetArchetypeChunkComponentType<CharaMotion>(false),
// 			};
// 			return job.Schedule(group, inputDeps);
// 		}

// 		// [BurstCompileAttribute]
// 		//Sharedを使っているのでバーストできない
// 		//事前にNativeArrayにコピーすれば良いとは思う
// 		struct CountJob : IJobChunk
// 		{
// 			public float deltaTime;

// 			public ArchetypeChunkComponentType<CharaMotion> typeCharaMotion;

// 			public void Execute(ArchetypeChunk chunk, int chunkIndex)
// 			{
// 				var array = chunk.GetNativeArray(typeCharaMotion);
// 				if (chunk.Has(typeCharaMotion))
// 					ProcessCharaMotion(array);
// 			}

// 			void ProcessCharaMotion(NativeArray<CharaMotion> array)
// 			{
// 				for (int i = 0; i < array.Length; i++)
// 				{
// 					CharaMotion motion = array[i];
// 					var framesCount = Shared.aniScriptSheet.scripts[(int)motion.motionType].frames.Count;

// 					motion.count++;
// 					motion.totalCount++;
// 					//４カウントで１アニメカウント
// 					if ((motion.count >> 2) >= framesCount)
// 					{
// 						motion.count = 0;
// 					}
// 					array[i] = motion;
// 				}
// 			}
// 		}
// 	}
// }