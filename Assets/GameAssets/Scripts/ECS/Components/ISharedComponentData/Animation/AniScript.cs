using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;


[Serializable]
public struct AniScript : IEquatable<AniScript>, ISharedComponentData
{
    public string id;
    public List<AniFrame> frames;
    public void Setup(string id, List<AniFrame> AniFrames)
    {
        this.id = id;
        this.frames = AniFrames;
    }
    public static AniScript CreateAniScript(string id, List<AniFrame> AniFrames)
    {
        AniScript res = new AniScript();
        res.Setup(id, AniFrames);
        return res;
    }

    public bool Equals(AniScript obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return EqualityComparer<Transform>.Default.GetHashCode();
    }
}