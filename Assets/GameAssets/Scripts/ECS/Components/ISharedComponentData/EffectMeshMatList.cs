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

        private static readonly int LoadCount = 100;
        public void LoadEffectBG(Sprite sprite)
        {
            m_effectBGList = new List<MeshMat>();
            for (int i = 0; i < LoadCount; i++)
            {
                string shaderName = string.Format(PathSettings.EffectBGShader, i.ToString("d2"));
                if (!Load(m_effectBGList, sprite, shaderName, 1))
                {
                    break;
                }
            }
        }

        public void LoadEffectScreen(Sprite sprite)
        {
            m_effectScreenList = new List<MeshMat>();
            for (int i = 0; i < LoadCount; i++)
            {
                string shaderName = string.Format(PathSettings.EffectScreenShader, i.ToString("d2"));
                if (!Load(m_effectScreenList, sprite, shaderName, 1))
                {
                    break;
                }
            }
        }

        public void AddEffectSide(Sprite sprite, int materialCount)
        {
            m_effectSideList = new List<MeshMat>();
            for (int i = 0; i < LoadCount; i++)
            {
                string shaderName = string.Format(PathSettings.EffectSideShader, i.ToString("d2"));
                if (!Load(m_effectSideList, sprite, shaderName, materialCount))
                {
                    break;
                }
            }
        }

        public void LoadEffectLarge(Sprite sprite)
        {
            m_effectLargeList = new List<MeshMat>();
            for (int i = 0; i < LoadCount; i++)
            {
                string shaderName = string.Format(PathSettings.EffectLargeShader, i.ToString("d2"));
                if (!Load(m_effectLargeList, sprite, shaderName, 1))
                {
                    break;
                }
            }
        }

        public void LoadEffectMedium(Sprite sprite)
        {
            m_effectMediumList = new List<MeshMat>();
            for (int i = 0; i < LoadCount; i++)
            {
                string shaderName = string.Format(PathSettings.EffectMediumShader, i.ToString("d2"));
                if (!Load(m_effectMediumList, sprite, shaderName, 1))
                {
                    break;
                }
            }
        }

        public void LoadEffectSmall(Sprite sprite)
        {
            m_effectSmallList = new List<MeshMat>();
            for (int i = 0; i < LoadCount; i++)
            {
                string shaderName = string.Format(PathSettings.EffectSmallShader, i.ToString("d2"));
                if (!Load(m_effectSmallList, sprite, shaderName, 1))
                {
                    break;
                }
            }
        }

        public void LoadFilterScreen(Sprite sprite)
        {
            m_filterScreenList = new List<MeshMat>();
            for (int i = 0; i < LoadCount; i++)
            {
                string shaderName = string.Format(PathSettings.FillterScreenShader, i.ToString("d2"));
                if (!Load(m_filterScreenList, sprite, shaderName, 1))
                {
                    break;
                }
            }
        }

        public void LoadFrameParts(Sprite sprite, EnumFrameParts objNo, int materialCount)
        {
            if (m_framePartsList == null)
            {
                m_framePartsList = new List<MeshMat>();
            }

            string shaderName = string.Format(PathSettings.FramePartsShader, ((int)objNo).ToString("d2"));

            Load(m_framePartsList, sprite, shaderName, materialCount);
        }

        bool Load(List<MeshMat> list, Sprite sprite, string shaderName, int materialCount)
        {
            MeshMat newMeshMat = new MeshMat();

            var shader = Shader.Find(shaderName);

            if (shader == null)
            {
                return false;
            }

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
            return true;
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