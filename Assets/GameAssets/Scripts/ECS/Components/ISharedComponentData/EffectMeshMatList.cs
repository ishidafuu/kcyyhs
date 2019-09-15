using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    public struct MeshMat
    {
        public Material m_material;
        public Mesh m_mesh;
        int m_colorPropertyId;
    }

    [Serializable]
    public struct EffectMeshMatList : IEquatable<EffectMeshMatList>, ISharedComponentData
    {
        public List<MeshMat> m_meshMatList;
        // int m_colorPropertyId;

        public void Add(string spritePath, string matPath, string shaderName)
        {
            if (m_meshMatList == null)
            {
                m_meshMatList = new List<MeshMat>();
            }

            MeshMat newMeshMat = new MeshMat();

            var sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;
            var shader = Shader.Find(shaderName);
            newMeshMat.m_material = Resources.Load(matPath, typeof(Material)) as Material;
            // m_colorPropertyId = Shader.PropertyToID("_Color");
            if (sprite == null)
            {
                Debug.LogError(spritePath);
            }
            if (newMeshMat.m_material == null)
            {
                Debug.LogError(matPath);
            }
            if (shader == null)
            {
                Debug.LogError(shaderName);
            }

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

            newMeshMat.m_mesh = new Mesh
            {
                vertices = _vertices.ToArray(),
                uv = sprite.uv,
                triangles = triangles
            };

            m_meshMatList.Add(newMeshMat);

        }

        public bool Equals(EffectMeshMatList obj)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<Transform>.Default.GetHashCode();
        }
    }
}