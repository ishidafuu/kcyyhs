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

        public static YHFilterEffectList m_yhFilterEffectList;
        public static YHCharaAnimList m_yhCharaAnimList;


        static readonly string DefaultShader = "Sprites/DefaultSprite";

        public static void ReadySharedComponentData()
        {
            m_charaMeshMat = new MeshMatList(GetCharaPath(0), DefaultShader);
            m_bgFrameMeshMat = new MeshMatList(GetBackGroundPath(0), DefaultShader);
            m_commonMeshMat = new MeshMatList(PathSettings.CommonSprite, DefaultShader);

            m_yhFilterEffectList = new YHFilterEffectList();
            m_yhFilterEffectList.Init();

            m_yhCharaAnimList = new YHCharaAnimList();
            m_yhCharaAnimList.Init();
        }

        private static string GetBackGroundPath(int bgNo)
        {
            return string.Format(PathSettings.BackGroundSprite, bgNo.ToString("d2"));
        }

        private static string GetCharaPath(int charaNo)
        {
            return string.Format(PathSettings.CharaSprite, charaNo.ToString("d2"));
        }

    }
}