using Unity.Entities;
using Unity.Transforms;
namespace YYHS
{
    public static class ComponentTypes
    {
        public static ComponentType[] CharaComponentType = {
            typeof(CharaTag), // キャラタグ
            typeof(CharaId), // ID
            // typeof(PadScan), // 入力（CharaEntityFactoryで必要なキャラのみつける）
            typeof(ToukiMeter), // 闘気メーター
            // typeof(BgScroll), // 背景スクロール
        };

        public static ComponentType[] FilterEffectComponentType = {
            typeof(FilterEffect),
        };
    }
}
