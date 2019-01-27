// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.Experimental.PlayerLoop;

// namespace NKKD
// {

// 	//行動変化
// 	public class ChangeBehaveSystem : ComponentSystem
// 	{
// 		struct Group
// 		{
// 			public readonly int Length;
// 			public EntityArray entities;

// 			public ComponentDataArray<CharaLook> look;
// 			public ComponentDataArray<CharaBehave> behave;
// 			public ComponentDataArray<CharaMotion> motion;
// 			[ReadOnly] public ComponentDataArray<Position> position;

// 		}

// 		[Inject] Group group;

// 		[ComputeJobOptimization]
// 		struct PositionJob : IJobParallelFor
// 		{
// 			public ComponentDataArray<CharaLook> look;
// 			public ComponentDataArray<CharaBehave> behave;
// 			public ComponentDataArray<CharaMotion> motion;
// 			[ReadOnly] public ComponentDataArray<Position> position;
// 			[ReadOnly] public float realtimeSinceStartup;
// 			[ReadOnly] public NativeArray<float> vecx;
// 			[ReadOnly] public NativeArray<float> vecy;
// 			[ReadOnly] public NativeArray<float> rand;
// 			public void Execute(int i)
// 			{

// 				switch (behave[i].behaveType)
// 				{
// 					case (int)EnumBehave.Idle:
// 						if (realtimeSinceStartup > behave[i].endTime)
// 						{
// 							CharaBehave resBehave = behave[i];
// 							CharaLook resLook = look[i];
// 							CharaMotion resMotion = motion[i];
// 							//float randomValue = Mathf.PerlinNoise(position[i].Value.x, position[i].Value.y) - 0.5f;
// 							//float randomValue2 = Mathf.PerlinNoise(position[i].Value.y, position[i].Value.x) - 0.5f;
// 							//var asdf = Random.Range(0, 100 * 20);

// 							resBehave.behaveType = (int)EnumBehave.Wander;
// 							resBehave.endTime = (realtimeSinceStartup + 0.5f + rand[i % 100] * 5);
// 							var nextAngle = rand[(i + 1) % 100];
// 							if (nextAngle < 0.33)
// 							{
// 								if (behave[i].angle < 11)
// 								{
// 									resBehave.angle += 1;
// 								}
// 								else
// 								{
// 									resBehave.angle = 0;
// 								}
// 							}
// 							else if (nextAngle < 0.66)
// 							{
// 								if (behave[i].angle > 0)
// 								{
// 									resBehave.angle -= 1;
// 								}
// 								else
// 								{
// 									resBehave.angle = 11;
// 								}
// 							}
// 							resBehave.targetVecNrm.x = vecx[behave[i].angle];
// 							resBehave.targetVecNrm.y = vecy[behave[i].angle];

// 							//向き
// 							if ((behave[i].angle >= 1) && (behave[i].angle <= 5))
// 							{
// 								resLook.isLeft = 0;
// 							}
// 							else if ((behave[i].angle >= 7) && (behave[i].angle <= 11))
// 							{
// 								resLook.isLeft = 1;
// 							}

// 							if ((behave[i].angle >= 3) && (behave[i].angle <= 9))
// 							{
// 								resLook.isBack = 0;
// 							}
// 							else
// 							{
// 								resLook.isBack = 1;
// 							}
// 							behave[i] = resBehave;
// 							look[i] = resLook;

// 							resMotion.count = 0;
// 							resMotion.motionType = EnumMotion.Walk;
// 							motion[i] = resMotion;
// 						}
// 						break;
// 					case (int)EnumBehave.Wander:
// 						if (realtimeSinceStartup > behave[i].endTime)
// 						{
// 							CharaBehave resBehave;
// 							ToIdle(behave[i], position[i], rand[i % 100], out resBehave);
// 							behave[i] = resBehave;
// 							CharaMotion resMotion = motion[i];
// 							resMotion.count = 0;
// 							resMotion.motionType = EnumMotion.Idle;
// 							motion[i] = resMotion;
// 						}
// 						break;
// 				}

// 			}

// 			public void ToIdle(CharaBehave behave, Position position, float randValue,
// 				out CharaBehave resBehave)
// 			{
// 				resBehave = behave;
// 				float randomValue = Mathf.PerlinNoise(position.Value.x, position.Value.y) - 0.5f;
// 				resBehave.behaveType = (int)EnumBehave.Idle;
// 				resBehave.endTime = (realtimeSinceStartup + 0.5f + randValue * 3);
// 			}
// 		}

// 		protected override void OnUpdate()
// 		{
// 			float[] vecxf = { 0, 0.5f, 0.867f, 1f, 0.867f, 0.5f, 0, -0.5f, -0.867f, -1f, -0.867f, -0.5f };
// 			float[] vecyf = { 1f, 0.867f, 0.5f, 0, -0.5f, -0.867f, -1f, -0.867f, -0.5f, 0, 0.5f, 0.867f };
// 			var vecx = new NativeArray<float>(vecxf, Allocator.TempJob);
// 			var vecy = new NativeArray<float>(vecyf, Allocator.TempJob);

// 			var rand = new NativeArray<float>(100, Allocator.TempJob);
// 			for (int i = 0; i < 100; i++)rand[i] = Random.value;

// 			//Random.value
// 			var job = new PositionJob()
// 			{
// 				behave = group.behave,
// 					look = group.look,
// 					position = group.position,
// 					motion = group.motion,
// 					realtimeSinceStartup = Time.realtimeSinceStartup,
// 					vecy = vecy,
// 					vecx = vecx,
// 					rand = rand,
// 			};
// 			var jobHandle = job.Schedule(group.Length, 50);
// 			jobHandle.Complete();

// 			vecx.Dispose();
// 			vecy.Dispose();
// 			rand.Dispose();
// 		}
// 	}
// }