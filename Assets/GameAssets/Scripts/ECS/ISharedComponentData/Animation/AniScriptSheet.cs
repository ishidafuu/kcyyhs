using System;
using System.Collections.Generic;
using Unity.Entities;

namespace NKKD
{
    [Serializable]
    public struct AniScriptSheet : ISharedComponentData
    {
        //各モーションごとのアニメーション情報
        public List<AniScript> scripts;
    }
}