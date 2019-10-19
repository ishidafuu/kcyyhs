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
        public static YHCharaAnimList m_yhCharaAnimList;
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

            m_effectMeshMatList.LoadEffectBG(screenSprite);
            m_effectMeshMatList.LoadEffectScreen(screenSprite);
            m_effectMeshMatList.LoadEffectLarge(largeSprite);
            m_effectMeshMatList.LoadEffectMedium(mediumSprite);
            m_effectMeshMatList.LoadEffectSmall(smallSprite);
            m_effectMeshMatList.LoadFilterScreen(screenSprite);

            LoadFrameParts();

            m_yhCharaAnimList = new YHCharaAnimList();
            m_yhCharaAnimList.Init();
        }

        private static void LoadFrameParts()
        {
            m_effectMeshMatList.LoadFrameParts(CreateSprite(96, 4),
                EnumFrameParts.ToukiGauge,
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(96, 4),
                EnumFrameParts.LifeGauge,
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(64, 4),
                EnumFrameParts.BalanceGauge,
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(58, 12),
                EnumFrameParts.ReiGauge,
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(16, 8),
                EnumFrameParts.Signal,
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(16, 16),
                EnumFrameParts.ReiPiece,
                Settings.Instance.Common.ReiPieceCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(128, 128),
                EnumFrameParts.Jump,
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(128, 128),
                EnumFrameParts.Air,
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.LoadFrameParts(CreateSprite(128, 128),
                EnumFrameParts.Fall,
                Settings.Instance.Common.PlayerCount);
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
    }
}