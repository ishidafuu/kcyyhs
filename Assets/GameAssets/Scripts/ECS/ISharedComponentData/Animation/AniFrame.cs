using System;
using Unity.Entities;
using UnityEngine;

namespace NKKD
{
    /// <summary>
    /// 実際にゲームから呼ばれる、エディタから出力後のデータ
    /// 実際の座標などが入る
    /// </summary>
    [Serializable]
    public struct AniFrame : ISharedComponentData
    {
        public Vector2Int pos;
        public int angle;
        public int face;
    }
}