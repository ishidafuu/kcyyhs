using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class ReiPieceCountSystem : JobComponentSystem
    {
        EntityQuery m_queryRei;
        EntityQuery m_queryChara;
        NativeArray<Random> m_randoms;
        protected override void OnCreate()
        {
            m_queryRei = GetEntityQuery(
                ComponentType.ReadWrite<ReiPiece>()
            );

            m_queryChara = GetEntityQuery(
                ComponentType.ReadWrite<Status>()
            );

            m_randoms = new NativeArray<Random>(1, Allocator.Persistent)
            {
                [0] = new Random(1)
            };
        }

        protected override void OnDestroy()
        {
            m_randoms.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Settings.Instance.Debug.IsSkip())
                return inputDeps;

            BattleSequencer seq = GetSingleton<BattleSequencer>();
            m_queryChara.AddDependency(inputDeps);

            NativeArray<ReiPiece> reiPieces = m_queryRei.ToComponentDataArray<ReiPiece>(Allocator.TempJob);
            NativeArray<Status> statuses = m_queryChara.ToComponentDataArray<Status>(Allocator.TempJob);

            Distribute(ref seq, reiPieces);

            var job = new CountJob()
            {
                m_reiPieces = reiPieces,
                m_statuses = statuses,
                ReiDistributeX = Settings.Instance.DrawPos.ReiDistributeX,
                ReiMeterY = Settings.Instance.DrawPos.ReiMeterY - Settings.Instance.DrawPos.ReiPoolY,
                DistributeFrame = Settings.Instance.Animation.ReiDistributeFrame,
                WaitFrame = Settings.Instance.Animation.ReiWaitFrame,
                WaitFrame2 = Settings.Instance.Animation.ReiWaitFrame2,
                ReiMax = Settings.Instance.Common.ReiMax,
                m_randoms = m_randoms,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_queryRei.CopyFromComponentDataArray(reiPieces);
            m_queryChara.CopyFromComponentDataArray(statuses);
            reiPieces.Dispose();
            statuses.Dispose();
            return inputDeps;
        }

        private void Distribute(ref BattleSequencer seq, NativeArray<ReiPiece> reiPieces)
        {
            if (seq.m_isDistributeRei)
            {
                seq.m_isDistributeRei = false;

                for (int i = 0; i < reiPieces.Length; i++)
                {
                    var reiPiece = reiPieces[i];
                    reiPiece.m_reiState = EnumReiState.Wait;
                    //TODO:攻撃結果で配分
                    reiPiece.m_isSideA = (i % 2 == 0);
                    reiPiece.m_count = 0;
                    reiPieces[i] = reiPiece;
                }

                SetSingleton(seq);
            }
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<ReiPiece> m_reiPieces;
            public NativeArray<Status> m_statuses;
            public NativeArray<Random> m_randoms;
            public int ReiDistributeX;
            public int ReiMeterY;
            public float DistributeFrame;
            public float WaitFrame;
            public int WaitFrame2;
            public int ReiMax;

            public void Execute()
            {

                for (int i = 0; i < m_reiPieces.Length; i++)
                {
                    var reiPiece = m_reiPieces[i];
                    var random = m_randoms[0];

                    reiPiece.m_count++;
                    switch (reiPiece.m_reiState)
                    {
                        case EnumReiState.Born:
                            UpdateBorn(ref reiPiece);
                            break;
                        case EnumReiState.Idle:
                            UpdateIdle(ref reiPiece);
                            break;
                        case EnumReiState.Wait:
                            UpdateWait(ref reiPiece);
                            break;
                        case EnumReiState.Distribute:
                            UpdateDistribute(ref reiPiece, ref random);
                            break;
                    }

                    m_reiPieces[i] = reiPiece;
                    m_randoms[0] = random;
                }
            }

            private void UpdateBorn(ref ReiPiece reiPiece)
            {
                reiPiece.m_movePos.x = (int)(math.sin((float)(reiPiece.m_offset) / reiPiece.m_speed) * reiPiece.m_width);
                reiPiece.m_movePos.y = (int)(math.cos((float)(reiPiece.m_offset) / reiPiece.m_speed) * reiPiece.m_width);
                if (reiPiece.m_count >= WaitFrame)
                {
                    reiPiece.m_count = 0;
                    reiPiece.m_reiState = EnumReiState.Idle;
                }
            }

            private static void UpdateIdle(ref ReiPiece reiPiece)
            {
                reiPiece.m_movePos.x = (int)(math.sin((float)(reiPiece.m_count + reiPiece.m_offset) / reiPiece.m_speed) * reiPiece.m_width);
                reiPiece.m_movePos.y = (int)(math.cos((float)(reiPiece.m_count + reiPiece.m_offset) / reiPiece.m_speed) * reiPiece.m_width);
            }

            private void UpdateWait(ref ReiPiece reiPiece)
            {
                if (reiPiece.m_count >= WaitFrame)
                {
                    reiPiece.m_basePos.x += reiPiece.m_movePos.x;
                    reiPiece.m_basePos.y += reiPiece.m_movePos.y;
                    reiPiece.m_movePos.x = 0;
                    reiPiece.m_movePos.y = 0;
                    reiPiece.m_reiState = EnumReiState.Distribute;
                    reiPiece.m_count = 0;
                }
            }

            private void UpdateDistribute(ref ReiPiece reiPiece, ref Random random)
            {
                int index = (reiPiece.m_isSideA) ? 0 : 1;
                int sign = (reiPiece.m_isSideA) ? 1 : -1;
                var status = m_statuses[index];

                int waitTime = reiPiece.m_id * WaitFrame2;
                if (reiPiece.m_count > waitTime)
                {
                    float time = (float)(reiPiece.m_count - waitTime) / DistributeFrame;
                    float2 start = new float2(0, 0);
                    float2 end = new float2((sign * ReiDistributeX) - reiPiece.m_basePos.x, ReiMeterY - reiPiece.m_basePos.y);
                    float2 pos = math.lerp(start, end, time);
                    reiPiece.m_movePos.x = (int)pos.x;
                    reiPiece.m_movePos.y = (int)pos.y;

                    if (time > 1f)
                    {
                        if (status.m_rei < ReiMax)
                        {
                            status.m_rei++;
                        }

                        reiPiece.m_reiState = EnumReiState.Born;
                        reiPiece.m_count = 0;
                        reiPiece.m_speed = random.NextFloat(10f, 20f);
                        reiPiece.m_width = random.NextFloat(2f, 4f);
                        reiPiece.m_basePos = new Vector2Int(
                            (int)(math.sin((math.PI * 2) / 6f * reiPiece.m_id) * 10),
                            (int)(math.cos((math.PI * 2) / 6f * reiPiece.m_id) * 10));
                    }
                }

                m_statuses[index] = status;
            }
        }
    }
}
