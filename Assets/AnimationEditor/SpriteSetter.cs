using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace YYHS
{
	[ExecuteInEditMode]
	public class SpriteSetter : MonoBehaviour
	{

		public int CharaNo;

		public GameObject Base;
		public GameObject SpriteRC;

		public void InitObject()
		{
			Base = GameObject.Find("Base");
			SpriteRC = GameObject.Find("SpriteRC");

			// 削除
			var transforms = Base.GetComponentsInChildren<Transform>();
			foreach (var item in transforms)
			{
				if (item == Base.transform)
					continue;
				DestroyImmediate(item.gameObject);
			}

			// 生成
			string path = GetCharaPath();
			Debug.Log(path);
			UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));
			foreach (var item in spriteList)
			{
				var newSprite = Instantiate(SpriteRC);
				newSprite.transform.SetParent(Base.transform);
				newSprite.name = item.name;
			}
		}

		public void LoadObject()
		{
			// オートセット
			// aniScriptSheet = Resources.FindObjectsOfTypeAll<AniScriptSheetObject>().First()as AniScriptSheetObject;
			// aniBasePos = Resources.FindObjectsOfTypeAll<AniBasePosObject>().First()as AniBasePosObject;
			// Texture2D tex2d = Resources.Load("Textures/AssetName")as Texture2D;

			// var sprites = new Dictionary<string, Sprite>();
			string path = GetCharaPath();
			UnityEngine.Object[] list = Resources.LoadAll(path, typeof(Sprite));

			// listを回してDictionaryに格納
			for (var i = 0; i < list.Length; ++i)
			{
				Debug.Log(list[i].name);
				var sprite = list[i] as Sprite;

				var targetObj = GameObject.Find(sprite.name);
				if (targetObj == null)
				{
					Debug.LogError(sprite.name + "GameObject Not Found");

					continue;
				}

				var targetSpriteRenderer = targetObj.GetComponent<SpriteRenderer>();

				if (targetSpriteRenderer == null)
				{
					Debug.LogError(sprite.name + "SpriteRenderer Not Found");
					continue;
				}
				targetSpriteRenderer.sprite = sprite;

				if (targetSpriteRenderer.name.IndexOf("a_") >= 0)
				{
					targetSpriteRenderer.sortingOrder = +10;
				}
				else if (targetSpriteRenderer.name.IndexOf("b_") >= 0)
				{
					targetSpriteRenderer.sortingOrder = -10;
				}
			}
		}

		private string GetCharaPath()
		{
			return "Sprites/Character/chara" + CharaNo.ToString("d2");
		}
	}

	[CustomEditor(typeof(SpriteSetter))] // 拡張するクラスを指定
	public class SpriteSetterEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			// 元のInspector部分を表示
			base.OnInspectorGUI();

			// ボタンを表示

			if (GUILayout.Button("InitObject"))
				(target as SpriteSetter).InitObject();

			if (GUILayout.Button("LoadObject"))
				(target as SpriteSetter).LoadObject();

		}

	}
}