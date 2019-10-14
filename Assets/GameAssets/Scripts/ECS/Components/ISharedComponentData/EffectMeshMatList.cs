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

        public void AddMaterial(Material material, int number)
        {
            material.SetInt("_No", number);
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
        public List<MeshMat> m_effectBGList;
        public List<MeshMat> m_effectScreenList;
        public List<MeshMat> m_effectSideList;
        public List<MeshMat> m_effectLargeList;
        public List<MeshMat> m_effectMediumList;
        public List<MeshMat> m_effectSmallList;
        public List<MeshMat> m_filterScreenList;
        public List<MeshMat> m_framePartsList;

        public void AddEffectBG(Sprite sprite, string shaderName)
        {
            if (m_effectBGList == null)
            {
                m_effectBGList = new List<MeshMat>();
            }
            Add(m_effectBGList, sprite, shaderName, 1);
        }

        public void AddEffectScreen(Sprite sprite, string shaderName)
        {
            if (m_effectScreenList == null)
            {
                m_effectScreenList = new List<MeshMat>();
            }
            Add(m_effectScreenList, sprite, shaderName, 1);
        }

        public void AddEffectSide(Sprite sprite, string shaderName, int materialCount)
        {
            if (m_effectSideList == null)
            {
                m_effectSideList = new List<MeshMat>();
            }
            Add(m_effectSideList, sprite, shaderName, 1);
        }

        public void AddEffectLarge(Sprite sprite, string shaderName)
        {
            if (m_effectLargeList == null)
            {
                m_effectLargeList = new List<MeshMat>();
            }
            Add(m_effectLargeList, sprite, shaderName, 1);
        }

        public void AddEffectMedium(Sprite sprite, string shaderName)
        {
            if (m_effectMediumList == null)
            {
                m_effectMediumList = new List<MeshMat>();
            }
            Add(m_effectMediumList, sprite, shaderName, 1);
        }

        public void AddEffectSmall(Sprite sprite, string shaderName)
        {
            if (m_effectSmallList == null)
            {
                m_effectSmallList = new List<MeshMat>();
            }
            Add(m_effectSmallList, sprite, shaderName, 1);
        }

        public void AddFilterScreen(Sprite sprite, string shaderName)
        {
            if (m_filterScreenList == null)
            {
                m_filterScreenList = new List<MeshMat>();
            }
            Add(m_filterScreenList, sprite, shaderName, 1);
        }

        public void AddFrameParts(Sprite sprite, string shaderName, int materialCount)
        {
            if (m_framePartsList == null)
            {
                m_framePartsList = new List<MeshMat>();
            }
            Add(m_framePartsList, sprite, shaderName, materialCount);
        }

        void Add(List<MeshMat> list, Sprite sprite, string shaderName, int materialCount)
        {
            MeshMat newMeshMat = new MeshMat();


            // var sprite = Resources.Load(spritePath, typeof(Sprite)) as Sprite;
            var shader = Shader.Find(shaderName);

            for (int i = 0; i < materialCount; i++)
            {
                newMeshMat.AddMaterial(new Material(shader), i);
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