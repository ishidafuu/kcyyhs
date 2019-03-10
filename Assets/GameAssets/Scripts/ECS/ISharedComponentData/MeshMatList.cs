	using System.Collections.Generic;
	using System;
	using Unity.Entities;
	using UnityEngine;

	[Serializable]
	public struct MeshMatList : ISharedComponentData
	{
		public Material material;
		public Dictionary<string, Mesh> meshs;

		public MeshMatList(string path, string shader)
		{
			meshs = new Dictionary<string, Mesh>();
			UnityEngine.Object[] list = Resources.LoadAll(path, typeof(Sprite));

			// listがnullまたは空ならエラーで返す
			if (list == null || list.Length == 0)
			{
				Debug.LogWarning(path);
			}
			// マテリアル用シェーダー
			var matShader = Shader.Find(shader);
			material = new Material(matShader);

			if (matShader == null)
			{
				Debug.LogWarning(shader);
			}

			// listを回してDictionaryに格納
			for (var i = 0; i < list.Length; ++i)
			{
				Debug.Log(list[i].name);
				var sprite = list[i] as Sprite;
				var mesh = GenerateQuadMesh(sprite);
				meshs.Add(list[i].name, mesh);
				if (i == 0)
				{
					material.mainTexture = sprite.texture;
				}
			}
		}

		Mesh GenerateQuadMesh(Sprite sprite)
		{
			Vector3[] _vertices = {
				new Vector3(sprite.vertices[0].x, 0, sprite.vertices[0].y),
				new Vector3(sprite.vertices[1].x, 0, sprite.vertices[1].y),
				new Vector3(sprite.vertices[2].x, 0, sprite.vertices[2].y),
				new Vector3(sprite.vertices[3].x, 0, sprite.vertices[3].y)
			};

			int[] triangles = {
				sprite.triangles[0],
				sprite.triangles[1],
				sprite.triangles[2],
				sprite.triangles[3],
				sprite.triangles[4],
				sprite.triangles[5]
			};

			return new Mesh
			{
				vertices = _vertices,
					uv = sprite.uv,
					triangles = triangles
			};
		}
	}