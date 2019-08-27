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

        public static YHFilterEffectList effectList;

        static readonly string DefaultShader = "Sprites/DefaultSprite";

        public static void ReadySharedComponentData()
        {
            bgFrameMeshMat = new MeshMatList(GetBackGroundPath(0), DefaultShader);
            commonMeshMat = new MeshMatList(PathSettings.CommonSprite, DefaultShader);
            effectList = new YHFilterEffectList();
            effectList.Init();
        }

        private static string GetBackGroundPath(int bgNo)
        {
            return string.Format(PathSettings.BackGroundSprite, bgNo.ToString("d2"));
        }

    }
}