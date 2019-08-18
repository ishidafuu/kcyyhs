﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace YYHS
{
    [ExecuteInEditMode]
    public class Define : SingletonMonoBehaviour<Define>
    {
        public CommonSettings Common;
        public MoveSettings Move;
        public DrawPosSettings DrawPos;
        public DebugSettings Debug;

        /// <summary>
        /// オートセット
        /// </summary>
        public void LoadObject()
        {
            Common = Resources.FindObjectsOfTypeAll<CommonSettings>().First() as CommonSettings;
            Move = Resources.FindObjectsOfTypeAll<MoveSettings>().First() as MoveSettings;
            Debug = Resources.FindObjectsOfTypeAll<DebugSettings>().First() as DebugSettings;
            DrawPos = Resources.FindObjectsOfTypeAll<DrawPosSettings>().First() as DrawPosSettings;
        }
    }
}