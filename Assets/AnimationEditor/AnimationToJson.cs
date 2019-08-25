using YamlDotNet.Serialization;
using YamlDotNet.Samples.Helpers;
using UnityEngine;
using UnityEngine;
using UnityEditor;
using Unity.Collections;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace YYHS
{
    [ExecuteInEditMode]
    public class AnimationToJson : MonoBehaviour
    {
        public void Main()
        {
            string[] guids2 = AssetDatabase.FindAssets("Punch00_00", new[] { "Assets/GameAssets/Resources/Animations/Common" });
            Debug.Log(guids2);

            string path = AssetDatabase.GUIDToAssetPath(guids2[0]);
            string destpath = path.Replace(".anim", ".bytes");
            string assetpath = path.Replace(".anim", ".asset");
            System.IO.File.Copy(path, destpath, true);
            AssetDatabase.Refresh();
            TextAsset text_asset = AssetDatabase.LoadAssetAtPath<TextAsset>(destpath);
            int removeIndex = text_asset.text.IndexOf("AnimationClip:");
            string yamlText = text_asset.text.Remove(0, removeIndex);
            StringReader r = new StringReader(yamlText);
            Deserializer deserializer = new DeserializerBuilder().Build();
            object yamlObject = deserializer.Deserialize(new StringReader(yamlText));
            Serializer serializer = new SerializerBuilder().JsonCompatible().Build();
            string json = serializer.Serialize(yamlObject);
            Debug.Log(json);
            YHAnimation newAnim = JsonUtility.FromJson<YHAnimation>(json);
            YHAnimationsObject animObjects = new YHAnimationsObject();
            animObjects.animations.Add(newAnim);

            AssetDatabase.CreateAsset(animObjects, "Assets/GameAssets/Resources/ScriptableObjects/YHAnimation/YHAnimationsObject.asset");
            AssetDatabase.SaveAssets();


        }
    }

    [CustomEditor(typeof(AnimationToJson))]
    public class AnimationToJsonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Convert"))
                (target as AnimationToJson).Main();

            // if (GUILayout.Button("Inactive Character Sprite"))
            //     (target as SpriteSetter).InactiveCharacterSprite();
            // if (GUILayout.Button("Inactive BG Sprite"))
            //     (target as SpriteSetter).InactiveBGSprite();
            // if (GUILayout.Button("Inactive Effect Sprite"))
            //     (target as SpriteSetter).InactiveEffectSprite();

        }

    }
}