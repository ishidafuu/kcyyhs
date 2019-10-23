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
        EntityQuery m_query;
        NativeArray<Random> m_randoms;
        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<ReiPiece>()
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
            m_query.AddDependency(inputDeps);

            NativeArray<ReiPiece> reiPieces = m_query.ToComponentDataArray<ReiPiece>(Allocator.TempJob);

            Distribute(ref seq, reiPieces);

            var job = new CountJob()
            {
                m_reiPieces = reiPieces,
                ReiMeterX = Settings.Instance.DrawPos.ReiMeterX,
                ReiMeterY = Settings.Instance.DrawPos.ReiMeterY - Settings.Instance.DrawPos.ReiPoolY,
                DistributeFrame = Settings.Instance.Animation.ReiDistributeFrame,
                m_randoms = m_randoms,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(reiPieces);
            reiPieces.Dispose();
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
            public int ReiMeterX;
            public int ReiMeterY;
            public float DistributeFrame;
            public NativeArray<Random> m_randoms;

            public void Execute()
            {
                for (int i = 0; i < m_reiPieces.Length; i++)
                {
                    var reiPiece = m_reiPieces[i];
                    var random = m_randoms[0];

                    reiPiece.m_count++;
                    switch (reiPiece.m_reiState)
                    {
                        case EnumReiState.Idle:
                            reiPiece.m_movePos.x = (int)(math.sin((float)reiPiece.m_count / reiPiece.m_speed) * reiPiece.m_width);
                            reiPiece.m_movePos.y = (int)(math.cos((float)reiPiece.m_count / reiPiece.m_speed) * reiPiece.m_width);
                            break;
                        case EnumReiState.Wait:
                            if (reiPiece.m_count > 60)
                            {
                                reiPiece.m_basePos.x += reiPiece.m_movePos.x;
                                reiPiece.m_basePos.y += reiPiece.m_movePos.y;
                                reiPiece.m_movePos.x = 0;
                                reiPiece.m_movePos.y = 0;
                                reiPiece.m_reiState = EnumReiState.Distribute;
                                reiPiece.m_count = 0;
                            }
                            break;
                        case EnumReiState.Distribute:
                            int sign = (reiPiece.m_isSideA) ? 1 : -1;
                            float time = (float)reiPiece.m_count / DistributeFrame;
                            float2 start = new float2(0, 0);
                            float2 end = new float2((sign * ReiMeterX) - reiPiece.m_basePos.x, ReiMeterY - reiPiece.m_basePos.y);
                            float2 pos = math.lerp(start, end, time);
                            reiPiece.m_movePos.x = (int)pos.x;
                            reiPiece.m_movePos.y = (int)pos.y;
                            if (time > 1f)
                            {
                                reiPiece.m_reiState = EnumReiState.Idle;
                                reiPiece.m_count = random.NextInt(0, 60);
                                reiPiece.m_speed = random.NextFloat(10f, 20f);
                                reiPiece.m_width = random.NextFloat(2f, 4f);
                                reiPiece.m_basePos = new Vector2Int(
                                    (int)(math.sin((math.PI * 2) / 6f * i) * 10),
                                    (int)(math.cos((math.PI * 2) / 6f * i) * 10));
                            }
                            break;
                    }
                    m_reiPieces[i] = reiPiece;
                    m_randoms[0] = random;
                }
            }
        }
    }
}
