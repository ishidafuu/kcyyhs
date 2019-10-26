using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
// using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Experimental.PlayerLoop;

namespace YYHS
{

    [UpdateInGroup(typeof(RenderGroup))]
    public class FilterEffectDrawSystem : ComponentSystem
    {
        EntityQuery m_query;
        Quaternion m_Quaternion = Quaternion.Euler(new Vector3(-90, 0, 0));
        Quaternion m_QuaternionRev = Quaternion.Euler(new Vector3(-90, 180, 0));

        protected override void OnCreate()
        {
            m_query = GetEntityQuery(
                ComponentType.ReadOnly<FilterEffect>()
            // ComponentType.ReadOnly<BgScroll>()
            );
        }

        protected override void OnUpdate()
        {
            NativeArray<FilterEffect> filterEffects = m_query.ToComponentDataArray<FilterEffect>(Allocator.TempJob);
            DrawFilterEffect(filterEffects);
            filterEffects.Dispose();
        }

        private void DrawFilterEffect(NativeArray<FilterEffect> filterEffects)
        {
            for (int i = 0; i < filterEffects.Length; i++)
            {
                var filterEffect = filterEffects[i];
                if (!filterEffect.m_isActive)
                    continue;

                // Debug.Log($"activeFilter:{filterEffect.m_effectIndex}");

                MeshMat meshMat;
                EnumDrawLayer layer = EnumDrawLayer.OverChara;
                Vector3 position = new Vector3(0, Settings.Instance.DrawPos.BgScrollY, (int)layer);
                Vector3 scale = Vector3.one;
                // Debug.Log($"{filterEffect.m_effectType}:{filterEffect.m_effectIndex}");
                switch (filterEffect.m_effectType)
                {
                    case EnumEffectType.EffectBG:
                        position.z = (int)EnumDrawLayer.OverBackGround;
                        meshMat = Shared.m_effectMeshMatList.m_effectBGList[filterEffect.m_effectIndex];
                        break;
                    case EnumEffectType.EffectScreen:
                        meshMat = Shared.m_effectMeshMatList.m_effectScreenList[filterEffect.m_effectIndex];
                        break;
                    case EnumEffectType.EffectLarge:
                        meshMat = Shared.m_effectMeshMatList.m_effectLargeList[filterEffect.m_effectIndex];
                        break;
                    case EnumEffectType.EffectMedium:
                        meshMat = Shared.m_effectMeshMatList.m_effectMediumList[filterEffect.m_effectIndex];
                        break;
                    case EnumEffectType.EffectSmall:
                        meshMat = Shared.m_effectMeshMatList.m_effectSmallList[filterEffect.m_effectIndex];
                        break;
                    case EnumEffectType.EffectDamageBody:
                        meshMat = Shared.m_effectMeshMatList.m_effectLargeList[(int)EnumEffectLarge.Damage];
                        UpdateEffectPos(ref position, filterEffect.m_effectType);
                        // TODO:ダメージに応じたサイズ
                        scale = new Vector3(1.5f, 1.5f, 1.5f);
                        break;
                    case EnumEffectType.EffectDamageFace:
                        meshMat = Shared.m_effectMeshMatList.m_effectLargeList[(int)EnumEffectLarge.Damage];
                        UpdateEffectPos(ref position, filterEffect.m_effectType);
                        scale = new Vector3(1.5f, 1.5f, 1.5f);
                        break;
                    case EnumEffectType.FillterScreen:
                        meshMat = Shared.m_effectMeshMatList.m_filterScreenList[filterEffect.m_effectIndex];
                        break;
                    default:
                        return;
                }

                Mesh mesh = meshMat.m_mesh;
                Material mat = meshMat.GetMaterial();
                Matrix4x4 matrixes = Matrix4x4.TRS(position,
                    filterEffect.m_isSideA ? m_Quaternion : m_QuaternionRev,
                    scale);

                Graphics.DrawMesh(mesh, matrixes, mat, 0);
            }
        }

        private void UpdateEffectPos(ref Vector3 position, EnumEffectType effectType)
        {
            BattleSequencer seq = GetSingleton<BattleSequencer>();

            if (seq.m_seqState <= EnumBattleSequenceState.Start)
                return;

            bool isSideA = seq.m_animation.m_isSideA;
            int charaNo = seq.m_animation.m_charaNo;
            EnumAnimationName animName = seq.m_animation.m_animName;
            YHAnimation anim = Shared.m_yhCharaAnimList.GetAnim(charaNo, animName);
            int count = seq.m_animation.m_count;

            foreach (YHAnimationParts item in anim.m_parts)
            {
                bool isPosition = false;
                switch (effectType)
                {
                    case EnumEffectType.EffectDamageBody:
                        isPosition = (item.m_effectPosition == EnumEffectPosition.Dodge);
                        break;
                    case EnumEffectType.EffectDamageFace:
                        // isPosition = (item.m_effectPosition == EnumEffectPosition.Face);
                        break;
                }

                if (!isPosition)
                    continue;

                YHFrameData isActive = YHAnimationUtil.GetNowFrameData(count, item.m_isActive);

                if (isActive == null || !isActive.m_value)
                    continue;

                Vector2 partsPos = YHAnimationUtil.EvaluteLocalPos(item, count, isSideA);

                position.x = partsPos.x;
                position.y = partsPos.y;
                break;
            }
        }
    }
}
