﻿using UnityEngine.U2D;
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
            Settings.Instance.Debug.InitCount();
            Settings.Instance.SetPixelSize(Screen.width / m_pixelPerfectCamera.refResolutionX);
            var scene = SceneManager.GetActiveScene();
            if (scene.name != SCENE_NAME)
                return;

            var manager = InitializeWorld();

            ReadySharedComponentData();
            ComponentCache();
            InitializeEntities(manager);

            SoundManager.Instance.PlayBgm(UnityEngine.Random.Range(0, 4));
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
            ScanGroup scanGroup = world.GetOrCreateSystem<ScanGroup>();
            simulationSystemGroup.AddSystemToUpdateList(scanGroup);
            scanGroup.AddSystemToUpdateList(world.GetOrCreateSystem<PadScanSystem>());
            scanGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ToukiMeterInputSystem>());
            scanGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ActionInputSystem>());
            CountGroup countGroup = world.GetOrCreateSystem<CountGroup>();
            simulationSystemGroup.AddSystemToUpdateList(countGroup);
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ToukiMeterCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ReiPieceCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BattleSequencerCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FilterEffectCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<JumpCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DownCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DamageCountSystem>());
            countGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ReiCountSystem>());

            PresentationSystemGroup presentationSystemGroup = world.GetOrCreateSystem<PresentationSystemGroup>();
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<SpritDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FilterEffectDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<ReiPieceDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<FrameDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BattleDrawDrawSystem>());
            presentationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<DebugViewerSystem>());

            initializationSystemGroup.SortSystemUpdateList();
            simulationSystemGroup.SortSystemUpdateList();
            presentationSystemGroup.SortSystemUpdateList();

            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            return world.EntityManager;
        }

        void ComponentCache()
        {
            Cache.m_pixelPerfectCamera = FindObjectOfType<PixelPerfectCamera>();
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
            CreateReiPieceEntity(manager);
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
                var entity = FilterEffectEntityFactory.CreateEntity(i, manager);
            }
        }

        void CreateReiPieceEntity(EntityManager manager)
        {
            for (int i = 0; i < Settings.Instance.Common.ReiPieceCount; i++)
            {
                var entity = ReiPieceEntityFactory.CreateEntity(i, manager);
            }
        }

        void CreateBattleEntity(EntityManager manager)
        {
            var entity = BattleEntityFactory.CreateEntity(manager);
        }
    }
}