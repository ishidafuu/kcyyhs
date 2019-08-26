using YamlDotNet.Serialization;
using YamlDotNet.Samples.Helpers;
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
        [SerializeField] SpriteSetter m_spriteSetter;
        public void Convert()
        {
            int charaNo = m_spriteSetter.GetCharaNo();
            List<string> destPaths = new List<string>();
            YHAnimationsObject outputObjects = ScriptableObject.CreateInstance<YHAnimationsObject>();
            CreateByteFiles(destPaths, charaNo);
            CreateYHAnimation(destPaths, outputObjects);
            AssetDatabase.CreateAsset(outputObjects, $"Assets/GameAssets/ScriptableObjects/YHAnimation/YHAnim_{charaNo.ToString("d2")}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreateByteFiles(List<string> destPaths, int charaNo)
        {
            string[] animGuids = AssetDatabase.FindAssets("", new[] { $"Assets/GameAssets/Animations/Chara{charaNo.ToString("d2")}" });
            foreach (var guid in animGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.IndexOf(".anim") == -1)
                    continue;

                string destPath = path.Replace(".anim", ".bytes");
                // Debug.Log(destPath);
                System.IO.File.Copy(path, destPath, true);
                destPaths.Add(destPath);
            }
            AssetDatabase.Refresh();
        }

        private static void CreateYHAnimation(List<string> destPaths, YHAnimationsObject outputObjects)
        {
            foreach (var destPath in destPaths)
            {
                // text->yaml
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(destPath);
                int removeIndex = textAsset.text.IndexOf("AnimationClip:");
                string yamlText = textAsset.text.Remove(0, removeIndex);
                Deserializer deserializer = new DeserializerBuilder().Build();
                object yamlObject = deserializer.Deserialize(new StringReader(yamlText));

                // yaml->json
                Serializer serializer = new SerializerBuilder().JsonCompatible().Build();
                string json = serializer.Serialize(yamlObject);

                // json->rawAnim
                YHRawAnimation rawAnim = JsonUtility.FromJson<YHRawAnimation>(json);

                // rawAnim->yhAnim
                YHAnimation anim = YHRawAnimationConverter.Convert(rawAnim);

                outputObjects.animations.Add(anim);
                System.IO.File.Delete(destPath);
            }
        }

    }

    [CustomEditor(typeof(AnimationToJson))]
    public class AnimationToJsonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Convert"))
                (target as AnimationToJson).Convert();

        }

    }
}