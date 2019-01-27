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
// 	/// 入力による状態変化システム
// 	/// </summary>
// 	public class InputMotionSystem : ComponentSystem
// 	{
// 		ComponentGroup group;

// 		protected override void OnCreateManager()
// 		{
// 			group = GetComponentGroup(
// 				ComponentType.Create<CharaMotion>(),
// 				ComponentType.Create<CharaDash>(),
// 				ComponentType.ReadOnly<PadInput>());
// 		}

// 		ComponentDataArray<CharaMotion> charaMotions;
// 		ComponentDataArray<CharaDash> charaDashs;
// 		ComponentDataArray<PadInput> padInputs;

// 		protected override void OnUpdate()
// 		{
// 			charaMotions = group.GetComponentDataArray<CharaMotion>();
// 			charaDashs = group.GetComponentDataArray<CharaDash>();
// 			padInputs = group.GetComponentDataArray<PadInput>();

// 			for (int i = 0; i < charaMotions.Length; i++)
// 			{
// 				//モーションごとの入力
// 				switch (charaMotions[i].motionType)
// 				{
// 					case EnumMotion.Idle:
// 						UpdateIdle(i);
// 						break;
// 					case EnumMotion.Walk:
// 						UpdateWalk(i);
// 						break;
// 					case EnumMotion.Dash:
// 						UpdateDash(i);
// 						break;
// 					case EnumMotion.Slip:
// 						UpdateSlip(i);
// 						break;
// 					case EnumMotion.Jump:
// 						break;
// 					case EnumMotion.Fall:
// 						break;
// 					case EnumMotion.Land:
// 						break;
// 					case EnumMotion.Damage:
// 						break;
// 					case EnumMotion.Fly:
// 						break;
// 					case EnumMotion.Down:
// 						break;
// 					case EnumMotion.Dead:
// 						break;
// 					case EnumMotion.Action:
// 						break;
// 					default:
// 						Debug.Assert(false);
// 						break;
// 				}
// 			}
// 		}

// 		void UpdateIdle(int i)
// 		{
// 			if (CheckWalk(i))
// 				return;
// 		}
// 		void UpdateWalk(int i)
// 		{
// 			// if (CheckJump(i))
// 			// 	return;
// 			// if (CheckRun(i))
// 			// 	return;
// 			if (CheckIdle(i))
// 				return;
// 		}

// 		void UpdateDash(int i)
// 		{
// 			if (CheckJump(i))
// 				return;
// 			if (CheckSlip(i))
// 				return;
// 		}

// 		void UpdateSlip(int i)
// 		{

// 		}

// 		void UpdateLand(int i)
// 		{

// 		}

// 		void UpdateDamage(int i)
// 		{

// 		}

// 		void UpdateDown(int i)
// 		{

// 		}

// 		void UpdateDead(int i)
// 		{

// 		}

// 		/// <summary>
// 		/// アイドルチェック
// 		/// </summary>
// 		/// <param name="motion"></param>
// 		/// <param name="padInput"></param>
// 		/// <returns></returns>
// 		bool CheckIdle(int i)
// 		{
// 			if (!padInputs[i].IsAnyCrossPress())
// 			{
// 				var charaMotion = charaMotions[i];
// 				charaMotion.SetMotion(EnumMotion.Idle);
// 				charaMotions[i] = charaMotion;
// 				return true;
// 			}

// 			return false;
// 		}

// 		/// <summary>
// 		/// 歩きチェック
// 		/// </summary>
// 		/// <param name="motion"></param>
// 		/// <param name="padInput"></param>
// 		/// <returns></returns>
// 		bool CheckWalk(int i)
// 		{
// 			if (padInputs[i].IsAnyCrossPress())
// 			{
// 				var charaMotion = charaMotions[i];
// 				charaMotion.SetMotion(EnumMotion.Walk);
// 				charaMotions[i] = charaMotion;
// 				return true;
// 			}

// 			return false;
// 		}
// 		/// <summary>
// 		/// ジャンプチェック
// 		/// </summary>
// 		/// <param name="motion"></param>
// 		/// <param name="padInput"></param>
// 		/// <returns></returns>
// 		bool CheckJump(int i)
// 		{
// 			if (padInputs[i].IsJumpPush())
// 			{
// 				var charaMotion = charaMotions[i];
// 				charaMotion.SetMotion(EnumMotion.Jump);
// 				charaMotions[i] = charaMotion;
// 				return true;
// 			}

// 			return false;
// 		}

// 		/// <summary>
// 		/// ダッシュチェック
// 		/// </summary>
// 		/// <param name="motion"></param>
// 		/// <param name="padInput"></param>
// 		/// <returns></returns>
// 		bool CheckDash(int i)
// 		{
// 			if (padInputs[i].crossLeft.IsDouble() || padInputs[i].crossRight.IsDouble())
// 			{
// 				var charaMotion = charaMotions[i];
// 				charaMotion.SetMotion(EnumMotion.Dash);
// 				charaMotions[i] = charaMotion;

// 				// ダッシュ向き
// 				var charaDash = charaDashs[i];
// 				charaDash.dashMuki = (padInputs[i].crossLeft.IsDouble())
// 					? EnumMuki.Left
// 					: EnumMuki.Right;
// 				charaDashs[i] = charaDash;

// 				return true;
// 			}

// 			return false;
// 		}

// 		/// <summary>
// 		/// 滑りチェック
// 		/// </summary>
// 		/// <param name="motion"></param>
// 		/// <param name="padInput"></param>
// 		/// <returns></returns>
// 		bool CheckSlip(int i)
// 		{
// 			if (padInputs[i].IsAnyCrossPress())
// 			{
// 				var charaMotion = charaMotions[i];
// 				charaMotion.SetMotion(EnumMotion.Slip);
// 				charaMotions[i] = charaMotion;
// 				return true;
// 			}

// 			return false;
// 		}

// 	}
// }