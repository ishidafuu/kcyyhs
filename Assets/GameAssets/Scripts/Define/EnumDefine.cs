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
        UnderFrame,
        OverChara,
        Chara,
        OverBackGround,
        BackGround,
    }

    public enum EnumBGPartsType
    {
        bg_00,
        bg_01,
        bg_02,
        bg_03,
        bg_04,
        bg_05,
        bg_06,
        bg_07,
        bg_08,
    }

    public enum EnumCommonPartsType
    {
        frame_bottom,
        frame_line,
        frame_top,
    }

    public enum EnumAnimType
    {
        Action,
        Defence,
        DefenceReaction,
    }

    public enum EnumBattleSequenceState
    {
        Idle,
        Start,
        Play,
    }

    public enum EnumAnimationStep
    {
        Sleep,
        WaitPageA,
        WaitPageB,
        Finished,
        Skip,
    }

    public enum EnumDamageLv
    {
        Air,
        NoDamage,
        Tip,
        Hit,
    }
    public static partial class EnumDamageLvExtend
    {
        public static bool IsHit(this EnumDamageLv value)
        {
            bool res = false;
            switch (value)
            {
                case EnumDamageLv.Hit:
                    res = true;
                    break;
            }
            return res;
        }
    }


    public enum EnumDamageReaction
    {
        None,
        Shaky,
        Fly,
    }


    public enum EnumActionType
    {
        None,
        Jump,
        Guard,
        Reverse,
        Technique,
        ShortAttack,
        AirShortAttack,
        MiddleAttack,
        LongAttack,
        WaveAttack,
        GroundAttack,
    }
    public static partial class EnumActionTypeExtend
    {
        public static bool IsChaseable(this EnumActionType value, EnumActionType targetValue)
        {
            bool res = false;
            switch (value)
            {
                case EnumActionType.ShortAttack:
                    res = value > targetValue;
                    break;
                case EnumActionType.AirShortAttack:
                    res = (targetValue == EnumActionType.ShortAttack)
                        || (targetValue == EnumActionType.GroundAttack);
                    break;
                case EnumActionType.MiddleAttack:
                    res = value >= targetValue;
                    break;
                case EnumActionType.LongAttack:
                case EnumActionType.WaveAttack:
                    res = true;
                    break;
                case EnumActionType.GroundAttack:
                    res = (targetValue != EnumActionType.AirShortAttack);
                    break;
            }

            return res;
        }

        public static bool IsOvertakeable(this EnumActionType value, EnumActionType targetValue)
        {
            bool res = false;
            switch (value)
            {
                case EnumActionType.ShortAttack:
                    res = value > targetValue;
                    break;
                case EnumActionType.AirShortAttack:
                    res = (targetValue == EnumActionType.ShortAttack) || (targetValue == EnumActionType.GroundAttack);
                    break;
                case EnumActionType.MiddleAttack:
                    res = (value == EnumActionType.LongAttack)
                        || (value == EnumActionType.WaveAttack);
                    break;
                case EnumActionType.GroundAttack:
                    res = (targetValue != EnumActionType.AirShortAttack);
                    break;
                default:
                    res = true;
                    break;
            }

            return res;
        }

        public static bool IsAttack(this EnumActionType value)
        {
            bool res = false;
            switch (value)
            {
                case EnumActionType.ShortAttack:
                case EnumActionType.AirShortAttack:
                case EnumActionType.MiddleAttack:
                case EnumActionType.LongAttack:
                case EnumActionType.WaveAttack:
                case EnumActionType.GroundAttack:
                    res = true;
                    break;
            }
            return res;
        }

        public static bool IsAirHit(this EnumActionType value)
        {
            bool res = false;
            switch (value)
            {
                case EnumActionType.MiddleAttack:
                case EnumActionType.LongAttack:
                case EnumActionType.WaveAttack:
                    res = true;
                    break;
            }
            return res;
        }

        public static bool IsShort(this EnumActionType value)
        {
            bool res = false;
            switch (value)
            {
                case EnumActionType.ShortAttack:
                case EnumActionType.AirShortAttack:
                    res = true;
                    break;
            }
            return res;
        }
    }


    public enum EnumDefenceType
    {
        Stand,
        Fly,
        Jumping,
        Step,
    }


    public enum EnumAnimationName
    {
        _Air00,
        _Air01,
        _Air02,
        _DefenceA00,
        _DefenceA01,
        _DefenceA02,
        _DefenceA03,
        _DefenceB00,
        _DefenceB01,
        _DefenceB02,
        _DefenceB03,
        _DefenceC00,
        _DefenceC01,
        _DefenceC02,
        _DefenceC03,
        _Down00,
        _Down01,
        _Fly,
        _Jump00,
        _Jump01,
        _JumpAction,
        _Miss,
        _Reverse,
        _Shaky,
        _Stand00,
        _Stand01,
        _Stand02,
        _Wait,

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


    }

    public enum EnumEventFunctionName
    {
        None,
        EventPlaySE,
        EventEffectBG,
        EventEffectScreen,
        EventEffectScreenArg,
        EventEffectSideOff,
        EventEffectLarge,
        EventEffectMedium,
        EventEffectSmall,
        EventEffectDamageBody,
        EventEffectDamageFace,
        EventEffectReset,
        EventFillterScreen,
        EventFillterScreenArg,
        EventJump,
        EventBGFillterArg,
        EventFlash,
        EventBlink,
        EventColor,
    }


    public enum EnumEffectType
    {
        EffectScreen,
        EffectLarge,
        EffectMedium,
        EffectSmall,
        EffectDamageBody,
        EffectDamageFace,
        FillterScreen,
        EffectBG,
    }

    public enum EnumFillter
    {
        EndBattleSequence,
        SwitchSplitView,
    }

    public enum EnumFrameParts
    {
        ToukiGauge,
        LifeGauge,
        BalanceGauge,
        ReiGauge,
        Signal,
        ReiPiece,
        Jump,
        Air,
        Fall,
        ReiPieceDistribute,
    }

    public enum EnumSignal
    {
        Sleep,
        Decide,
        Skip,
    }

    public enum EnumReiState
    {
        Born,
        Idle,
        Wait,
        Distribute,

    }

    public enum EnumEffectLarge
    {
        Damage,
    }

    public enum EnumEffectPosition
    {
        None,
        Dodge,
        Face,
        Shot,
    }

    public enum EnumJumpState
    {
        None = 0,
        Jumping,
        Air,
        Falling,
    }

    public static partial class EnumJumpStateExtend
    {
        public static bool IsAir(this EnumJumpState value)
        {
            bool res = false;
            switch (value)
            {
                case EnumJumpState.Air:
                case EnumJumpState.Jumping:
                    res = true;
                    break;
            }
            return res;
        }
    }


    public enum EnumJumpEffectStep
    {
        JumpStart = 0,
        InJumping,
        OutJumping,
        Air,
        FallStart,
        InFalling,
        OutFalling,
    }

    public enum EnumShaderType
    {
        EffectBG,
        EffectScreen,
        EffectLarge,
        EffectMedium,
        EffectSmall,
        FilterScreen,
        FrameParts,
    }

    public enum EnumShaderParam
    {
        _Value,
        _Frame,
        _Alpha,
    }

    public enum EnumAttackRangeType
    {
        None,
        Short,
        Middle,
        Long,
        Wave,
        Ground,
    }

    public enum EnumAttackEffectType
    {
        None,
        Heal,
        LockTech,
        ToukiSpeedUp,
    }

    public enum EnumDownState
    {
        None,
        Down,
        Reverse,
    }
}
