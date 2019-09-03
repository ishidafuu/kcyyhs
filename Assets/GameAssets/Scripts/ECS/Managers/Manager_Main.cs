using UnityEngine.U2D;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.Entities;
using System.Collections.Generic;

namespace YYHS
{
    sealed class Manager_Main : MonoBehaviour
    {
        const string SCENE_NAME = "Main";

        List<Entity> m_playerEntityList = new List<Entity>();

        [SerializeField]
        PixelPerfectCamera m_pixelPerfectCamera;

        void Start()
        {
            Settings.Instance.SetPixelSize(Screen.width / m_pixelPerfectCamera.refResolutionX);
            var scene = SceneManager.GetActiveScene();
            if (scene.name != SCENE_NAME)
                return;

            var manager = InitializeWorld();

            ReadySharedComponentData();
            ComponentCache();
            InitializeEntities(manager);
        }

        void OnDestroy()
        {
            World.DisposeAllWorlds();
            WordStorage.Instance.Dispose();
            WordStorage.Instance = null;
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(null);
        }

        EntityManager InitializeWorld()
        {
            World[] worlds = new World[1];
            ref World world = ref worlds[0];
            world = new World(SCENE_NAME);
            World.Active = world;

            InitializationSystemGroup initializationSystemGroup = world.GetOrCreateSystem<InitializationSystemGroup>();


            SimulationSystemGroup simulationSystemGroup = world.GetOrCreateSystem<SimulationSystemGroup>();
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PadScanSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ToukiMeterInputSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<CharaInputSystem>());

            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ToukiMeterCountSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FilterEffectCountSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BattleSequencerCountSystem>());

            PresentationSystemGroup presentationSystemGroup = world.GetOrCreateSystem<PresentationSystemGroup>();
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BGDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FilterEffectDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MeterDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BattleDrawDrawSystem>());


            initializationSystemGroup.SortSystemUpdateList();
            simulationSystemGroup.SortSystemUpdateList();
            presentationSystemGroup.SortSystemUpdateList();

            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            return world.EntityManager;
        }

        void ComponentCache()
        {
            Cache.pixelPerfectCamera = FindObjectOfType<PixelPerfectCamera>();
        }

        void ReadySharedComponentData()
        {
            Shared.ReadySharedComponentData();
        }

        void InitializeEntities(EntityManager manager)
        {
            CreateCharaEntity(manager);
            CreateFilterEffectEntity(manager);
            CreateBattleEntity(manager);
            // manager.SetSharedComponentData(entity, _meshMatList);
        }

        void CreateCharaEntity(EntityManager manager)
        {
            for (int i = 0; i < Settings.Instance.Common.CharaCount; i++)
            {
                var playerEntity = (i < m_playerEntityList.Count)
                    ? m_playerEntityList[i]
                    : Entity.Null;

                var entity = CharaEntityFactory.CreateEntity(i, manager);
            }
        }

        void CreateFilterEffectEntity(EntityManager manager)
        {
            for (int i = 0; i < Settings.Instance.Common.FilterEffectCount; i++)
            {
                var entity = FilterEffectEntityFactory.CreateEntity(i, manager, ref Shared.yhFilterEffectList);
            }
        }

        void CreateBattleEntity(EntityManager manager)
        {
            var entity = BattleEntityFactory.CreateEntity(manager, ref Shared.yhFilterEffectList);
        }
    }
}