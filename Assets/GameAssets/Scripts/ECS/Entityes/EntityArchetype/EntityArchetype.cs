using Unity.Entities;
using Unity.Transforms;
namespace YYHS
{
    public static class ComponentTypes
    {
        public static ComponentType[] CharaComponentType = {
            typeof(SideInfo), // キャラ番号など
            typeof(ToukiMeter), // 闘気メーター
            // typeof(PadScan), // 入力（CharaEntityFactoryで必要なキャラのみつける）
        };

        public static ComponentType[] FilterEffectComponentType = {
            typeof(FilterEffect),
        };

        public static ComponentType[] BattleComponentType = {
            typeof(BattleSequencer),
        };
    }
}
