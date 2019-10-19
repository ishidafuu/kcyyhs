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
            if (m_spriteSetter.GetAnimType() == EditAnimType.Action)
            {
                int charaNo = m_spriteSetter.GetCharaNo();
                ConvertAction(charaNo);
            }
            else
            {
                ConvertCommon();
            }
        }
        public void ConvertAll()
        {
            for (int i = 0; i < 100; i++)
            {
                bool isFound = ConvertAction(i);
                if (!isFound)
                    break;
            }

            ConvertCommon();
        }

        bool ConvertAction(int charaNo)
        {
            string[] animGuids = AssetDatabase.FindAssets("", new[] { $"Assets/GameAssets/Animations/Chara{charaNo.ToString("d2")}" });

            if (animGuids.Length == 0)
            {
                return false;
            }

            string createAssetPath = $"Assets/GameAssets/Resources/YHCharaAnim/yh_chara_anim{charaNo.ToString("d2")}.asset";
            Convert(animGuids, createAssetPath);

            return true;
        }

        void ConvertCommon()
        {
            string[] animGuids = AssetDatabase.FindAssets("", new[] { $"Assets/GameAssets/Animations/Common" });
            string createAssetPath = $"Assets/GameAssets/Resources/YHCharaAnim/yh_chara_anim_common.asset";
            Convert(animGuids, createAssetPath);
        }

        void Convert(string[] animGuids, string createAssetPath)
        {
            YHAnimationsObject outputObjects = ScriptableObject.CreateInstance<YHAnimationsObject>();
            List<string> destPaths = new List<string>();
            CreateByteFiles(destPaths, animGuids);
            CreateYHAnimation(destPaths, outputObjects);
            AssetDatabase.CreateAsset(outputObjects, createAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Convert Finished : {createAssetPath}");
        }

        private static void CreateByteFiles(List<string> destPaths, string[] animGuids)
        {

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

                // .bytesファイル削除（要素確認の際はここをコメントアウト）
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

            if (GUILayout.Button("ConvertAll"))
                (target as AnimationToJson).ConvertAll();

        }

    }
}