using System;
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

    public enum EnumDrawLayer
    {
        OverFrame,
        Frame,
        OverChara,
        Chara,
        OverBackGround,
        BackGround,
    }

    public enum EnumToukiMaterState
    {
        Active = 0,
        Inactive,
        Decide,
    }



    public enum EnumBGPartsType
    {
        bg00,
        bg01,
        bg02,
        bg03,
        bg04,
        bg05,
        bg06,
        bg07,
        bg08,
        frame_bottom,
        frame_top,
        meter,
        meter00,
        meter01,
        meter02,
        meter03,
        meter04,

    }

    public enum EnumAnimType
    {
        Action,
        Defence,
        DefenceReaction,
    }

    public enum EnumAnimationStep
    {
        Sleep,
        Ready,
        Start,
        Fire,
        Finished,
    }

    public enum EnumDamageLv
    {
        NoDamage,
        Tip,
        Hit,
        CleanHit,
    }

    public enum EnumDamageReaction
    {
        None,
        Shaky,
        Down,
    }


    public enum EnumActionType
    {
        None,
        Guard,
        Technique,
        ShortAttack,
        MiddleAttack,
        LongAttack,
        GroundAttack,
        WaveAttack,
    }

    public enum EnumDefenceType
    {
        Stand,
        Fly,
        Step,
        Jumping,
    }


    public enum EnumAnimationName
    {
        _Air,
        _Charge00,
        _Charge01,
        _Fall00,
        _Fall01,
        _Jump00,
        _Jump01,
        _Stand,
        Action00_00,
        Action00_01,
        Action01_00,
        Action01_01,
        Action02_00,
        Action02_01,
        Action03_00,
        Action03_01,
        Action04_00,
        Action04_01,
        Action05_00,
        Action05_01,
        Action06_00,
        Action06_01,
        Action07_00,
        Action07_01,
        DefenceA00,
        DefenceA01,
        DefenceA02,
        DefenceB00,
        DefenceB01,
        DefenceB02,
        Down,
        Shaky,
        Miss,

    }

}
