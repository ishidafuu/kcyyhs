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
				EnumUnityButtonType.Fire1.ToString(),
					EnumUnityButtonType.Fire2.ToString(),
					EnumUnityButtonType.Fire3.ToString(),
					EnumUnityButtonType.Fire4.ToString(),
					EnumUnityButtonType.Fire5.ToString(),
					EnumUnityButtonType.Fire6.ToString(),
			});
		/// <summary>
		/// 更新
		/// </summary>
		protected override void OnUpdate()
		{
			var padInputs = m_group.ToComponentDataArray<PadInput>(Allocator.TempJob);
			for (int i = 0; i < padInputs.Length; i++)
			{
				var padInput = padInputs[i];
				string player = "P" + i.ToString();
				SetCross(ref padInput, player);
				SetButton(ref padInput, player);
				padInputs[i] = padInput;
			}
			m_group.CopyFromComponentDataArray(padInputs);
			padInputs.Dispose();

		}

		void SetCross(ref PadInput padInput, string player)
		{
			var nowAxis = new Vector2(Input.GetAxis(player + "Horizontal"), Input.GetAxis(player + "Vertical"));
			padInput.SetCross(nowAxis, Time.time);
			// if (nowAxis != Vector2.zero)
			// 	Debug.Log(nowAxis);
		}

		void SetButton(ref PadInput padInput, string player)
		{
			foreach (EnumUnityButtonType item in Enum.GetValues(typeof(EnumUnityButtonType)))
			{
				var buttonName = player + ButtonTypeName[(int)item];
				var isPush = Input.GetButtonDown(buttonName);
				var isPress = Input.GetButton(buttonName);
				var isPop = Input.GetButtonUp(buttonName);

				switch (item)
				{
					case EnumUnityButtonType.Fire1:
						padInput.buttonA.SetButtonData(isPush, isPress, isPop, Time.time);
						break;
					case EnumUnityButtonType.Fire2:
						padInput.buttonB.SetButtonData(isPush, isPress, isPop, Time.time);
						break;
					case EnumUnityButtonType.Fire3:
						padInput.buttonX.SetButtonData(isPush, isPress, isPop, Time.time);
						break;
					case EnumUnityButtonType.Fire4:
						padInput.buttonY.SetButtonData(isPush, isPress, isPop, Time.time);
						break;
				}
				if (isPush)
					Debug.Log(buttonName);
			}
		}
	}
}