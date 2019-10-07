﻿using System;
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

        public static readonly int EffectCount = 17;
        public static readonly int ScreenFillterCount = 2;
        public static readonly int BGFillterCount = 1;
        public static int m_testShaderNo = EffectCount - 1;

        static readonly string DefaultShader = "Sprites/DefaultSprite";

        public static void ReadySharedComponentData()
        {
            m_charaMeshMat[0] = new MeshMatList(GetCharaPath(0), DefaultShader);
            m_charaMeshMat[1] = new MeshMatList(GetCharaPath(1), DefaultShader);
            m_bgFrameMeshMat = new MeshMatList(GetBackGroundPath(0), DefaultShader);
            m_commonMeshMat = new MeshMatList(PathSettings.CommonSprite, DefaultShader);

            Sprite sprite = CreateFilterSprite();

            for (int i = 0; i < EffectCount; i++)
            {
                m_effectMeshMatList.AddEffect(sprite, GetEffectShaderName(i));
            }

            for (int i = 0; i < ScreenFillterCount; i++)
            {
                m_effectMeshMatList.AddScreenFilter(sprite, GetScreenFillterShaderName(i));
            }

            for (int i = 0; i < BGFillterCount; i++)
            {
                m_effectMeshMatList.AddBGFilter(sprite, GetBGFillterShaderName(i));
            }

            AddGauge();


            m_yhCharaAnimList = new YHCharaAnimList();
            m_yhCharaAnimList.Init();
        }

        private static void AddGauge()
        {
            m_effectMeshMatList.AddFrameParts(CreateFilterSprite(96, 4),
                GetFramePartsShaderName(EnumFrameParts.ToukiGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateFilterSprite(96, 4),
                GetFramePartsShaderName(EnumFrameParts.LifeGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateFilterSprite(64, 4),
                GetFramePartsShaderName(EnumFrameParts.BalanceGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateFilterSprite(58, 12),
                GetFramePartsShaderName(EnumFrameParts.ReiGauge),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateFilterSprite(16, 8),
                GetFramePartsShaderName(EnumFrameParts.Signal),
                Settings.Instance.Common.PlayerCount);
            m_effectMeshMatList.AddFrameParts(CreateFilterSprite(16, 16),
                GetFramePartsShaderName(EnumFrameParts.ReiPiece),
                Settings.Instance.Common.ReiPieceCount);
        }

        private static string GetBGFillterShaderName(object i)
        {
            throw new NotImplementedException();
        }

        private static Sprite CreateFilterSprite(int width = 256, int height = 256, float pivot = 0.5f)
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
        private static string GetEffectShaderName(int effectNo) => string.Format(PathSettings.EffectShader, effectNo.ToString("d2"));
        private static string GetScreenFillterShaderName(int fillterNo) => string.Format(PathSettings.ScreenFillterShader, fillterNo.ToString("d2"));
        private static string GetBGFillterShaderName(int fillterNo) => string.Format(PathSettings.BGFillterShader, fillterNo.ToString("d2"));
        private static string GetFramePartsShaderName(EnumFrameParts objNo) => string.Format(PathSettings.FramePartsShader, ((int)objNo).ToString("d2"));
    }
}