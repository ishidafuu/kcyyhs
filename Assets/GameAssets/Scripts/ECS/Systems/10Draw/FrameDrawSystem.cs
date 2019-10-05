using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    public class FrameDrawSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        Quaternion m_QuaternionRev = Quaternion.Euler(new Vector3(-90, 180, 0));

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<ToukiMeter>(),
                ComponentType.ReadOnly<StateMeter>()
            );
        }

        protected override void OnUpdate()
        {
            DrawFrame();
            DrawToukiMeter();
            DrawStateMeter();
            DrawSignal();
        }

        private void DrawFrame()
        {
            Matrix4x4 frameTopMatrix = Matrix4x4.TRS(new Vector3(0, Settings.Instance.DrawPos.FrameTopY,
            (int)EnumDrawLayer.Frame), m_Quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.m_commonMeshMat.m_meshDict[EnumCommonPartsType.frame_top.ToString()],
                frameTopMatrix,
                Shared.m_commonMeshMat.m_materialDict[EnumCommonPartsType.frame_top.ToString()], 0);

            Matrix4x4 frameBottomMatrix = Matrix4x4.TRS(new Vector3(0, Settings.Instance.DrawPos.FrameBottomY,
            (int)EnumDrawLayer.Frame), m_Quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.m_commonMeshMat.m_meshDict[EnumCommonPartsType.frame_bottom.ToString()],
                frameBottomMatrix,
                Shared.m_commonMeshMat.m_materialDict[EnumCommonPartsType.frame_bottom.ToString()], 0);
        }

        private void DrawToukiMeter()
        {
            var toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);

            for (int i = 0; i < toukiMeters.Length; i++)
            {
                bool isSideA = (i == 0);
                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Touki].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Touki].GetMaterial(i);
                var toukiMeter = toukiMeters[i];
                float touki = (float)toukiMeter.m_value / mesh.bounds.size.x;
                float sign = isSideA ? +1 : -1;

                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(sign * Settings.Instance.DrawPos.ToukiMeterX, Settings.Instance.DrawPos.ToukiMeterY, layer),
                    isSideA ? m_Quaternion : m_QuaternionRev,
                    Vector3.one);
                mat.SetFloat("_Value", touki);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }

            toukiMeters.Dispose();
        }



        private void DrawStateMeter()
        {
            var stateMeters = m_query.ToComponentDataArray<StateMeter>(Allocator.TempJob);

            DrawBalanceMeter(stateMeters);

            DrawLifeMeter(stateMeters);

            DrawReiMeter(stateMeters);

            stateMeters.Dispose();
        }

        private void DrawBalanceMeter(NativeArray<StateMeter> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                bool isSideA = (i == 0);
                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Balance].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Balance].GetMaterial(i);
                var stateMeter = stateMeters[i];
                float balance = (float)stateMeter.m_balance / mesh.bounds.size.x;

                float sign = isSideA ? +1 : -1;
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(sign * Settings.Instance.DrawPos.BalanceMeterX, Settings.Instance.DrawPos.BalanceMeterY, layer),
                    isSideA ? m_QuaternionRev : m_Quaternion,
                    Vector3.one);
                mat.SetFloat("_Value", balance);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void DrawLifeMeter(NativeArray<StateMeter> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                bool isSideA = (i == 0);
                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Life].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Life].GetMaterial(i);
                var stateMeter = stateMeters[i];
                float life = (float)stateMeter.m_life / mesh.bounds.size.x;

                float sign = isSideA ? +1 : -1;

                float posX = (i == 0)
                    ? Settings.Instance.DrawPos.LifeMeterX
                    : -Settings.Instance.DrawPos.LifeMeterX;

                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(sign * Settings.Instance.DrawPos.LifeMeterX, Settings.Instance.DrawPos.LifeMeterY, layer),
                    isSideA ? m_Quaternion : m_QuaternionRev,
                    Vector3.one);
                mat.SetFloat("_Value", life);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void DrawReiMeter(NativeArray<StateMeter> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                bool isSideA = (i == 0);
                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Rei].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Rei].GetMaterial(i);
                var stateMeter = stateMeters[i];
                int drawReiLen = (stateMeter.m_rei + (stateMeter.m_rei / Settings.Instance.DrawPos.ReiSeparate)) * 2;
                // drawReiLen = (Settings.Instance.Debug.ShaderFrame + (Settings.Instance.Debug.ShaderFrame / Settings.Instance.DrawPos.ReiSeparate)) * 2;
                float rei = (float)drawReiLen / mesh.bounds.size.x;

                float sign = isSideA ? +1 : -1;

                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(sign * Settings.Instance.DrawPos.ReiMeterX, Settings.Instance.DrawPos.ReiMeterY, layer),
                    isSideA ? m_QuaternionRev : m_Quaternion,
                    Vector3.one);
                mat.SetFloat("_Value", rei);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void DrawSignal()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            for (int i = 0; i < Settings.Instance.Common.PlayerCount; i++)
            {
                bool isSideA = (i == 0);
                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Signal].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Signal].GetMaterial(i);

                var animStep = (isSideA)
                    ? seq.m_sideA.m_animStep
                    : seq.m_sideB.m_animStep;

                EnumSignal signal = EnumSignal.Decide;
                switch (animStep)
                {
                    case EnumAnimationStep.Sleep:
                        signal = EnumSignal.Sleep;
                        break;
                    case EnumAnimationStep.Skip:
                        signal = EnumSignal.Skip;
                        break;
                }

                float sign = isSideA ? +1 : -1;
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(sign * Settings.Instance.DrawPos.SignalX, Settings.Instance.DrawPos.SignalY, layer),
                    isSideA ? m_QuaternionRev : m_Quaternion,
                    Vector3.one);
                mat.SetFloat("_Value", (float)signal);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }
    }
}
