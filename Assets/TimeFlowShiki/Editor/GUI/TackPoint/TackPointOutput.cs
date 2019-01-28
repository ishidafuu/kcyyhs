using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NKKD.EDIT
{
    public class TackPointOutput
    {
        public TackPointModel model;

        public void SetModels(TackPointModel model)
        {
            this.model = model;
        }

        public Dictionary<string, object> OutputDict()
        {
            var res = new Dictionary<string, object>
                {
                    //{TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TITLE, this.title_},
                    { TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_START, model.Start },
                    { TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_SPAN, model.Span },
                    { TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TIMELINETYPE, model.TimelineType },
                };

            switch ((TimelineType)model.TimelineType)
            {
                case TimelineType.TL_POS:
                    res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_POSDATA] = JsonUtility.ToJson(model.MotionData.mPos);
                    break;
                case TimelineType.TL_TRANSFORM:
                    res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_TRANSFORMDATA] = JsonUtility.ToJson(model.MotionData.mTransform);
                    break;
                case TimelineType.TL_COLOR:
                    res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_COLORDATA] = JsonUtility.ToJson(model.MotionData.mColor);
                    break;
                case TimelineType.TL_EFFECT:
                    res[TimeFlowShikiSettings.TIMEFLOWSHIKI_DATA_TACK_EFFECTDATA] = JsonUtility.ToJson(model.MotionData.mEffect);
                    break;
                default:
                    Debug.LogError("other timelineType_");
                    break;

            }
            return res;
        }

        public MotionTackPos OutputMotionTackPos()
        {
            return new MotionTackPos(model.Start, model.Span, model.MotionData.mPos);
        }
        public MotionTackTransform OutputMotionTackTransform()
        {
            return new MotionTackTransform(model.Start, model.Span, model.MotionData.mTransform);
        }
        public MotionTackColor OutputMotionTackColor()
        {
            return new MotionTackColor(model.Start, model.Span, model.MotionData.mColor);
        }
        public MotionTackEffect OutputMotionTackEffect()
        {
            return new MotionTackEffect(model.Start, model.Span, model.MotionData.mEffect);
        }
    }
}