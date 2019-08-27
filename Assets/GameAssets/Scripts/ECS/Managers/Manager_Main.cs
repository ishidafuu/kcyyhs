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
            PresentationSystemGroup presentationSystemGroup = world.GetOrCreateSystem<PresentationSystemGroup>();


            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PadScanSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ToukiMeterInputJobSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ToukiMeterCountJobSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FilterEffectCountJobSystem>());

            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BGDrawSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FilterEffectDrawSystem>());
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<MeterDrawSystem>());

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
    }
}