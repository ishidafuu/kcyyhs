using Unity.Entities;
using UnityEngine;
namespace YYHS
{
    /// <summary>
    /// キー入力
    /// </summary>
    public struct PadScan : IComponentData
    {
        // 十字のバッファ
        Vector2 axis;
        // 十字
        public Button crossUp;
        public Button crossDown;
        public Button crossLeft;
        public Button crossRight;
        // ボタン
        public Button buttonA;
        public Button buttonB;
        public Button buttonX;
        public Button buttonY;
        public void SetCross(Vector2 _axis, float _time)
        {
            // 直前キーから変更無ければ処理しない
            if (_axis != axis)
            {
                var isUp = (_axis.y > +0.1f);
                var isDown = (_axis.y < -0.1f);
                var isRight = (_axis.x > +0.1f);
                var isLeft = (_axis.x < -0.1f);

                axis = _axis;
                crossUp.SetCrossData(isUp, Time.time);
                crossDown.SetCrossData(isDown, Time.time);
                crossRight.SetCrossData(isRight, Time.time);
                crossLeft.SetCrossData(isLeft, Time.time);
                // Debug.Log(axis);
            }
        }

        public EnumCrossType GetPressCross()
        {
            if (crossUp.IsPress())
                return EnumCrossType.Up;

            if (crossDown.IsPress())
                return EnumCrossType.Down;

            if (crossLeft.IsPress())
                return EnumCrossType.Left;

            if (crossRight.IsPress())
                return EnumCrossType.Right;

            return EnumCrossType.None;
        }

        public EnumButtonType GetPressButton()
        {
            if (buttonA.IsPress())
                return EnumButtonType.A;

            if (buttonB.IsPress())
                return EnumButtonType.B;

            if (buttonX.IsPress())
                return EnumButtonType.X;

            if (buttonY.IsPress())
                return EnumButtonType.Y;

            return EnumButtonType.None;
        }

        /// <summary>
        /// どれか十字が押されてる
        /// </summary>
        /// <returns></returns>
        public bool IsAnyCrossPress()
        {
            return (crossUp.IsPress() || crossDown.IsPress() || crossLeft.IsPress() || crossRight.IsPress());
        }
        /// <summary>
        /// ジャンプ入力
        /// </summary>
        /// <returns></returns>
        public bool IsJumpPush()
        {
            return ((buttonA.IsPress() && buttonB.IsPush())
                || (buttonA.IsPush() && buttonB.IsPress()));
        }

    }

    public struct Button
    {
        // 連打受付時間
        const float DOUBLE_TIME = 0.4f;
        // 押した瞬間
        byte isPush;
        // 押してる
        byte isPress;
        // 離した瞬間
        byte isPop;
        // 連打
        byte isDouble;
        // ダッシュ用直前押した瞬間時間
        float lastPushTime;

        public bool IsPush()
        {
            return isPush != 0;
        }

        public bool IsPress()
        {
            return isPress != 0;
        }

        public bool IsPop()
        {
            return isPop != 0;
        }

        public bool IsDouble()
        {
            return isDouble != 0;
        }

        public void SetButtonData(bool _isPush, bool _isPress, bool _isPop, float _time)
        {
            isPush = BoolToByte(_isPush);
            isPress = BoolToByte(_isPress);
            isPop = BoolToByte(_isPop);
            isDouble = BoolToByte(IsPush() && ((_time - lastPushTime) < DOUBLE_TIME));
            if (IsPush())
                lastPushTime = _time;

        }

        public void SetCrossData(bool _isPress, float _time)
        {
            isPush = BoolToByte((isPress == 0) && _isPress);
            isPress = BoolToByte(_isPress);
            isPop = BoolToByte((isPress != 0) && !_isPress);
            isDouble = BoolToByte(IsPush() && ((_time - lastPushTime) < DOUBLE_TIME));
            if (IsPush())
                lastPushTime = _time;

            // if (IsPress())
            // 	Debug.Log("isPress");
        }
        byte BoolToByte(bool value)
        {
            return (value) ? (byte)1 : (byte)0;
        }

        // bool ByteToBool(byte value)
        // {
        // 	return (value != 0);
        // }
    }
}
