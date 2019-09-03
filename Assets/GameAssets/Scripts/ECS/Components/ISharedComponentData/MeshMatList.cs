using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    [Serializable]
    public struct MeshMatList : IEquatable<MeshMatList>, ISharedComponentData
    {
        public Dictionary<string, Material> materialDict;
        public Dictionary<string, Mesh> meshDict;
        int colorPropertyId;

        public MeshMatList(string path, string shader)
        {
            meshDict = new Dictionary<string, Mesh>();
            materialDict = new Dictionary<string, Material>();
            // spriteDict = new Dictionary<string, Sprite>();
            UnityEngine.Object[] list = Resources.LoadAll(path, typeof(Sprite));

            // listがnullまたは空ならエラーで返す
            if (list == null || list.Length == 0)
            {
                Debug.LogWarning(path);
            }

            // マテリアル用シェーダー
            var matShader = Shader.Find(shader);
            colorPropertyId = Shader.PropertyToID("_Color");

            if (matShader == null)
            {
                Debug.LogWarning(shader);
            }

            // listを回してDictionaryに格納
            for (var i = 0; i < list.Length; ++i)
            {
                // Debug.LogWarning(list[i].name);
                var sprite = list[i] as Sprite;
                var mesh = GenerateQuadMesh(sprite);
                var material = new Material(matShader);
                material.mainTexture = sprite.texture;

                meshDict.Add(list[i].name, mesh);
                materialDict.Add(list[i].name, material);

            }
        }

        Mesh GenerateQuadMesh(Sprite sprite)
        {
            List<Vector3> _vertices = new List<Vector3>();
            for (int i = 0; i < sprite.uv.Length; i++)
            {
                _vertices.Add(new Vector3(sprite.vertices[i].x, 0, sprite.vertices[i].y));
            }

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
                vertices = _vertices.ToArray(),
                uv = sprite.uv,
                triangles = triangles
            };
        }

        public Material SetColor(string imageName, Color color)
        {
            Material mat = materialDict[imageName];
            mat.SetColor(colorPropertyId, color);
            return mat;
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