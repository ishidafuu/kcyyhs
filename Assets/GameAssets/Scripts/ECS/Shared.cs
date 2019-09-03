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
        public static MeshMatList charaMeshMat;
        public static MeshMatList bgFrameMeshMat;
        public static MeshMatList commonMeshMat;

        public static YHFilterEffectList yhFilterEffectList;
        public static YHCharaAnimList yhCharaAnimList;


        static readonly string DefaultShader = "Sprites/DefaultSprite";

        public static void ReadySharedComponentData()
        {
            charaMeshMat = new MeshMatList(GetCharaPath(0), DefaultShader);
            bgFrameMeshMat = new MeshMatList(GetBackGroundPath(0), DefaultShader);
            commonMeshMat = new MeshMatList(PathSettings.CommonSprite, DefaultShader);

            yhFilterEffectList = new YHFilterEffectList();
            yhFilterEffectList.Init();

            yhCharaAnimList = new YHCharaAnimList();
            yhCharaAnimList.Init();
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