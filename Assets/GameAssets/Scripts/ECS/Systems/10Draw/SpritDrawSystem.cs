using Unity.Collections;
using Unity.Entities;
using UnityEngine; // using Unity.Transforms;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    public class SpritDrawSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        Vector3 m_SpriteBGScale = new Vector3(0.5f, 1, 1);

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<ToukiMeter>(),
                ComponentType.ReadOnly<JumpState>(),
                ComponentType.ReadOnly<DownState>(),
                ComponentType.ReadOnly<SideInfo>()
            );
        }

        protected override void OnUpdate()
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (seq.m_seqState >= EnumBattleSequenceState.Play)
                return;

            var toukiMeters = m_query.ToComponentDataArray<ToukiMeter>(Allocator.TempJob);
            var jumpStates = m_query.ToComponentDataArray<JumpState>(Allocator.TempJob);
            var downStates = m_query.ToComponentDataArray<DownState>(Allocator.TempJob);
            var sideInfos = m_query.ToComponentDataArray<SideInfo>(Allocator.TempJob);

            if (!Settings.Instance.Debug.IsShaderView && !Settings.Instance.Debug.IsCharaView)
            {
                DrawChara(toukiMeters, jumpStates, downStates, sideInfos);
                DrawFrameLine();
                DrawFilterEffect(toukiMeters, jumpStates);
            }

            toukiMeters.Dispose();
            jumpStates.Dispose();
            downStates.Dispose();
            sideInfos.Dispose();
        }

        private void DrawFrameLine()
        {
            Matrix4x4 frameLineMatrix = Matrix4x4.TRS(new Vector3(0, Settings.Instance.DrawPos.BgScrollY,
            (int)EnumDrawLayer.Frame), m_Quaternion, Vector3.one);
            Graphics.DrawMesh(Shared.m_commonMeshMat.m_meshDict[EnumCommonPartsType.frame_line.ToString()],
                frameLineMatrix,
                Shared.m_commonMeshMat.m_materialDict[EnumCommonPartsType.frame_line.ToString()], 0);
        }

        private void DrawChara(NativeArray<ToukiMeter> toukiMeters, NativeArray<JumpState> jumpStates,
             NativeArray<DownState> downStates, NativeArray<SideInfo> sideInfos)
        {
            for (int i = 0; i < toukiMeters.Length; i++)
            {
                var toukiMeter = toukiMeters[i];
                var downState = downStates[i];
                var sideInfo = sideInfos[i];
                int charaNo = sideInfo.m_charaNo;
                bool isSideA = sideInfo.m_isSideA;
                int basePosX = SideUtil.PosSign(isSideA) * Settings.Instance.DrawPos.BgScrollX;

                EnumAnimationName animName = EnumAnimationName._Stand00;
                int count = 0;

                switch (downState.m_state)
                {
                    case EnumDownState.None:
                        {
                            var jumpState = jumpStates[i];
                            count = (jumpState.m_state == EnumJumpState.None)
                                ? toukiMeter.m_animationCount
                                : jumpState.m_animationCount;
                            animName = SwitchStandAnim(toukiMeter, sideInfo);
                            animName = SwitchJumpAnim(animName, jumpState);
                        }
                        break;
                    case EnumDownState.Down:
                        animName = EnumAnimationName._Down00;
                        break;
                    case EnumDownState.Reverse:
                        animName = EnumAnimationName._Down01;
                        count = downState.m_count;
                        break;
                }

                bool isDrawBackGround = YHAnimationUtil.DrawYHAnimation(animName, charaNo, count, basePosX, sideInfo.m_isSideA, true);

                if (!isDrawBackGround)
                {
                    DrawBackGround(basePosX, ref toukiMeter);
                }
            }
        }

        private void DrawBackGround(int basePosX, ref ToukiMeter toukiMeter)
        {
            string bgName = EnumBGPartsType.bg_00.ToString();

            Mesh baseMesh = Shared.m_bgFrameMeshMat.m_meshDict[bgName];
            Material mat = Shared.m_bgFrameMeshMat.m_materialDict[bgName];
            Vector3 pos = new Vector3(basePosX, Settings.Instance.DrawPos.BgScrollY,
                    (int)EnumDrawLayer.BackGround);

            Matrix4x4 bgScrollMatrixes = Matrix4x4.TRS(pos, m_Quaternion, m_SpriteBGScale);

            Mesh mesh = new Mesh()
            {
                vertices = baseMesh.vertices,
                uv = new Vector2[]
                {
                    new Vector2(toukiMeter.m_bgScrollTextureUL, baseMesh.uv[0].y),
                    new Vector2(toukiMeter.m_bgScrollTextureUR, baseMesh.uv[1].y),
                    new Vector2(toukiMeter.m_bgScrollTextureUL, baseMesh.uv[2].y),
                    new Vector2(toukiMeter.m_bgScrollTextureUR, baseMesh.uv[3].y),
                },
                triangles = baseMesh.triangles,
            };
            Graphics.DrawMesh(mesh, bgScrollMatrixes, mat, 0);
        }

        private static EnumAnimationName SwitchStandAnim(ToukiMeter toukiMeter, SideInfo sideInfo)
        {
            if ((toukiMeter.m_cross == EnumCrossType.Right && sideInfo.m_isSideA)
                || (toukiMeter.m_cross == EnumCrossType.Left && !sideInfo.m_isSideA)
                || toukiMeter.m_cross == EnumCrossType.Up)
            {
                return EnumAnimationName._Stand01;
            }
            else if ((toukiMeter.m_cross == EnumCrossType.Right && !sideInfo.m_isSideA)
                || (toukiMeter.m_cross == EnumCrossType.Left && sideInfo.m_isSideA)
                || toukiMeter.m_cross == EnumCrossType.Down)
            {
                return EnumAnimationName._Stand02;
            }

            return EnumAnimationName._Stand00;
        }

        private static EnumAnimationName SwitchJumpAnim(EnumAnimationName animName, JumpState jumpState)
        {
            switch (jumpState.m_state)
            {
                case EnumJumpState.Jumping:
                    animName = EnumAnimationName._Jump00;
                    break;
                case EnumJumpState.Air:
                    switch (animName)
                    {
                        case EnumAnimationName._Stand01:
                            animName = EnumAnimationName._Air01;
                            break;
                        case EnumAnimationName._Stand02:
                            animName = EnumAnimationName._Air02;
                            break;
                        default:
                            animName = EnumAnimationName._Air00;
                            break;
                    }
                    break;
                case EnumJumpState.Falling:
                    animName = EnumAnimationName._Jump01;
                    break;
            }

            return animName;
        }

        private void DrawFilterEffect(NativeArray<ToukiMeter> toukiMeters, NativeArray<JumpState> jumpStates)
        {
            for (int i = 0; i < jumpStates.Length; i++)
            {
                var jumpState = jumpStates[i];
                if (jumpState.m_state == EnumJumpState.None)
                    continue;

                MeshMat meshMat = null;

                bool isFadeIn = false;
                switch (jumpState.m_effectStep)
                {
                    case EnumJumpEffectStep.InJumping:
                        meshMat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.Jump];
                        isFadeIn = true;
                        break;
                    case EnumJumpEffectStep.OutJumping:
                        meshMat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.Jump];
                        break;
                    case EnumJumpEffectStep.InFalling:
                        meshMat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.Fall];
                        isFadeIn = true;
                        break;
                    case EnumJumpEffectStep.OutFalling:
                        meshMat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.Fall];
                        break;
                    case EnumJumpEffectStep.Air:
                        meshMat = Shared.m_effectMeshMatList.m_framePartsList[(int)EnumFrameParts.Air];
                        break;
                }

                if (meshMat == null)
                    continue;

                float alpha = isFadeIn
                    ? (float)jumpState.m_stepCount / (float)Settings.Instance.Animation.JumpFadeFrame
                    : (float)(Settings.Instance.Animation.JumpFadeFrame - jumpState.m_stepCount) / (float)Settings.Instance.Animation.JumpFadeFrame;
                alpha = Mathf.Clamp01(alpha);
                int posX = SideUtil.PosSign(i) * Settings.Instance.DrawPos.BgScrollX;

                EnumDrawLayer layer = EnumDrawLayer.OverBackGround;
                Vector3 position = new Vector3(posX, Settings.Instance.DrawPos.BgScrollY, (int)layer);
                Vector3 scale = Vector3.one;
                Mesh mesh = meshMat.m_mesh;
                Material mat = meshMat.GetMaterial(i);
                Matrix4x4 matrixes = Matrix4x4.TRS(position, m_Quaternion, scale);
                mat.SetFloat(EnumShaderParam._Alpha.ToString(), alpha);

                // Debug.Log($"{jumpState.m_effectStep}:{alpha}");

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }
    }
}
