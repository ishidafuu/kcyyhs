using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace YYHS
{
    [ExecuteInEditMode]
    public class PartsParmater : MonoBehaviour
    {
        public bool m_IsBrink = false;
    }

    [CustomEditor(typeof(PartsParmater))]
    public class PartsParmaterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // if (GUILayout.Button("Init Object"))
            //     (target as SpriteSetter).InitObject();

            // if (GUILayout.Button("Inactive Character Sprite"))
            //     (target as SpriteSetter).InactiveCharacterSprite();
            // if (GUILayout.Button("Inactive BG Sprite"))
            //     (target as SpriteSetter).InactiveBGSprite();
            // if (GUILayout.Button("Inactive Effect Sprite"))
            //     (target as SpriteSetter).InactiveEffectSprite();

        }

    }
}