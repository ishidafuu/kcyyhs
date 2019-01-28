using System;
using System.Collections.Generic;
using UnityEngine;
namespace NKKD
{
	//InputManagerSetter.csで編集
	public enum EnumButtonType
	{
		Fire1,
		Fire2,
		Fire3,
		Fire4,
		Fire5,
		Fire6,
	}

	public enum EnumCrossType
	{
		Up,
		Down,
		Left,
		Right,
	}

	public enum EnumInputCross
	{
		Up,
		Down,
		Left,
		Right,
		_END,
	}

	public enum EnumInputButton
	{
		A,
		B,
		Jump,
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

}