﻿using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class ToukiMeterCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<ToukiMeter>(),
                ComponentType.ReadOnly<JumpState>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Settings.Instance.Debug.IsSkip())
                return inputDeps;

            BattleSequencer seq = GetSingleton<BattleSequencer>();
            m_query.AddDependency(inputDeps);

            NativeArray<ToukiMeter> toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            NativeArray<JumpState> jumpStates = m_query.ToComponentDataArray<JumpState>(Allocator.TempJob);

            Vector2[] uv = Shared.m_bgFrameMeshMat.m_meshDict[EnumBGPartsType.bg_00.ToString()].uv;
            var job = new CountToukiJob()
            {
                m_toukiMeters = toukiMeters,
                m_jumpStates = jumpStates,
                m_seq = seq,
                BgScrollRange = Settings.Instance.DrawPos.BgWidth << (Settings.Instance.DrawPos.BgScrollRangeFactor - 1),
                ToukiMax = Settings.Instance.Common.ToukiMax,
                ToukiAnimationInterval = Settings.Instance.Animation.ToukiAnimationInterval,
                DecideScrollSpeed = Settings.Instance.Animation.DecideScrollSpeed,
                HighScrollSpeed = Settings.Instance.Animation.HighScrollSpeed,
                LowScrollSpeed = Settings.Instance.Animation.LowScrollSpeed,
                SpriteUl = uv[0].x,
                SpriteUr = uv[1].x,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(toukiMeters);
            toukiMeters.Dispose();
            jumpStates.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountToukiJob : IJob
        {
            public NativeArray<ToukiMeter> m_toukiMeters;
            [ReadOnly] public NativeArray<JumpState> m_jumpStates;
            [ReadOnly] public BattleSequencer m_seq;
            [ReadOnly] public int BgScrollRange;
            [ReadOnly] public int ToukiMax;
            [ReadOnly] public float SpriteUl;
            [ReadOnly] public float SpriteUr;
            [ReadOnly] public int ToukiAnimationInterval;
            [ReadOnly] public int HighScrollSpeed;
            [ReadOnly] public int LowScrollSpeed;
            [ReadOnly] public int DecideScrollSpeed;
            float m_bgScrollWidth;

            public void Execute()
            {
                m_bgScrollWidth = (SpriteUr - SpriteUl) / 2;

                for (int i = 0; i < m_toukiMeters.Length; i++)
                {
                    var toukiMeter = m_toukiMeters[i];
                    var JumpState = m_jumpStates[i];

                    if (JumpState.m_state == EnumJumpState.None
                     || JumpState.m_state == EnumJumpState.Air)
                    {
                        bool isSideA = SideUtil.IsSideA(i);
                        bool isDecided = (isSideA)
                            ? (m_seq.m_sideA.m_animStep != EnumAnimationStep.Sleep)
                            : (m_seq.m_sideB.m_animStep != EnumAnimationStep.Sleep);

                        if (isDecided)
                        {
                            UpdateDecidedScroll(ref toukiMeter, isSideA);
                        }
                        else
                        {
                            if (toukiMeter.m_isDecided)
                            {
                                ResetValue(ref toukiMeter);
                            }

                            IncMeterValue(ref toukiMeter);
                            IncBGScrollValue(ref toukiMeter, isSideA);
                            IncAnimationCount(ref toukiMeter);
                        }
                    }

                    m_toukiMeters[i] = toukiMeter;
                }
            }

            private static void ResetValue(ref ToukiMeter toukiMeter)
            {
                toukiMeter.m_value = 0;
                toukiMeter.m_isDecided = false;
            }

            private int IncBGScrollCount(int bgScroll, int value, bool isSideA)
            {
                if (isSideA)
                {
                    bgScroll += value;
                }
                else
                {
                    bgScroll -= value;
                }

                bgScroll = (bgScroll + BgScrollRange) % BgScrollRange;

                return bgScroll;
            }

            private void UpdateDecidedScroll(ref ToukiMeter toukiMeter, bool isSideA)
            {
                toukiMeter.m_bgScroll = IncBGScrollCount(toukiMeter.m_bgScroll, DecideScrollSpeed, isSideA);

                UpdateScrollUV(ref toukiMeter);
            }

            private void IncMeterValue(ref ToukiMeter toukiMeter)
            {
                if (toukiMeter.m_cross != EnumCrossType.None)
                {
                    toukiMeter.m_value++;
                    if (toukiMeter.m_value > ToukiMax)
                    {
                        toukiMeter.m_value = ToukiMax;
                    }
                }
            }

            private void IncBGScrollValue(ref ToukiMeter toukiMeter, bool isSideA)
            {
                switch (toukiMeter.m_cross)
                {
                    case EnumCrossType.Left:
                    case EnumCrossType.Down:
                        toukiMeter.m_bgScroll = IncBGScrollCount(toukiMeter.m_bgScroll, -LowScrollSpeed, isSideA);
                        break;
                    case EnumCrossType.Right:
                        toukiMeter.m_bgScroll = IncBGScrollCount(toukiMeter.m_bgScroll, HighScrollSpeed, isSideA);
                        break;
                }

                UpdateScrollUV(ref toukiMeter);
            }

            private void UpdateScrollUV(ref ToukiMeter toukiMeter)
            {
                float u = (float)toukiMeter.m_bgScroll / (float)BgScrollRange;
                toukiMeter.m_bgScrollTextureUL = SpriteUl + (u * m_bgScrollWidth);
                toukiMeter.m_bgScrollTextureUR = toukiMeter.m_bgScrollTextureUL + m_bgScrollWidth;
            }

            private void IncAnimationCount(ref ToukiMeter toukiMeter)
            {
                toukiMeter.m_animationCount++;
                if (toukiMeter.m_animationCount >= ToukiAnimationInterval)
                {
                    toukiMeter.m_animationCount = 0;
                }
            }
        }
    }
}
