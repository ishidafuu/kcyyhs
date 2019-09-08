﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YYHS
{
    public static class YHRawAnimationConverter
    {
        const float SlopeToTangent = 60f;
        public static YHAnimation Convert(YHRawAnimation rawAnim)
        {
            YHAnimation result = new YHAnimation()
            {
                m_name = rawAnim.AnimationClip.m_Name,
                m_length = TimeToFrame(rawAnim.AnimationClip.m_AnimationClipSettings.m_StopTime),
            };

            ConvertPosition(rawAnim, result);
            ConvertScale(rawAnim, result);
            ConvertRotate(rawAnim, result);
            ConvertFloat(rawAnim, result);
            ConvertEvent(rawAnim, result);
            // DistinctFrames(result);

            return result;
        }

        private static void ConvertPosition(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_PositionCurves)
            {
                string partsName = item.path.Remove(0, item.path.LastIndexOf("/") + 1);
                YHAnimationParts parts = GetOrCreateParts(result, partsName);

                List<Keyframe> keyFramesX = new List<Keyframe>();
                List<Keyframe> keyFramesY = new List<Keyframe>();
                foreach (var curve in item.curve.m_Curve)
                {
                    int frame = TimeToFrame(curve.time);

                    Keyframe keyFrameX = new Keyframe(frame, curve.value.x,
                        curve.inSlope.x / SlopeToTangent, curve.outSlope.x / SlopeToTangent,
                        curve.inWeight.x, curve.outWeight.x);

                    Keyframe keyFrameY = new Keyframe(frame, curve.value.y,
                        curve.inSlope.y / SlopeToTangent, curve.outSlope.y / SlopeToTangent,
                        curve.inWeight.y, curve.outWeight.y);

                    keyFramesX.Add(keyFrameX);
                    keyFramesY.Add(keyFrameY);
                }

                parts.m_positionX = new AnimationCurve(keyFramesX.ToArray());
                parts.m_positionY = new AnimationCurve(keyFramesY.ToArray());
            }
        }

        private static void ConvertScale(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_ScaleCurves)
            {
                string partsName = item.path.Remove(0, item.path.LastIndexOf("/") + 1);
                YHAnimationParts parts = GetOrCreateParts(result, partsName);

                List<Keyframe> keyFramesX = new List<Keyframe>();
                List<Keyframe> keyFramesY = new List<Keyframe>();
                foreach (var curve in item.curve.m_Curve)
                {
                    int frame = TimeToFrame(curve.time);

                    Keyframe keyFrameX = new Keyframe(frame, curve.value.x,
                        curve.inSlope.x / SlopeToTangent, curve.outSlope.x / SlopeToTangent,
                        curve.inWeight.x, curve.outWeight.x);

                    Keyframe keyFrameY = new Keyframe(frame, curve.value.y,
                        curve.inSlope.y / SlopeToTangent, curve.outSlope.y / SlopeToTangent,
                        curve.inWeight.y, curve.outWeight.y);

                    keyFramesX.Add(keyFrameX);
                    keyFramesY.Add(keyFrameY);
                }

                parts.m_scaleX = new AnimationCurve(keyFramesX.ToArray());
                parts.m_scaleY = new AnimationCurve(keyFramesY.ToArray());
            }
        }

        private static void ConvertRotate(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_EulerCurves)
            {
                string partsName = item.path.Remove(0, item.path.LastIndexOf("/") + 1);
                YHAnimationParts parts = GetOrCreateParts(result, partsName);
                List<Keyframe> keyFrames = new List<Keyframe>();
                foreach (var curve in item.curve.m_Curve)
                {
                    int frame = TimeToFrame(curve.time);

                    Keyframe keyFrame = new Keyframe(frame, curve.value.z,
                        curve.inSlope.z / SlopeToTangent, curve.outSlope.z / SlopeToTangent,
                        curve.inWeight.z, curve.outWeight.z);

                    keyFrames.Add(keyFrame);
                }

                parts.m_rotation = new AnimationCurve(keyFrames.ToArray());
            }
        }

        private static void ConvertFloat(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_FloatCurves)
            {
                string partsName = item.path.Remove(0, item.path.LastIndexOf("/") + 1);
                YHAnimationParts parts = GetOrCreateParts(result, partsName);
                string attr = item.attribute;
                foreach (var curve in item.curve.m_Curve)
                {
                    YHFrameData data = null;
                    switch (attr)
                    {
                        case "m_IsActive":
                            data = GetOrCreateFrameData(parts.m_isActive, TimeToFrame(curve.time));
                            break;
                        case "m_FlipX":
                            data = GetOrCreateFrameData(parts.m_isFlipX, TimeToFrame(curve.time));
                            break;
                        case "m_FlipY":
                            data = GetOrCreateFrameData(parts.m_isFlipY, TimeToFrame(curve.time));
                            break;
                        case "m_IsBrink":
                            data = GetOrCreateFrameData(parts.m_isBrink, TimeToFrame(curve.time));
                            break;
                        default:
                            Debug.LogError($"Unknown attribute : {attr}");
                            break;
                    }
                    data.m_value = (curve.value != 0);
                }

                // parts.frames = parts.frames.OrderBy(x => x.frame).ToList();
            }
        }

        private static void ConvertEvent(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_Events)
            {
                EnumEventFunctionName functionName = EnumEventFunctionName.None;
                foreach (EnumEventFunctionName evFuncName in Enum.GetValues(typeof(EnumEventFunctionName)))
                {
                    if (evFuncName.ToString() == item.functionName)
                    {
                        functionName = evFuncName;
                        break;
                    }
                }

                if (functionName == EnumEventFunctionName.None)
                {
                    Debug.LogError($"Not Found FunctionName:{item.functionName}");
                }

                YHFrameEvent newEvent = new YHFrameEvent()
                {
                    m_frame = TimeToFrame(item.time),
                    m_functionName = functionName,
                    m_stringParameter = item.data,
                    m_floatParameter = item.floatParameter,
                    m_intParameter = item.intParameter,
                };

                result.m_events.Add(newEvent);
            }
        }



        // private static void DistinctFrames(YHAnimation result)
        // {
        //     foreach (var item in result.parts)
        //     {
        //         List<YHFrameData> distinctFrames = new List<YHFrameData>();
        //         for (int i = 0; i < item.frames.Count; i++)
        //         {
        //             YHFrameData nowFrame = item.frames[i];
        //             if (i == 0)
        //             {
        //                 distinctFrames.Add(nowFrame);
        //                 continue;
        //             }
        //             YHFrameData lastFrame = item.frames[i - 1];

        //             if (nowFrame.isActive != lastFrame.isActive
        //                 || nowFrame.isFlipX != lastFrame.isFlipX
        //                 || nowFrame.isFlipY != lastFrame.isFlipY)
        //             {
        //                 distinctFrames.Add(nowFrame);
        //             }
        //         }

        //         item.frames = distinctFrames;
        //     }
        // }

        private static YHAnimationParts GetOrCreateParts(YHAnimation result, string partsName)
        {
            YHAnimationParts parts = result.m_parts.FirstOrDefault(x => x.m_name == partsName);
            if (parts == null)
            {
                parts = new YHAnimationParts()
                {
                    m_name = partsName,
                    m_orderInLayer = GetLayer(partsName)
                };

                result.m_parts.Add(parts);
            }
            return parts;
        }

        private static YHFrameData GetOrCreateFrameData(List<YHFrameData> frameData, int frame)
        {
            var data = new YHFrameData()
            {
                m_frame = frame,
            };

            YHFrameData lastFrameData = frameData.Where(x => x.m_frame < frame).LastOrDefault();

            if (lastFrameData != null)
            {
                data.m_value = lastFrameData.m_value;
            }

            frameData.Add(data);

            return data;
        }

        private static int TimeToFrame(float time) => (int)Mathf.Round(time * 60f);


        private static float GetLayer(string partsName)
        {
            if (partsName.IndexOf("uuu_") >= 0)
                return -0.5f;

            if (partsName.IndexOf("uu_") >= 0)
                return -0.4f;

            if (partsName.IndexOf("u_") >= 0)
                return -0.3f;

            if (partsName.IndexOf("l_") >= 0)
                return +0.3f;

            if (partsName.IndexOf("ll_") >= 0)
                return +0.4f;

            if (partsName.IndexOf("lll_") >= 0)
                return +0.5f;

            return 0;
        }
    }
}
