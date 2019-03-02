using System;
using System.Collections.Generic;
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
		enum EnumUnityButtonType
		{
			Fire1,
			Fire2,
			Fire3,
			Fire4,
			Fire5,
			Fire6,
		}

		ComponentGroup m_group;
		ReadOnlyCollection<string>[] ButtonTypeName;
		ReadOnlyCollection<string> HorizontalName;
		ReadOnlyCollection<string> VerticalName;

		protected override void OnCreateManager()
		{
			m_group = GetComponentGroup(
				ComponentType.Create<PadInput>());

			var playerNum = Define.Instance.Common.PlayerNum;
			var tpmPlayerNames = new List<string>();
			for (int i = 0; i < playerNum; i++)
			{
				tpmPlayerNames.Add($"P{i}");
			}

			InitButtonTypeName(tpmPlayerNames);
			InitHolizontalName(tpmPlayerNames);
			InitVerticalName(tpmPlayerNames);
		}

		private void InitButtonTypeName(List<string> tpmPlayerNames)
		{
			var ButtonNum = Define.Instance.Common.ButtonNum;
			ButtonTypeName = new ReadOnlyCollection<string>[ButtonNum];

			var buttonNames = new List<string>();
			for (int i = 0; i < ButtonNum; i++)
			{
				buttonNames.Add($"Fire{i + 1}");
			}

			for (int i = 0; i < ButtonNum; i++)
			{
				var playerButtonNames = new List<string>();
				foreach (var item in tpmPlayerNames)
				{
					playerButtonNames.Add($"{item}{buttonNames[i]}");
				}
				ButtonTypeName[i] = Array.AsReadOnly(playerButtonNames.ToArray());
			}
		}

		private void InitHolizontalName(List<string> tpmPlayerNames)
		{
			var tmpHorizontalNames = new List<string>();
			foreach (var item in tpmPlayerNames)
			{
				tmpHorizontalNames.Add($"{item}Horizontal");
			}
			HorizontalName = Array.AsReadOnly(tmpHorizontalNames.ToArray());
		}

		private void InitVerticalName(List<string> tpmPlayerNames)
		{
			var tmpVerticalNames = new List<string>();
			foreach (var item in tpmPlayerNames)
			{
				tmpVerticalNames.Add($"{item}Vertical");
			}
			VerticalName = Array.AsReadOnly(tmpVerticalNames.ToArray());
		}

		protected override void OnUpdate()
		{
			var padInputs = m_group.ToComponentDataArray<PadInput>(Allocator.TempJob);
			for (int i = 0; i < padInputs.Length; i++)
			{
				var padInput = padInputs[i];
				SetCross(ref padInput, i);
				SetButton(ref padInput, i);
				padInputs[i] = padInput;
			}
			m_group.CopyFromComponentDataArray(padInputs);
			padInputs.Dispose();

		}

		void SetCross(ref PadInput padInput, int playerNo)
		{
			var nowAxis = new Vector2(Input.GetAxis(HorizontalName[playerNo]), Input.GetAxis(VerticalName[playerNo]));
			padInput.SetCross(nowAxis, Time.time);
			// if (nowAxis != Vector2.zero)
			// 	Debug.Log(nowAxis);
		}

		void SetButton(ref PadInput padInput, int playerNo)
		{
			var ButtonNum = Define.Instance.Common.ButtonNum;

			for (int i = 0; i < ButtonNum; i++)
			{
				var isPush = Input.GetButtonDown(ButtonTypeName[i][playerNo]);
				var isPress = Input.GetButton(ButtonTypeName[i][playerNo]);
				var isPop = Input.GetButtonUp(ButtonTypeName[i][playerNo]);

				switch ((EnumUnityButtonType)i)
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
				// if (isPush)
				// 	Debug.Log(buttonName);
			}
		}
	}
}