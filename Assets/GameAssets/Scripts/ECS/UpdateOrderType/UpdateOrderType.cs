using Unity.Entities;
using Unity.Transforms;
namespace YYHS
{

    [UpdateBefore(typeof(CountGroup))]
    public class ScanGroup : ComponentSystemGroup { }

    [UpdateAfter(typeof(ScanGroup))]
    public class CountGroup : ComponentSystemGroup { }

    [UpdateAfter(typeof(CountGroup))]
    public class JudgeGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class RenderGroup : ComponentSystemGroup { }

}