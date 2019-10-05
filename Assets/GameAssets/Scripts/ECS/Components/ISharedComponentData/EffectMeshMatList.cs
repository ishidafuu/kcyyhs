using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

namespace YYHS
{
    public class MeshMat
    {
        List<Material> m_materials;
        public Mesh m_mesh;

        public MeshMat()
        {
            m_materials = new List<Material>();
        }

        public void AddMaterial(Material material)
        {
            m_materials.Add(material);
        }

        public Material GetMaterial(int index = 0)
        {
            return m_materials[index];
        }
    }

    [Serializable]
    public struct EffectMeshMatList : IEquatable<EffectMeshMatList>, ISharedComponentData
    {
        public List<MeshMat> m_effectList;
        public List<MeshMat> m_screenFilterList;
        public List<MeshMat> m_bgFilterList;
        public List<MeshMat> m_gaugeList;

        public void AddEffect(Sprite sprite, string shaderName)
        {
            if (m_effectList == null)
            {
                m_effectList = new List<MeshMat>();
            }
            Add(m_effectList, sprite, shaderName, 1);
        }

        public void AddScreenFilter(Sprite sprite, string shaderName)
        {
            if (m_screenFilterList == null)
            {
                m_screenFilterList = new List<MeshMat>();
            }
            Add(m_screenFilterList, sprite, shaderName, 1);
        }

        public void AddBGFilter(Sprite sprite, string shaderName)
        {
            if (m_bgFilterList == null)
            {
                m_bgFilterList = new List<MeshMat>();
            }
            Add(m_bgFilterList, sprite, shaderName, 1);
        }

        public void AddGauge(Sprite sprite, string shaderName)
        {
            if (m_gaugeList == null)
            {
                m_gaugeList = new List<MeshMat>();
            }
            Add(m_gaugeList, sprite, shaderName, Settings.Instance.Common.PlayerCount);
        }

        void Add(List<MeshMat> list, Sprite sprite, string shaderName, int materialCount)
        {
            MeshMat newMeshMat = new MeshMat();


            // var sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;
            var shader = Shader.Find(shaderName);

            for (int i = 0; i < materialCount; i++)
            {
                newMeshMat.AddMaterial(new Material(shader));
            }

            if (shader == null)
            {
                Debug.LogError("(shader == null)" + shaderName);
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

            list.Add(newMeshMat);

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