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
        [SerializeField] Animator m_animator;
        [SerializeField] GameObject m_character;
        [SerializeField] GameObject m_backGround;
        [SerializeField] GameObject m_effect;
        [SerializeField] GameObject m_camera;
        [SerializeField] GameObject m_spriteRC;
        [SerializeField] GameObject m_characterRC;
        [SerializeField] GameObject m_backGroundRC;
        [SerializeField] GameObject m_effectRC;
        [SerializeField] int m_charaNo;
        [SerializeField] int m_bgNo;

        List<GameObject> m_charaList = new List<GameObject>();
        List<GameObject> m_backgroundList = new List<GameObject>();
        List<GameObject> m_effectList = new List<GameObject>();

        public int GetCharaNo() => m_charaNo;

        private string GetCharaPath()
        {
            return string.Format(PathSettings.CharaSprite, m_charaNo.ToString("d2"));
        }

        private string GetBackGroundPath()
        {
            return string.Format(PathSettings.BackGroundSprite, m_bgNo.ToString("d2"));
        }

        public void InitObject()
        {
            FindAnimationController();
            FindObject();
            ClearList();
            DeleteOldObject(m_character);
            DeleteOldObject(m_backGround);
            DeleteOldObject(m_effect);
            CreateNewCharaObject();
            CreateNewBGObject();
            CreateNewEffectObject(GetCharaPath());
            CreateNewEffectObject(GetBackGroundPath());
            CreateNewEffectObject(PathSettings.CommonSprite);

            InitPosition();
            LoadObject();
            InactiveBGSprite();
            InactiveCharacterSprite();
            InactiveEffectSprite();
        }

        public void InactiveBGSprite()
        {
            foreach (var item in m_backgroundList)
            {
                item.SetActive(false);
            }
        }

        public void InactiveCharacterSprite()
        {
            foreach (var item in m_charaList)
            {
                item.SetActive(false);
            }
        }

        public void InactiveEffectSprite()
        {
            foreach (var item in m_effectList)
            {
                item.SetActive(false);
            }
        }

        public void LoadObject()
        {
            LoadSprite(GetCharaPath());
            LoadSprite(GetBackGroundPath());
        }

        private void ClearList()
        {
            m_backgroundList.Clear();
            m_effectList.Clear();
            m_charaList.Clear();
        }

        private void FindAnimationController()
        {
            m_animator = GetComponent<Animator>();
            string path = $"Assets/GameAssets/Animations/Chara{m_charaNo.ToString("d2")}/Controller.controller";
            RuntimeAnimatorController controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path) as RuntimeAnimatorController;
            m_animator.runtimeAnimatorController = controller;
        }

        private void FindObject()
        {
            m_character = GameObject.Find("Character");
            m_backGround = GameObject.Find("BackGround");
            m_effect = GameObject.Find("Effect");
            m_camera = GameObject.Find("Camera");
            m_spriteRC = GameObject.Find("SpriteRC");
            m_characterRC = GameObject.Find("CharacterRC");
            m_backGroundRC = GameObject.Find("BackGroundRC");
            m_effectRC = GameObject.Find("EffectRC");
        }

        private void InitPosition()
        {
            Vector3 BASE_POS = new Vector3(0, 48, 0);
            m_character.transform.localPosition = BASE_POS;
            m_backGround.transform.localPosition = BASE_POS;
            m_effect.transform.localPosition = BASE_POS;
        }

        private void CreateNewBGObject()
        {
            if (m_backGround == null)
            {
                m_backGround = Instantiate(m_backGroundRC);
                m_backGround.name = "BackGround";
                m_backGround.transform.SetParent(m_camera.transform);
            }

            string path = GetBackGroundPath();
            UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));
            foreach (var item in spriteList)
            {
                if (!CheckBG(item.name))
                    continue;

                if (CheckEffect(item.name))
                    continue;

                CreateSprite(item, m_backGround, m_backgroundList);
            }
        }

        private void CreateSprite(Object item, GameObject parentObj, List<GameObject> objectList)
        {
            var newSprite = Instantiate(m_spriteRC);
            newSprite.transform.SetParent(parentObj.transform);
            newSprite.name = item.name;
            objectList.Add(newSprite.gameObject);
        }

        private void CreateNewEffectObject(string path)
        {
            if (m_effect == null)
            {
                m_effect = Instantiate(m_effectRC);
                m_effect.name = "Effect";
                m_effect.transform.SetParent(m_camera.transform);
            }

            UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));
            foreach (var item in spriteList)
            {
                if (!CheckEffect(item.name))
                    continue;

                CreateSprite(item, m_effect, m_effectList);
            }
        }

        private void CreateNewCharaObject()
        {
            if (m_character == null)
            {
                m_character = Instantiate(m_characterRC);
                m_character.name = "Character";
                m_character.transform.SetParent(m_camera.transform);
            }

            string path = GetCharaPath();
            UnityEngine.Object[] spriteList = Resources.LoadAll(path, typeof(Sprite));
            foreach (var item in spriteList)
            {
                if (CheckEffect(item.name))
                    continue;

                CreateSprite(item, m_character, m_charaList);
            }
        }

        private void DeleteOldObject(GameObject baseObject)
        {
            if (baseObject != null)
            {
                DestroyImmediate(baseObject);
                baseObject = null;
            }

            // // 削除
            // var transforms = baseObject.GetComponentsInChildren<Transform>();
            // foreach (var item in transforms)
            // {
            //     if (item == baseObject.transform)
            //         continue;
            //     DestroyImmediate(item.gameObject);
            // }
        }

        public bool CheckEffect(string itemName)
        {
            return (itemName.IndexOf("effect") >= 0);
        }

        public bool CheckBG(string itemName)
        {
            return (itemName.IndexOf("bg") >= 0);
        }

        private void LoadSprite(string path)
        {
            UnityEngine.Object[] list = Resources.LoadAll(path, typeof(Sprite));

            // listを回してDictionaryに格納
            for (var i = 0; i < list.Length; ++i)
            {
                // Debug.Log(list[i].name);
                var sprite = list[i] as Sprite;

                var targetObj = GameObject.Find(sprite.name);
                if (targetObj == null)
                {
                    // Debug.LogError(sprite.name + "GameObject Not Found");
                    continue;
                }

                var targetSpriteRenderer = targetObj.GetComponent<SpriteRenderer>();

                if (targetSpriteRenderer == null)
                {
                    Debug.LogError(sprite.name + "SpriteRenderer Not Found");
                    continue;
                }

                targetSpriteRenderer.sprite = sprite;

                if (CheckBG(targetSpriteRenderer.name))
                {
                    targetSpriteRenderer.sprite = sprite;
                    targetSpriteRenderer.sortingOrder = -20;
                    targetSpriteRenderer.transform.localPosition = Vector3.zero;
                }
                else
                {
                    // 描画プライオリティ
                    if (targetSpriteRenderer.name.IndexOf("a_") >= 0)
                    {
                        targetSpriteRenderer.sortingOrder = +10;
                    }
                    else if (targetSpriteRenderer.name.IndexOf("b_") >= 0)
                    {
                        targetSpriteRenderer.sortingOrder = -10;
                    }
                }
                targetSpriteRenderer.transform.localPosition = Vector3.zero;

                targetObj.SetActive(false);
            }
        }


    }

    [CustomEditor(typeof(SpriteSetter))]
    public class SpriteSetterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Init Object"))
                (target as SpriteSetter).InitObject();

            if (GUILayout.Button("Inactive Character Sprite"))
                (target as SpriteSetter).InactiveCharacterSprite();
            if (GUILayout.Button("Inactive BG Sprite"))
                (target as SpriteSetter).InactiveBGSprite();
            if (GUILayout.Button("Inactive Effect Sprite"))
                (target as SpriteSetter).InactiveEffectSprite();

        }

    }
}