using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct MeshMatList : IEquatable<MeshMatList>, ISharedComponentData
    {
        public Material material;
        public Dictionary<string, Mesh> meshDict;
        public Dictionary<string, Sprite> spriteDict;

        public MeshMatList(string path, string shader)
        {
            meshDict = new Dictionary<string, Mesh>();
            spriteDict = new Dictionary<string, Sprite>();
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
                // Debug.Log(list[i].name);
                var sprite = list[i] as Sprite;
                spriteDict.Add(list[i].name, sprite);

                var mesh = GenerateQuadMesh(sprite);

                meshDict.Add(list[i].name, mesh);
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

            // Debug.Log(sprite.name);
            // foreach (var item in sprite.vertices)
            // {
            // 	Debug.Log(item);
            // }
            // foreach (var item in sprite.uv)
            // {
            // 	Debug.Log(item);
            // }

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

        public bool Equals(MeshMatList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}