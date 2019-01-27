//using UnityEngine;
//using UnityEngine.Experimental.PlayerLoop;
//using Unity.Entities;
//using Unity.Transforms2D;
//using Unity.Collections;
//using Unity.Jobs;

//namespace NKKD {

//	//各パーツの描画位置決定および描画
//	[UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
//	public class GeoDrawSystem : ComponentSystem {

//		struct Group {
//			public int Length;
//			public EntityArray entities;
//			[ReadOnly] public ComponentDataArray<GeoObj> obj;
//			[ReadOnly] public SharedComponentDataArray<GeoMeshMat> meshMat;
//		}
//		[Inject] Group group;

//		protected override void OnUpdate() {
//			for (int i = 0; i < group.Length; i++) {
//				for (int i2 = 0; i2 < group.meshMat[i].matrix.Length; i2++) {
//					Graphics.DrawMesh(group.meshMat[i].mesh,
//						group.meshMat[i].matrix[i2],
//						group.meshMat[i].material, 0);

//					//Graphics.DrawMeshInstanced(group.meshMat[i].mesh, 0,
//					//	group.meshMat[i].material,
//					//	group.meshMat[i].matrix.ToArray(),
//					//	group.meshMat[i].matrix.Length);
//				}

//			}
//		}
//	}
//}