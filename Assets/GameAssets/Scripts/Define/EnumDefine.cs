﻿using System;
using System.Collections.Generic;
using UnityEngine;
namespace YYHS
{

	public enum EnumCrossType
	{
		None = 0,
		Up,
		Down,
		Left,
		Right,
	}

	public enum EnumButtonType
	{
		None = 0,
		A,
		B,
		X,
		Y,
	}

	public enum EnumToukiMaterState
	{
		Active = 0,
		Inactive,
		Decide,
	}

	public enum EnumInputCross
	{
		Up,
		Down,
		Left,
		Right,
		_END,
	}

	// public enum EnumInputButton
	// {
	// 	A,
	// 	B,
	// 	Jump,
	// 	_END,
	// }

	public enum EnumPartsTypeBase
	{
		Ant,
		Head,
		Thorax,
		Gaster,
		Arm,
		Leg,
		_END,
	}

	//パーツ位置
	public enum EnumPartsType
	{
		Ant = 0,
		Head,
		Thorax,
		Gaster,
		LeftArm,
		RightArm,
		LeftHand,
		RightHand,
		LeftLeg,
		RightLeg,
		LeftFoot,
		RightFoot,
		_END,
	}

	public enum EnumBehave
	{
		Idle,
		Wander,
		Goto,
		Attack,
		_END,
	}

	public enum EnumMuki
	{
		Left = -1,
		None = 0,
		Right = 1,
	}

	public enum EnumMoveMuki
	{
		None,
		Left,
		LeftLeftUp,
		LeftUp,
		LeftLeftDown,
		LeftDown,
		Up,
		Right,
		RightRightUp,
		RightUp,
		RightRightDown,
		RightDown,
		Down,
	}

	//パーツ位置
	public enum EnumMotion
	{
		Idle = 0,
		Walk,
		Dash,
		Slip,
		Jump,
		Fall,
		Land,
		Damage,
		Fly,
		Down,
		Dead,
		Action,
	}

	//モーションフラグ
	[Flags]
	public enum EnumFlagMotion
	{
		None = 0x0000,
		//空中
		Air = 0x001,
		//ダッシュ
		Dash = 0x002,
		//ダメージ
		Damage = 0x003,
		};
}