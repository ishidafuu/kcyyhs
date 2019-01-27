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
// 	/// モーション変更システム
// 	/// </summary>
// 	public class ShiftMotionSystem : JobComponentSystem
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
// 					typePadInput = GetArchetypeChunkComponentType<PadInput>(true),
// 			};
// 			return job.Schedule(group, inputDeps);
// 		}

// 		// [BurstCompileAttribute]
// 		//Sharedを使っているのでバーストできない
// 		//事前にNativeArrayにコピーすれば良いとは思う
// 		struct CountJob : IJob
// 		{
// 			public float deltaTime;

// 			public ArchetypeChunkComponentType<CharaMotion> typeCharaMotion;

// 			public ArchetypeChunkComponentType<PadInput> typePadInput;

// 			public void Execute(ArchetypeChunk chunk, int chunkIndex)
// 			{
// 				var array = chunk.GetNativeArray(typeCharaMotion);
// 				if (chunk.Has(typeCharaMotion))
// 					ProcessCharaMotion(array);

// 				if (chunk.Has(typePadInput))
// 					ProcessCharaMotion(array);
// 			}

// 			void ProcessCharaMotion(NativeArray<CharaMotion> array)
// 			{
// 				for (int i = 0; i < array.Length; i++)
// 				{
// 					CharaMotion motion = array[i];

// 					//モーションごとの入力
// 					switch (motion.motionType)
// 					{
// 						case EnumMotion.Idle:
// 							UpdateIdle(motion);
// 							break;
// 						case EnumMotion.Walk:
// 							UpdateWalk();
// 							break;
// 						case EnumMotion.Run:
// 							UpdateRun();
// 							break;
// 						case EnumMotion.Slip:
// 							UpdateSlip();
// 							break;
// 						case EnumMotion.Jump:
// 							UpdateJump();
// 							break;
// 						case EnumMotion.Fall:
// 							UpdateFall();
// 							break;
// 						case EnumMotion.Land:
// 							UpdateLand();
// 							break;
// 						case EnumMotion.Damage:
// 							UpdateDamage();
// 							break;
// 						case EnumMotion.Fly:
// 							UpdateFly();
// 							break;
// 						case EnumMotion.Down:
// 							UpdateDown();
// 							break;
// 						case EnumMotion.Dead:
// 							UpdateDead();
// 							break;
// 						case EnumMotion.Action:
// 							UpdateAction();
// 							break;
// 						default:
// 							Debug.Assert(false);
// 							break;
// 					}

// 					array[i] = motion;
// 				}
// 			}

// 			void UpdateIdle(CharaMotion motion)
// 			{

// 			}
// 		}
// 	}
// }