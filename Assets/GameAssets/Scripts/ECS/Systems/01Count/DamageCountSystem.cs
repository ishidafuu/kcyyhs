using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Mathematics;
// using Unity.Transforms;
using UnityEngine;

namespace YYHS
{
    [UpdateInGroup(typeof(CountGroup))]
    public class DamageCountSystem : JobComponentSystem
    {
        EntityQuery m_query;

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadWrite<DamageState>(),
                ComponentType.ReadWrite<Status>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (Settings.Instance.Debug.IsSkip())
                return inputDeps;

            BattleSequencer seq = GetSingleton<BattleSequencer>();


            m_query.AddDependency(inputDeps);
            NativeArray<DamageState> damageStates = m_query.ToComponentDataArray<DamageState>(Allocator.TempJob);
            NativeArray<Status> statuses = m_query.ToComponentDataArray<Status>(Allocator.TempJob);


            var job = new CountJob()
            {
                m_damageStates = damageStates,
                m_statuses = statuses,
                m_seq = seq,
                LifeMax = Settings.Instance.Common.LifeMax,
                BalanceMax = Settings.Instance.Common.BalanceMax,
                IsDebugHeal = Settings.Instance.Debug.IsHeal,
                DamageSpeed = Settings.Instance.Common.DamageSpeed,
            };

            inputDeps = job.Schedule(inputDeps);
            inputDeps.Complete();
            m_query.CopyFromComponentDataArray(damageStates);
            m_query.CopyFromComponentDataArray(statuses);
            damageStates.Dispose();
            statuses.Dispose();



            return inputDeps;
        }

        // [BurstCompileAttribute]
        struct CountJob : IJob
        {
            public NativeArray<DamageState> m_damageStates;
            public NativeArray<Status> m_statuses;
            [ReadOnly] public BattleSequencer m_seq;
            public int LifeMax;
            public int BalanceMax;
            public bool IsDebugHeal;
            public int DamageSpeed;

            public void Execute()
            {
                for (int i = 0; i < m_damageStates.Length; i++)
                {
                    var damageState = m_damageStates[i];
                    var status = m_statuses[i];
                    if (m_seq.m_seqState == EnumBattleSequenceState.Idle)
                    {
                        if (!IsDebugHeal)
                            continue;

                        status.m_life += DamageSpeed;
                        if (status.m_life > LifeMax)
                        {
                            status.m_life = LifeMax;
                        }

                        status.m_balance += DamageSpeed;
                        if (status.m_balance > BalanceMax)
                        {
                            status.m_balance = BalanceMax;
                        }
                        m_statuses[i] = status;
                    }
                    else
                    {

                        bool isSideA = SideUtil.IsSideA(i);

                        if (isSideA)
                        {
                            if (!m_seq.m_sideA.m_isStartDamage)
                                continue;
                        }
                        else
                        {
                            if (!m_seq.m_sideB.m_isStartDamage)
                                continue;
                        }


                        if (damageState.m_lifeDamage > 0)
                        {
                            damageState.m_lifeDamage -= DamageSpeed;
                            status.m_life -= DamageSpeed;
                            if (status.m_life < 0)
                            {
                                status.m_life = 0;
                            }
                        }

                        if (damageState.m_balanceDamage > 0)
                        {
                            damageState.m_balanceDamage -= DamageSpeed;
                            status.m_balance -= DamageSpeed;
                            if (status.m_balance < 0)
                            {
                                status.m_balance = 0;
                            }
                        }

                        m_statuses[i] = status;
                        m_damageStates[i] = damageState;
                    }

                }
            }
        }
    }
}
