using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YYHS
{
    public static class YHRawAnimationConverter
    {
        public static YHAnimation Convert(YHRawAnimation rawAnim)
        {
            YHAnimation result = new YHAnimation()
            {
                name = rawAnim.AnimationClip.m_Name,
                length = TimeToFrame(rawAnim.AnimationClip.m_AnimationClipSettings.m_StopTime),
            };

            ConvertPosition(rawAnim, result);
            ConvertScale(rawAnim, result);
            ConvertRotate(rawAnim, result);
            ConvertFloat(rawAnim, result);
            ConvertEvent(rawAnim, result);
            DistinctFrames(result);

            return result;
        }

        private static void ConvertPosition(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_PositionCurves)
            {
                string partsName = item.path.Remove(0, item.path.LastIndexOf("/") + 1);
                YHAnimationParts parts = GetOrCreateParts(result, partsName);
                foreach (var curve in item.curve.m_Curve)
                {
                    Vector2Int position = Vector2Int.FloorToInt(curve.value);
                    YHFramePosition last = parts.positions.LastOrDefault();

                    if (last != null && last.position == position)
                        continue;

                    YHFramePosition framePosition = new YHFramePosition()
                    {
                        frame = TimeToFrame(curve.time),
                        position = position,
                        inTangent = curve.inSlope,
                        outTangent = curve.outSlope,
                        inWeight = curve.inWeight,
                        outWeight = curve.outWeight,
                    };

                    parts.positions.Add(framePosition);
                }
            }
        }

        private static void ConvertScale(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_ScaleCurves)
            {
                string partsName = item.path.Remove(0, item.path.LastIndexOf("/") + 1);
                YHAnimationParts parts = GetOrCreateParts(result, partsName);
                foreach (var curve in item.curve.m_Curve)
                {
                    Vector2Int scale = Vector2Int.FloorToInt(curve.value);
                    YHFrameScale last = parts.scales.LastOrDefault();

                    if (last != null && last.scale == scale)
                        continue;

                    YHFrameScale frameScale = new YHFrameScale()
                    {
                        frame = TimeToFrame(curve.time),
                        scale = scale,
                        inTangent = curve.inSlope,
                        outTangent = curve.outSlope,
                        inWeight = curve.inWeight,
                        outWeight = curve.outWeight,
                    };

                    parts.scales.Add(frameScale);
                }
            }
        }

        private static void ConvertRotate(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_EulerCurves)
            {
                string partsName = item.path.Remove(0, item.path.LastIndexOf("/") + 1);
                YHAnimationParts parts = GetOrCreateParts(result, partsName);
                foreach (var curve in item.curve.m_Curve)
                {
                    float rotation = curve.value.z;
                    YHFrameRotation last = parts.rotations.LastOrDefault();

                    if (last != null && last.rotation == rotation)
                        continue;

                    YHFrameRotation frameRotation = new YHFrameRotation()
                    {
                        frame = TimeToFrame(curve.time),
                        rotation = rotation,
                        inTangent = curve.inSlope.z,
                        outTangent = curve.outSlope.z,
                        inWeight = curve.inWeight.z,
                        outWeight = curve.outWeight.z,
                    };

                    parts.rotations.Add(frameRotation);
                }
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
                    YHFrameData last = parts.frames.LastOrDefault();

                    YHFrameData frameData = GetOrCreateFrameData(parts, TimeToFrame(curve.time));
                    switch (attr)
                    {
                        case "m_IsActive":
                            frameData.isActive = (curve.value != 0);
                            break;
                        case "m_FlipX":
                            frameData.isFlipX = (curve.value != 0);
                            break;
                        case "m_FlipY":
                            frameData.isFlipY = (curve.value != 0);
                            break;
                        case "m_IsBrink":
                            frameData.isBrink = (curve.value != 0);
                            break;
                        default:
                            Debug.LogError($"Unknown attribute : {attr}");
                            break;
                    }
                }

                parts.frames = parts.frames.OrderBy(x => x.frame).ToList();
            }
        }

        private static void ConvertEvent(YHRawAnimation rawAnim, YHAnimation result)
        {
            foreach (var item in rawAnim.AnimationClip.m_Events)
            {
                YHFrameEvent newEvent = new YHFrameEvent()
                {
                    frame = TimeToFrame(item.time),
                    functionName = item.functionName,
                    data = item.data,
                    floatParameter = item.floatParameter,
                    intParameter = item.intParameter,
                };

                result.events.Add(newEvent);
            }
        }

        private static void DistinctFrames(YHAnimation result)
        {
            foreach (var item in result.parts)
            {
                List<YHFrameData> distinctFrames = new List<YHFrameData>();
                for (int i = 0; i < item.frames.Count; i++)
                {
                    YHFrameData nowFrame = item.frames[i];
                    if (i == 0)
                    {
                        distinctFrames.Add(nowFrame);
                        continue;
                    }
                    YHFrameData lastFrame = item.frames[i - 1];

                    if (nowFrame.isActive != lastFrame.isActive
                        || nowFrame.isFlipX != lastFrame.isFlipX
                        || nowFrame.isFlipY != lastFrame.isFlipY)
                    {
                        distinctFrames.Add(nowFrame);
                    }
                }

                item.frames = distinctFrames;
            }
        }

        private static YHAnimationParts GetOrCreateParts(YHAnimation result, string partsName)
        {
            YHAnimationParts parts = result.parts.FirstOrDefault(x => x.name == partsName);
            if (parts == null)
            {
                parts = new YHAnimationParts()
                {
                    name = partsName
                };
                result.parts.Add(parts);
            }
            return parts;
        }

        private static YHFrameData GetOrCreateFrameData(YHAnimationParts parts, int frame)
        {
            YHFrameData frameData = parts.frames.FirstOrDefault(x => x.frame == frame);
            if (frameData == null)
            {
                frameData = new YHFrameData()
                {
                    frame = frame
                };
                parts.frames.Add(frameData);
            }
            return frameData;
        }

        private static int TimeToFrame(float time) => (int)Mathf.Round(time * 60f);

    }
}
