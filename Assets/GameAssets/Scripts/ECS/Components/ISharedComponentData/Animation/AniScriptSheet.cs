using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct AniScriptSheet : IEquatable<AniScriptSheet>, ISharedComponentData
{
    // 各モーションごとのアニメーション情報
    public List<AniScript> scripts;

    public bool Equals(AniScriptSheet obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return EqualityComparer<Transform>.Default.GetHashCode();
    }
}