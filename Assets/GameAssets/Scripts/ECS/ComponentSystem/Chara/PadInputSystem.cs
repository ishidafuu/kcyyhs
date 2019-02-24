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

namespace YYHS
{
	/// <summary>
	/// 入力システム（Input.GetButtonDownはメインスレッドからのみ呼び出せるのでComponentSystemで呼び出す）
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
				var padInput = padInputs[i];
				string player = "P" + i.ToString();
				SetCross(ref padInput, player);
				SetButton(ref padInput, player);
				padInputs[i] = padInput;
			}
		}

		void SetButton(ref PadInput padInput, string player)
		{
			foreach (EnumButtonType item in Enum.GetValues(typeof(EnumButtonType)))
			{
				var buttonName = player + ButtonTypeName[(int)item];
				var isPush = Input.GetButtonDown(buttonName);
				var isPress = Input.GetButton(buttonName);
				var isPop = Input.GetButtonUp(buttonName);

				switch (item)
				{
					case EnumButtonType.Fire1:
						padInput.buttonA.SetButtonData(isPush, isPress, isPop, Time.time);
						break;
					case EnumButtonType.Fire2:
						padInput.buttonB.SetButtonData(isPush, isPress, isPop, Time.time);
						break;
				}
				if (isPush)
					Debug.Log(buttonName);
			}
		}

		void SetCross(ref PadInput padInput, string player)
		{
			var nowAxis = new Vector2(Input.GetAxis(player + "Horizontal"), Input.GetAxis(player + "Vertical"));
			padInput.SetCross(nowAxis, Time.time);
		}
	}
}