using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
	public class ToukiMeterSwitchJobSystem : JobComponentSystem
	{
		ComponentGroup m_group;

		protected override void OnCreateManager()
		{
			m_group = GetComponentGroup(
				ComponentType.ReadOnly<PadInput>(),
				ComponentType.Create<ToukiMeter>()
			);

		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			m_group.AddDependency(inputDeps);

			var padInputs = m_group.ToComponentDataArray<PadInput>(Allocator.TempJob, out JobHandle handle1);
			var toukiMeters = m_group.ToComponentDataArray<ToukiMeter>(Allocator.TempJob, out JobHandle handle2);
			inputDeps = JobHandle.CombineDependencies(handle1, handle2);

			var job = new InputToToukiJob()
			{
				padInputs = padInputs,
					toukiMeters = toukiMeters,
			};
			inputDeps = job.Schedule(inputDeps);

			m_group.AddDependency(inputDeps);
			m_group.CopyFromComponentDataArray(toukiMeters, out JobHandle handle3);
			// inputDeps.Complete();

			inputDeps = new ReleaseJob
			{
				toukiMeters = toukiMeters
			}.Schedule(handle3);

			return inputDeps;
		}

		struct ReleaseJob : IJob
		{
			[DeallocateOnJobCompletion]
			public NativeArray<ToukiMeter> toukiMeters;

			public void Execute() {}
		}

		[BurstCompileAttribute]
		struct InputToToukiJob : IJob
		{
			[ReadOnly]
			[DeallocateOnJobCompletion]
			public NativeArray<PadInput> padInputs;

			public NativeArray<ToukiMeter> toukiMeters;

			public void Execute()
			{
				for (int i = 0; i < padInputs.Length; i++)
				{
					var toukiMeter = toukiMeters[i];

					if (toukiMeter.state != EnumToukiMaterState.Active)
					{
						break;
					}

					if (toukiMeter.muki != padInputs[i].GetPressCross())
					{
						toukiMeter.muki = padInputs[i].GetPressCross();
						toukiMeter.value = 0;
					}
					toukiMeters[i] = toukiMeter;
				}
			}

		}

	}
}