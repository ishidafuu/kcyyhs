using Unity.Collections;
using Unity.Entities;
using UnityEngine; 

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
                ComponentType.ReadOnly<Status>()
            );
        }

        protected override void OnUpdate()
        {
            DrawFrame();
            DrawToukiMeter();
            DrawStatus();
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
                Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ToukiGauge].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ToukiGauge].GetMaterial(i);
                var toukiMeter = toukiMeters[i];
                float touki = (float)toukiMeter.m_value / mesh.bounds.size.x;
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(SideUtil.PosSign(i) * Settings.Instance.DrawPos.ToukiMeterX, Settings.Instance.DrawPos.ToukiMeterY, layer),
                    SideUtil.IsSideA(i) ? m_Quaternion : m_QuaternionRev,
                    Vector3.one);
                mat.SetFloat(EnumShaderParam._Value.ToString(), touki);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }

            toukiMeters.Dispose();
        }



        private void DrawStatus()
        {
            var stateMeters = m_query.ToComponentDataArray<Status>(Allocator.TempJob);

            DrawBalanceMeter(stateMeters);

            DrawLifeMeter(stateMeters);

            DrawReiMeter(stateMeters);

            stateMeters.Dispose();
        }

        private void DrawBalanceMeter(NativeArray<Status> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.BalanceGauge].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.BalanceGauge].GetMaterial(i);
                var stateMeter = stateMeters[i];
                float balance = (float)stateMeter.m_balance / Settings.Instance.Common.BalanceMax;
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(SideUtil.PosSign(i) * Settings.Instance.DrawPos.BalanceMeterX, Settings.Instance.DrawPos.BalanceMeterY, layer),
                    SideUtil.IsSideA(i) ? m_QuaternionRev : m_Quaternion,
                    Vector3.one);
                mat.SetFloat(EnumShaderParam._Value.ToString(), balance);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void DrawLifeMeter(NativeArray<Status> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.LifeGauge].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.LifeGauge].GetMaterial(i);
                var stateMeter = stateMeters[i];
                float life = (float)stateMeter.m_life / Settings.Instance.Common.LifeMax;
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(SideUtil.PosSign(i) * Settings.Instance.DrawPos.LifeMeterX, Settings.Instance.DrawPos.LifeMeterY, layer),
                    SideUtil.IsSideA(i) ? m_Quaternion : m_QuaternionRev,
                    Vector3.one);
                mat.SetFloat(EnumShaderParam._Value.ToString(), life);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void DrawReiMeter(NativeArray<Status> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiGauge].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.ReiGauge].GetMaterial(i);
                var stateMeter = stateMeters[i];
                int drawReiLen = (stateMeter.m_rei + (stateMeter.m_rei / Settings.Instance.DrawPos.ReiSeparate)) * 2;
                float rei = (float)drawReiLen / mesh.bounds.size.x;
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(SideUtil.PosSign(i) * Settings.Instance.DrawPos.ReiMeterX, Settings.Instance.DrawPos.ReiMeterY, layer),
                    SideUtil.IsSideA(i) ? m_QuaternionRev : m_Quaternion,
                    Vector3.one);
                mat.SetFloat(EnumShaderParam._Value.ToString(), rei);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void DrawSignal()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            for (int i = 0; i < Settings.Instance.Common.PlayerCount; i++)
            {
                Mesh mesh = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.Signal].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.Signal].GetMaterial(i);

                var animStep = (SideUtil.IsSideA(i))
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

                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(SideUtil.PosSign(i) * Settings.Instance.DrawPos.SignalX, Settings.Instance.DrawPos.SignalY, layer),
                    SideUtil.IsSideA(i) ? m_QuaternionRev : m_Quaternion,
                    Vector3.one);
                mat.SetFloat(EnumShaderParam._Value.ToString(), (float)signal);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }
    }
}
