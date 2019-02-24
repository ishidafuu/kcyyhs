using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
	public class ToukiMeterDebubSystem : ComponentSystem
	{
		ComponentGroup m_group;

		protected override void OnCreateManager()
		{
			m_group = GetComponentGroup(
				ComponentType.Create<PadInput>(),
				ComponentType.Create<ToukiMeter>()
			);
		}

		protected override void OnUpdate()
		{
			var toukiMeters = m_group.GetComponentDataArray<ToukiMeter>();
			for (int i = 0; i < toukiMeters.Length; i++)
			{
				var toukiMeter = toukiMeters[i];
				// Debug.Log(toukiMeter.value);
				// Debug.Log(toukiMeter.muki);
				ConsoleProDebug.Watch("ToukiMeter.value", toukiMeter.value.ToString());
				ConsoleProDebug.Watch("ToukiMeter.muki", toukiMeter.muki.ToString());
				toukiMeters[i] = toukiMeter;
			}
		}

	}
}