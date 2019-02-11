using System;
using System.Collections.ObjectModel;
using HedgehogTeam.EasyTouch;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace NKKD
{
	/// <summary>
	/// 入力システム
	/// </summary>
	public class PadInputSystem : ComponentSystem
	{
		ComponentGroup m_group;

		protected override void OnCreateManager()
		{
			m_group = GetComponentGroup(
				ComponentType.Create<PadInput>());
		}

		/// <summary>
		/// ボタン名の定数
		/// </summary>
		/// <value></value>
		ReadOnlyCollection<string> ButtonTypeName =
			Array.AsReadOnly(new string[]
			{
				EnumButtonType.Fire1.ToString(),
					EnumButtonType.Fire2.ToString(),
					EnumButtonType.Fire3.ToString(),
					EnumButtonType.Fire4.ToString(),
					EnumButtonType.Fire5.ToString(),
					EnumButtonType.Fire6.ToString(),
			});
		/// <summary>
		/// 更新
		/// </summary>
		protected override void OnUpdate()
		{
			var padInputs = m_group.GetComponentDataArray<PadInput>();
			for (int i = 0; i < padInputs.Length; i++)
			{
				var patInput = padInputs[i];
				string player = "P" + i.ToString();

				//十字
				var nowAxis = new Vector2(Input.GetAxis(player + "Horizontal"), Input.GetAxis(player + "Vertical"));
				patInput.SetCross(nowAxis, Time.time);

				//ボタン
				foreach (EnumButtonType item in Enum.GetValues(typeof(EnumButtonType)))
				{
					var buttonName = player + ButtonTypeName[(int)item];
					var isPush = Input.GetButtonDown(buttonName);
					var isPress = Input.GetButton(buttonName);
					var isPop = Input.GetButtonUp(buttonName);

					switch (item)
					{
						case EnumButtonType.Fire1:
							patInput.buttonA.SetButtonData(isPush, isPress, isPop, Time.time);
							break;
						case EnumButtonType.Fire2:
							patInput.buttonB.SetButtonData(isPush, isPress, isPop, Time.time);
							break;
					}
				}
				padInputs[i] = patInput;
			}
		}
	}
}