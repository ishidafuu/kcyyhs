using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
sealed class Manager_MovingCubes : MonoBehaviour
{
    [SerializeField] MeshInstanceRenderer[] renderers;

    void Start()
    {
        World.Active = new World("move cube");
        manager = World.Active.CreateManager<EntityManager>();

        // World.CreateManager(Type t, params object[] p) とWorld.CreateManager<T>(params object[] p) と
        // 二種類を使い分けていますが、 後者は前者の単なるラッパーです。 
        // T型のScriptBehaviourManagerを欲しいのでなければ非ジェネリック版を使った方が微小なれどもパフォーマンスによいです。
        World.Active.CreateManager(typeof(EndFrameTransformSystem));
        World.Active.CreateManager<MeshInstanceRendererSystem>().ActiveCamera = GetComponent<Camera>();
        World.Active.CreateManager(typeof(MoveSystem));
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.Active);

        archetype = manager.CreateArchetype(ComponentType.Create<Position>(), ComponentType.Create<Velocity>(), ComponentType.Create<MeshInstanceRenderer>());

        var src = manager.CreateEntity(archetype);
        renderers[0].material.enableInstancing = true;
        // EntityにIComponentDataやISharedComponentDataを同期的に設定する場合、
        //  EntityManager.SetComponentData(Entity, T) やSetSharedComponentData(Entity, T) を使用します。
        // IComponentDataに書き込むのはローコストですが、 ISharedComponentDataに値を設定するのはかなり高コストです。
        // 故に、 ECSはPrototypeデザインパターンを採用しています。

        // 具体的にはEntityManager.Instantiate(Entity, NativeArray) メソッドですね。
        // これは第一引数のEntityのComponentData全てをコピーしたEntityを新たに作成し、 第二引数に詰めるというものです。
        // EntityManager.CreateEntityを何万回と繰り返して個別にSetSharedComponentDataするより
        // 遥かにメモリ効率もCPU効率も優れています。 ISharedComponentDataを扱う際はなるべくこれを使いましょう。
        manager.SetSharedComponentData(src, renderers[0]);
        Set(src);
        using(var _ = new NativeArray<Entity>(11450, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
        {
            manager.Instantiate(src, _);
            for (int i = 0; i < _.Length; i++)
                Set(_[i]);
        }
    }

    EntityManager manager;
    EntityArchetype archetype;

    private void Set( in Entity e)
    {
        manager.SetComponentData(e, new Position { Value = new Unity.Mathematics.float3((Random.value - 0.5f) * 40, (Random.value - 0.5f) * 40, (Random.value - 0.5f) * 40) });
        manager.SetComponentData(e, new Velocity { Value = new Unity.Mathematics.float3((Random.value - 0.5f), (Random.value - 0.5f), (Random.value - 0.5f)) });
    }
}