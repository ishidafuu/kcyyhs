using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
	[UpdateInGroup(typeof(CountGroup))]
	public class ToukiMeterCountJobSystem : JobComponentSystem
	{
		ComponentGroup m_group;

		protected override void OnCreateManager()
		{
			m_group = GetComponentGroup(
				ComponentType.Create<ToukiMeter>()
			);
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			m_group.AddDependency(inputDeps);
			var toukiMeters = m_group.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);

			inputDeps = new UpdateToukiJob()
			{
				toukiMeters = toukiMeters,
			}.Schedule(inputDeps);

			m_group.AddDependency(inputDeps);
			m_group.CopyFromComponentDataArray(toukiMeters, out JobHandle handle3);

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
		struct UpdateToukiJob : IJob
		{
			public NativeArray<ToukiMeter> toukiMeters;

			public void Execute()
			{
				for (int i = 0; i < toukiMeters.Length; i++)
				{
					var toukiMeter = toukiMeters[i];
					if (toukiMeter.muki != EnumCrossType.None)
					{
						toukiMeter.value++;
						if (toukiMeter.value > 100)
						{
							toukiMeter.value = 100;
						}
						// int asdf = toukiMeter.value;
						// Debug.Log(asdf);
					}

					toukiMeters[i] = toukiMeter;
				}
			}

		}

	}
}