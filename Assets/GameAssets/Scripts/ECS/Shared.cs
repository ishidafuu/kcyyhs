using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YYHS
{
    public static class Shared
    {
        // SharedComponentData
        public static MeshMatList[] m_charaMeshMat = new MeshMatList[2];
        public static MeshMatList m_bgFrameMeshMat;
        public static MeshMatList m_commonMeshMat;

        public static EffectMeshMatList m_effectMeshMatList;

        // public static YHFilterEffectList m_yhFilterEffectList;
        public static YHCharaAnimList m_yhCharaAnimList;

        public static readonly int EffectScreenCount = 16;
        public static readonly int EffectLargeCount = 1;
        public static readonly int EffectMediumCount = 0;
        public static readonly int EffectSmallCount = 0;
        public static readonly int ScreenFillterCount = 2;
        public static readonly int BGFillterCount = 3;
        public static int m_testShaderNo = EffectScreenCount - 1;

        static readonly string DefaultShader = "Sprites/DefaultSprite";

        public static void ReadySharedComponentData()
        {
            m_charaMeshMat[0] = new MeshMatList(GetCharaPath(0), DefaultShader);
            m_charaMeshMat[1] = new MeshMatList(GetCharaPath(1), DefaultShader);
            m_bgFrameMeshMat = new MeshMatList(GetBackGroundPath(0), DefaultShader);
            m_commonMeshMat = new MeshMatList(PathSettings.CommonSprite, DefaultShader);

            Sprite screenSprite = CreateSprite();
            Sprite largeSprite = CreateSprite(64, 64);
            Sprite mediumSprite = CreateSprite(32, 32);
            Sprite smallSprite = CreateSprite(16, 16);


            for (int i = 0; i < EffectScreenCount; i++)
            {
                m_effectMeshMatList.AddEffectScreen(screenSprite, GetEffectScreenShaderName(i));
            }

            for (int i = 0; i < EffectLargeCount; i++)
            {
                m_effectMeshMatList.AddEffectLarge(largeSprite, GetEffectLargeShaderName(i));
            }

            for (int i = 0; i < EffectMediumCount; i++)
            {
                m_effectMeshMatList.AddEffectMedium(mediumSprite, GetEffectMediumShaderName(i));
            }

            for (int i = 0; i < EffectSmallCount; i++)
            {
                m_effectMeshMatList.AddEffectSmall(smallSprite, GetEffectSmallShaderName(i));
            }

            for (int i = 0; i < ScreenFillterCount; i++)
            {
                m_effectMeshMatList.AddFilterScreen(screenSprite, GetFillterScreenShaderName(i));
            }

            for (int i = 0; i < BGFillterCount; i++)
            {
                m_effectMeshMatList.AddFilterBG(screenSprite, GetFillterBGShaderName(i));
            }

            AddGauge();


            m_yhCharaAnimList = new YHCharaAnimList();
            m_yhCharaAnimList.Init();
        }

        private static void AddGauge()
        {
            m_effectMeshMatList.AddFrameParts(CreateSprite(96, 4),
                GetFramePartsShaderName(EnumFrameParts.ToukiGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateSprite(96, 4),
                GetFramePartsShaderName(EnumFrameParts.LifeGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateSprite(64, 4),
                GetFramePartsShaderName(EnumFrameParts.BalanceGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateSprite(58, 12),
                GetFramePartsShaderName(EnumFrameParts.ReiGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateSprite(16, 8),
                GetFramePartsShaderName(EnumFrameParts.Signal),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateSprite(16, 16),
                GetFramePartsShaderName(EnumFrameParts.ReiPiece),
                Settings.Instance.Common.ReiPieceCount);
        }

        private static string GetBGFillterShaderName(object i)
        {
            throw new NotImplementedException();
        }

        private static Sprite CreateSprite(int width = 256, int height = 256, float pivot = 0.5f)
        {
            // const int WIDTH = 256;
            // const int HEIGHT = 256;
            // const float PIVOT = 0.5f;
            const int PIXEL_PER_UNIT = 1;
            Texture2D texture2D = new Texture2D(width, height);
            return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, width, height), new Vector2(pivot, pivot), PIXEL_PER_UNIT);
        }

        private static string GetBackGroundPath(int bgNo) => string.Format(PathSettings.BackGroundSprite, bgNo.ToString("d2"));
        private static string GetCharaPath(int charaNo) => string.Format(PathSettings.CharaSprite, charaNo.ToString("d2"));
        private static string GetEffectSpritePath(EnumShaderBaseTexture effectNo) => string.Format(PathSettings.EffectSprite, ((int)effectNo).ToString("d2"));
        private static string GetEffectMaterialPath(int effectNo) => string.Format(PathSettings.EffectMaterial, effectNo.ToString("d2"));
        private static string GetEffectSmallShaderName(int effectNo) => string.Format(PathSettings.EffectSmallShader, effectNo.ToString("d2"));
        private static string GetEffectMediumShaderName(int effectNo) => string.Format(PathSettings.EffectMediumShader, effectNo.ToString("d2"));
        private static string GetEffectLargeShaderName(int effectNo) => string.Format(PathSettings.EffectLargeShader, effectNo.ToString("d2"));
        private static string GetEffectScreenShaderName(int effectNo) => string.Format(PathSettings.EffectScreenShader, effectNo.ToString("d2"));
        private static string GetFillterScreenShaderName(int fillterNo) => string.Format(PathSettings.FillterScreenShader, fillterNo.ToString("d2"));
        private static string GetFillterBGShaderName(int fillterNo) => string.Format(PathSettings.FillterBGShader, fillterNo.ToString("d2"));
        private static string GetFramePartsShaderName(EnumFrameParts objNo) => string.Format(PathSettings.FramePartsShader, ((int)objNo).ToString("d2"));
    }
}