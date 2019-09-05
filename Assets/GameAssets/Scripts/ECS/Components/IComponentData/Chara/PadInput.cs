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
        Vector2 m_axis;
        // 十字
        public Button m_crossUp;
        public Button m_crossDown;
        public Button m_crossLeft;
        public Button m_crossRight;
        // ボタン
        public Button m_buttonA;
        public Button m_buttonB;
        public Button m_buttonX;
        public Button m_buttonY;
        public void SetCross(Vector2 axis, float time)
        {
            // 直前キーから変更無ければ処理しない
            if (axis != m_axis)
            {
                bool isUp = (axis.y > +0.1f);
                bool isDown = (axis.y < -0.1f);
                bool isRight = (axis.x > +0.1f);
                bool isLeft = (axis.x < -0.1f);

                m_axis = axis;
                m_crossUp.SetCrossData(isUp, Time.time);
                m_crossDown.SetCrossData(isDown, Time.time);
                m_crossRight.SetCrossData(isRight, Time.time);
                m_crossLeft.SetCrossData(isLeft, Time.time);
            }
        }

        public EnumCrossType GetPressCross()
        {
            if (m_crossUp.m_isPress)
                return EnumCrossType.Up;

            if (m_crossDown.m_isPress)
                return EnumCrossType.Down;

            if (m_crossLeft.m_isPress)
                return EnumCrossType.Left;

            if (m_crossRight.m_isPress)
                return EnumCrossType.Right;

            return EnumCrossType.None;
        }

        public EnumButtonType GetPressButton()
        {
            if (m_buttonA.m_isPress)
                return EnumButtonType.A;

            if (m_buttonB.m_isPress)
                return EnumButtonType.B;

            if (m_buttonX.m_isPress)
                return EnumButtonType.X;

            if (m_buttonY.m_isPress)
                return EnumButtonType.Y;

            return EnumButtonType.None;
        }

        public bool IsAnyCrossPress()
        {
            return (m_crossUp.m_isPress || m_crossDown.m_isPress || m_crossLeft.m_isPress || m_crossRight.m_isPress);
        }

        public bool IsJumpPush()
        {
            return ((m_buttonA.m_isPress && m_buttonB.m_isPush)
                || (m_buttonA.m_isPush && m_buttonB.m_isPress));
        }

    }

    public struct Button
    {
        // 連打受付時間
        const float Double_Time = 0.4f;
        // 押した瞬間
        public boolean m_isPush;
        // 押してる
        public boolean m_isPress;
        // 離した瞬間
        public boolean m_isPop;
        // 連打
        public boolean m_isDouble;
        // ダッシュ用直前押した瞬間時間
        float m_lastPushTime;

        public void SetButtonData(boolean isPush, boolean isPress, boolean isPop, float time)
        {
            m_isPush = isPush;
            m_isPress = isPress;
            m_isPop = isPop;
            m_isDouble = m_isPush && ((time - m_lastPushTime) < Double_Time);
            if (m_isPush)
                m_lastPushTime = time;

        }

        public void SetCrossData(boolean isPress, float time)
        {
            m_isPush = !m_isPress && isPress;
            m_isPress = isPress;
            m_isPop = m_isPress && !isPress;
            m_isDouble = (m_isPush && ((time - m_lastPushTime) < Double_Time));
            if (m_isPush)
                m_lastPushTime = time;

        }
    }
}
