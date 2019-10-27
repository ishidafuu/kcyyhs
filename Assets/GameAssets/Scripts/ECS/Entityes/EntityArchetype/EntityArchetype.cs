using Unity.Entities;
using Unity.Transforms;
namespace YYHS
{
    public static class ComponentTypes
    {
        public static ComponentType[] CharaComponentType = {
            typeof(SideInfo),
            typeof(ToukiMeter),
            typeof(Status),
            typeof(JumpState),
            typeof(DamageState),
            typeof(ReiState),
            typeof(DownState),
            // typeof(PadScan), // （CharaEntityFactoryで必要なキャラのみつける）
        };

        public static ComponentType[] FilterEffectComponentType = {
            typeof(FilterEffect),
        };

        public static ComponentType[] BattleComponentType = {
            typeof(BattleSequencer),
            typeof(ReiPool),
        };

        public static ComponentType[] ReiPieceComponentType = {
            typeof(ReiPiece),
        };
    }
}
