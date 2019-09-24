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
        public static MeshMatList m_charaMeshMat;
        public static MeshMatList m_bgFrameMeshMat;
        public static MeshMatList m_commonMeshMat;

        public static EffectMeshMatList m_effectMeshMatList;

        // public static YHFilterEffectList m_yhFilterEffectList;
        public static YHCharaAnimList m_yhCharaAnimList;

        public static readonly int EffectCount = 14;
        public static readonly int FillterCount = 1;
        public static int m_testShaderNo = EffectCount - 1;


        static readonly string DefaultShader = "Sprites/DefaultSprite";

        public static void ReadySharedComponentData()
        {
            m_charaMeshMat = new MeshMatList(GetCharaPath(0), DefaultShader);
            m_bgFrameMeshMat = new MeshMatList(GetBackGroundPath(0), DefaultShader);
            m_commonMeshMat = new MeshMatList(PathSettings.CommonSprite, DefaultShader);

            Sprite sprite = CreateFilterSprite();

            for (int i = 0; i < EffectCount; i++)
            {
                m_effectMeshMatList.AddEffect(sprite, GetEffectShaderName(i));
            }

            for (int i = 0; i < FillterCount; i++)
            {
                m_effectMeshMatList.AddFilter(sprite, GetFillterShaderName(i));
            }
            //GetEffectSpritePath(EnumShaderBaseTexture.BigQuad)

            // m_yhFilterEffectList = new YHFilterEffectList();
            // m_yhFilterEffectList.Init();

            m_yhCharaAnimList = new YHCharaAnimList();
            m_yhCharaAnimList.Init();
        }

        private static Sprite CreateFilterSprite()
        {
            const int WIDTH = 256;
            const int HEIGHT = 128;
            const float PIVOT = 0.5f;
            const int PIXEL_PER_UNIT = 1;
            Texture2D texture2D = new Texture2D(WIDTH, HEIGHT);
            return Sprite.Create(texture2D, new Rect(0.0f, 0.0f, WIDTH, HEIGHT), new Vector2(PIVOT, PIVOT), PIXEL_PER_UNIT);
        }

        private static string GetBackGroundPath(int bgNo) => string.Format(PathSettings.BackGroundSprite, bgNo.ToString("d2"));
        private static string GetCharaPath(int charaNo) => string.Format(PathSettings.CharaSprite, charaNo.ToString("d2"));
        private static string GetEffectSpritePath(EnumShaderBaseTexture effectNo) => string.Format(PathSettings.EffectSprite, ((int)effectNo).ToString("d2"));
        private static string GetEffectMaterialPath(int effectNo) => string.Format(PathSettings.EffectMaterial, effectNo.ToString("d2"));
        private static string GetEffectShaderName(int effectNo) => string.Format(PathSettings.EffectShader, effectNo.ToString("d2"));
        private static string GetFillterShaderName(int fillterNo) => string.Format(PathSettings.FillterShader, fillterNo.ToString("d2"));
    }
}