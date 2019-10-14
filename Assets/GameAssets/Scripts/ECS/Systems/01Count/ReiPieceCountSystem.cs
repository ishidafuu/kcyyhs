using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
// using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class ReiPieceCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<ReiPiece>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();
            m_query.AddDependency(inputDeps);

            NativeArray<ReiPiece> reiPieces = m_query.ToComponentDataArray<ReiPiece>(Allocator.TempJob);

            var job = new CountJob()
            {
                m_reiPieces = reiPieces,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(reiPieces);
            reiPieces.Dispose();
            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<ReiPiece> m_reiPieces;

            public void Execute()
            {
                for (int i = 0; i < m_reiPieces.Length; i++)
                {
                    var reiPiece = m_reiPieces[i];

                    reiPiece.m_count++;
                    reiPiece.m_movePos.x = (int)(math.sin((float)reiPiece.m_count / reiPiece.m_speed) * reiPiece.m_width);
                    reiPiece.m_movePos.y = (int)(math.cos((float)reiPiece.m_count / reiPiece.m_speed) * reiPiece.m_width);
                    m_reiPieces[i] = reiPiece;
                }
            }
        }
    }
}
