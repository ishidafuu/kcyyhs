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
                var toukiMeter = toukiMeters[i];
                float touki = (float)toukiMeter.m_value / (float)Settings.Instance.Common.ToukiMax;
                // -72 -37
                float posX = (i == 0)
                    ? Settings.Instance.DrawPos.ToukiMeterX
                    : -Settings.Instance.DrawPos.ToukiMeterX;

                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Touki].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Touki].GetMaterial(i);
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(posX, Settings.Instance.DrawPos.ToukiMeterY, layer),
                    m_Quaternion, Vector3.one);
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

            stateMeters.Dispose();
        }

        private void DrawBalanceMeter(NativeArray<StateMeter> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                var stateMeter = stateMeters[i];
                float balance = (float)stateMeter.m_balance / (float)Settings.Instance.Common.BalanceMax;

                float posX = (i == 0)
                    ? Settings.Instance.DrawPos.BalanceMeterX
                    : -Settings.Instance.DrawPos.BalanceMeterX;

                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Balance].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Balance].GetMaterial(i);
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(posX, Settings.Instance.DrawPos.BalanceMeterY, layer),
                    m_Quaternion, Vector3.one);
                mat.SetFloat("_Value", balance);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void DrawLifeMeter(NativeArray<StateMeter> stateMeters)
        {
            for (int i = 0; i < stateMeters.Length; i++)
            {
                var stateMeter = stateMeters[i];
                float life = (float)stateMeter.m_life / (float)Settings.Instance.Common.LifeMax;

                float posX = (i == 0)
                    ? Settings.Instance.DrawPos.LifeMeterX
                    : -Settings.Instance.DrawPos.LifeMeterX;

                Mesh mesh = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Life].m_mesh;
                Material mat = Shared.m_effectMeshMatList.m_gaugeList[(int)EnumGauge.Life].GetMaterial(i);
                int layer = (int)EnumDrawLayer.UnderFrame;

                Matrix4x4 matrixes = Matrix4x4.TRS(
                    new Vector3(posX, Settings.Instance.DrawPos.LifeMeterY, layer),
                    m_Quaternion, Vector3.one);
                mat.SetFloat("_Value", life);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }
    }
}
