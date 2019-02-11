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

namespace NKKD
{
	public static class Shared
	{
		//SharedComponentData
		public static MeshMatList charaMeshMat;
		public static AniScriptSheet aniScriptSheet;
		public static AniBasePos aniBasePos;
		public static MeshMatList geoMeshMat;
		public static MeshMatList foodMeshMat;

		//SharedComponentDataの読み込み
		public static void ReadySharedComponentData()
		{
			//スプライトからメッシュの作成
			charaMeshMat = new MeshMatList();
			charaMeshMat.Init();
			const int DEKUNUM = 1;
			for (int i = 0; i < DEKUNUM; i++)
			{

				var res = charaMeshMat.Load("deku" + i.ToString("d2"), false, "Sprites/CharaSprite");
				// Debug.Log(res);
				if (res == 0)break;
			}

			geoMeshMat = new MeshMatList();
			geoMeshMat.Init();
			for (int i = 0; i < 1; i++)
			{
				var res = geoMeshMat.Load("tip" + i.ToString("d2"), true, "Sprites/GeoSprite");
				if (res == 0)break;
			}

			foodMeshMat = new MeshMatList();
			foodMeshMat.Init();
			foodMeshMat.Load("food", true, "Sprites/FoodSprite");

			//スクリプタブルオブジェクトの読み込み
			aniScriptSheet = new AniScriptSheet();
			if (Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().Length == 0)
				Debug.LogError("aniScriptSheet 0");
			aniScriptSheet.scripts = (Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().First()as AniScriptSheetObject).scripts;

			aniBasePos = new AniBasePos();
			if (Resources.FindObjectsOfTypeAll<AniBasePosObject>().Length == 0)
				Debug.LogError("aniBasePos 0");
			aniBasePos = (Resources.FindObjectsOfTypeAll<AniBasePosObject>().First()as AniBasePosObject).aniBasePos;
		}

	}
}