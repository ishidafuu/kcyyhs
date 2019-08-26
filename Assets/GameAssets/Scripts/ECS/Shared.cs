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

        static readonly string DefaultShader = "Sprites/DefaultSprite";

        public static void ReadySharedComponentData()
        {
            commonMeshMat = new MeshMatList(PathSettings.CommonSprite, DefaultShader);
            bgFrameMeshMat = new MeshMatList(GetBackGroundPath(0), DefaultShader);
            // meterMeshMat = new MeshMatList("yyhs/bg/meter", ShaderBg);

            // // スクリプタブルオブジェクトの読み込み
            // aniScriptSheet = new AniScriptSheet();
            // if (Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().Length == 0)
            // 	Debug.LogError("aniScriptSheet 0");
            // aniScriptSheet.scripts = (Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().First()as AniScriptSheetObject).scripts;

            // aniBasePos = new AniBasePos();
            // if (Resources.FindObjectsOfTypeAll<AniBasePosObject>().Length == 0)
            // 	Debug.LogError("aniBasePos 0");
            // aniBasePos = (Resources.FindObjectsOfTypeAll<AniBasePosObject>().First()as AniBasePosObject).aniBasePos;
        }

        private static string GetBackGroundPath(int bgNo)
        {
            return string.Format(PathSettings.BackGroundSprite, bgNo.ToString("d2"));
        }

    }
}