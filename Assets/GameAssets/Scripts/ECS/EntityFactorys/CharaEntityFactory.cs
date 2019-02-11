using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
//using Unity.Transforms2D;
using Unity.Collections;
//using toinfiniityandbeyond.Rendering2D;
using System.Collections.Generic;
using System.Linq;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.SceneManagement;
namespace NKKD
{
	public static class CharaEntityFactory
	{

		/// <summary>
		/// キャラエンティティ作成
		/// </summary>
		/// <param name="i"></param>
		/// <param name="entityManager"></param>
		/// <param name="ariMeshMat"></param>
		/// <param name="aniScriptSheet"></param>
		/// <param name="aniBasePos"></param>
		/// <returns></returns>
		public static Entity CreateEntity(int _i, EntityManager _entityManager,
			ref MeshMatList _meshMatList,
			ref AniScriptSheet _aniScriptSheet,
			ref AniBasePos _aniBasePos
		)
		{
			var archetype = _entityManager.CreateArchetype(ComponentTypes.CharaComponentType);

			var entity = _entityManager.CreateEntity(archetype);

			//ComponentDataのセット
			var posL = 0;
			//Define.Instance.GetMapSize() / 2;
			var posH = 0;
			//Define.Instance.GetMapSize() / 2;

			// //Tag
			// entityManager.SetComponentData(entity, new CharaTag);

			//必要なキャラのみインプットをつける
			if (_i < Define.Instance.Common.PlayerNum)
			{
				_entityManager.AddComponent(entity, ComponentType.Create<PadInput>());
			}

			//ID
			_entityManager.SetComponentData(entity, new CharaId
			{
				familyId = 0,
					myId = _i,
			});

			// //位置
			// _entityManager.SetComponentData(entity, new Position
			// {
			// 	Value = new float3(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0)
			// });

			//位置
			_entityManager.SetComponentData(entity, new CharaMove
			{
				position = new Vector3Int(UnityEngine.Random.Range(posL, posH), UnityEngine.Random.Range(posL, posH), 0),
					delta = Vector3Int.zero
			});

			//モーション
			_entityManager.SetComponentData(entity, new CharaMotion
			{

			});

			//位置
			_entityManager.SetComponentData(entity, new CharaMuki
			{
				muki = EnumMuki.Right,
					// dashMuki = EnumMuki.None,
			});

			// //行動
			// _entityManager.SetComponentData(entity, new CharaBehave
			// {
			// 	behaveType = 0,
			// 		angle = (int)UnityEngine.Random.Range(0, 11),
			// 		endTime = (Time.realtimeSinceStartup + 0.5f + UnityEngine.Random.value)
			// });

			//見た目
			_entityManager.SetComponentData(entity, new CharaLook
			{
				isLeft = 0,
					isBack = 0
			});

			//SharedComponentDataのセット
			_entityManager.AddSharedComponentData(entity, _meshMatList);
			_entityManager.AddSharedComponentData(entity, _aniScriptSheet);
			_entityManager.AddSharedComponentData(entity, _aniBasePos);

			return entity;
		}
	}
}