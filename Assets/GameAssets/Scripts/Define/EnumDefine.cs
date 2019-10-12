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
        bg00,
        bg01,
        bg02,
        bg03,
        bg04,
        bg05,
        bg06,
        bg07,
        bg08,

    }

    public enum EnumCommonPartsType
    {
        // cm_a_effect00_00,
        // cm_a_effect01_00,
        // cm_a_effect02_00,
        // cm_a_effect03_00,
        // cm_a_effect03_01,
        // cm_d_effect00_00,
        // cm_d_effect00_01,
        // cm_d_effect00_02,
        // cm_d_effect01_00,
        // cm_d_effect01_01,
        // cm_d_effect02_00,
        // cm_d_effect02_01,
        // cm_d_effect02_02,
        // cm_e_effect01_00,
        // cm_e_effect02_00,
        // cm_e_effect03_00,
        // cm_e_effect04_00,
        // cm_e_effect06_00,
        // cm_e_effect06_01,
        // cm_e_effect06_02,
        // cm_e_effect06_03,
        // cm_e_effect06_04,
        // cm_e_effect06_05,
        // cm_f_effect00_00,
        // cm_f_effect00_01,
        // cm_f_effect07_00,
        // cm_f_effect07_01,
        // cm_f_effect07_02,
        // cm_f_effect07_03,
        // cm_f_effect07_04,
        energy,
        fire00,
        fire01,
        fire02,
        frame_bottom,
        frame_line,
        frame_top,
        rei,
        signal00,
        signal01,
        signal02,
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
        _Air00,
        _Air01,
        _Air02,
        _DefenceA00,
        _DefenceA01,
        _DefenceA02,
        _DefenceB00,
        _DefenceB01,
        _DefenceB02,
        _Down,
        _Jump00,
        _Jump01,
        _Miss,
        _Shaky,
        _Stand00,
        _Stand01,
        _Stand02,

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
        EventEffectScreen,
        EventEffectScreenArg,
        EventEffectLarge,
        EventEffectMedium,
        EventEffectSmall,
        EventEffectDamageBody,
        EventEffectDamageFace,
        EventFillterScreen,
        EventFillterScreenArg,
        EventFillterBG,
        EventBGFillterArg,
        EventFlash,
        EventBlink,
        EventColor,
    }


    public enum EnumShaderBaseTexture
    {
        Screen,
        BigQuad,
        SmallQuad,
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
        FillterBG,
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
    }

    public enum EnumSignal
    {
        Sleep,
        Decide,
        Skip,
    }

    public enum EnumReiState
    {
        Idle,
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
}
