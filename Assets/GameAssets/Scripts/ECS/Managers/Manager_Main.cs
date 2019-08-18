using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
// using Unity.Rendering;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;

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
            simulationSystemGroup.AddSystemToUpdateList(world.GetOrCreateSystem<BGDrawSystem>());

            initializationSystemGroup.SortSystemUpdateList();
            simulationSystemGroup.SortSystemUpdateList();
            presentationSystemGroup.SortSystemUpdateList();

            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            return world.EntityManager;
        }


        // 各コンポーネントのキャッシュ
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
        }

        void CreateCharaEntity(EntityManager manager)
        {
            for (int i = 0; i < Define.Instance.Common.CharaNum; i++)
            {
                var playerEntity = (i < m_playerEntityList.Count)
                    ? m_playerEntityList[i]
                    : Entity.Null;

                var entity = CharaEntityFactory.CreateEntity(i, manager, ref Shared.charaMeshMat, ref Shared.aniScriptSheet, ref Shared.aniBasePos);
            }
        }

    }
}