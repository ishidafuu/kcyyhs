// using Unity.Collections;
// using Unity.Entities;
// using Unity.Jobs;
// using Unity.Transforms;
// using UnityEngine;
// using UnityEngine.Experimental.PlayerLoop;

// namespace NKKD
// {

// 	//各パーツの描画位置決定および描画
// 	[UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
// 	public class FoodDrawSystem : ComponentSystem
// 	{

// 		struct Group
// 		{
// 			public int Length;
// 			public EntityArray entities;
// 			[ReadOnly] public ComponentDataArray<FoodObj> obj;
// 			[ReadOnly] public ComponentDataArray<Position> position;
// 			[ReadOnly] public ComponentDataArray<FoodData> data;
// 			[ReadOnly] public SharedComponentDataArray<MeshMatList> meshMat;
// 		}

// 		[Inject] Group group;

// 		//カリング
// 		[ComputeJobOptimization]
// 		struct JobCulling : IJobParallelFor
// 		{
// 			public NativeArray<int> isInCamera;
// 			[ReadOnly] public float cameraXMin;
// 			[ReadOnly] public float cameraXMax;
// 			[ReadOnly] public float cameraYMin;
// 			[ReadOnly] public float cameraYMax;
// 			[ReadOnly] public NativeArray<Position> position;
// 			[ReadOnly] public int entitiesLength;
// 			public void Execute(int i)
// 			{

// 				isInCamera[i] = 1;
// 				//if (cameraXMax < position[i].Value.x) {
// 				//	isInCamera[i] = 0;
// 				//}
// 				//else if (cameraXMin > position[i].Value.x) {
// 				//	isInCamera[i] = 0;
// 				//}
// 				//else if (cameraYMax < position[i].Value.y) {
// 				//	isInCamera[i] = 0;
// 				//}
// 				//else if (cameraYMin > position[i].Value.y) {
// 				//	isInCamera[i] = 0;
// 				//}
// 				//else {
// 				//	isInCamera[i] = 1;
// 				//}
// 			}
// 		}

// 		//触角頭位置
// 		[ComputeJobOptimization]
// 		struct JobFood : IJob
// 		{
// 			public NativeArray<Matrix4x4> matrix;
// 			public NativeArray<int> matrixLength;
// 			public NativeArray<int> foodType;
// 			[ReadOnly] public Quaternion q;
// 			[ReadOnly] public NativeArray<int> isInCamera;
// 			[ReadOnly] public NativeArray<Position> position;
// 			[ReadOnly] public NativeArray<FoodData> data;
// 			[ReadOnly] public int entitiesLength;
// 			[ReadOnly] public Vector3 one;
// 			public void Execute()
// 			{
// 				int matIndex = 0;
// 				for (int i = 0; i < entitiesLength; i++)
// 				{

// 					//if (isInCamera[i] == 0) continue;

// 					Matrix4x4 tmpAntMatrix = Matrix4x4.TRS(new Vector3(position[i].Value.x,
// 							position[i].Value.y,
// 							position[i].Value.z),
// 						q, one);

// 					matrix[matIndex] = tmpAntMatrix;
// 					foodType[matIndex] = data[i].foodType;
// 					matIndex++;
// 				}
// 				matrixLength[0] = matIndex;
// 			}
// 		}

// 		protected override void OnUpdate()
// 		{
// 			//Debug.Log(group.entities.Length);

// 			NativeArray<int> isInCamera = new NativeArray<int>(group.Length, Allocator.Temp);
// 			var position = new NativeArray<Position>(group.Length, Allocator.Temp);
// 			group.position.CopyTo(position);
// 			var data = new NativeArray<FoodData>(group.Length, Allocator.Temp);
// 			group.data.CopyTo(data);

// 			var q = Quaternion.Euler(new Vector3(-90, 0, 0));

// 			float cameraW = Cache.pixelPerfectCamera.refResolutionX >> 1;
// 			float cameraH = (cameraW * 0.75f);
// 			var jobCulling = new JobCulling()
// 			{
// 				isInCamera = isInCamera,
// 					position = position,
// 					cameraXMax = Camera.main.transform.position.x + cameraW,
// 					cameraXMin = Camera.main.transform.position.x - cameraW,
// 					cameraYMax = Camera.main.transform.position.y + cameraH,
// 					cameraYMin = Camera.main.transform.position.y - cameraH,
// 					entitiesLength = group.entities.Length,
// 			};

// 			var jobHandleCulling = jobCulling.Schedule(group.Length, 64);
// 			jobHandleCulling.Complete();

// 			var jobFood = new JobFood()
// 			{
// 				matrix = new NativeArray<Matrix4x4>(group.Length, Allocator.Temp),
// 					foodType = new NativeArray<int>(group.Length, Allocator.Temp),
// 					matrixLength = new NativeArray<int>(1, Allocator.Temp),
// 					isInCamera = isInCamera,
// 					q = q,
// 					position = position,
// 					data = data,
// 					entitiesLength = group.entities.Length,
// 					one = Vector3.one,
// 			};
// 			var jobFoodHandle = jobFood.Schedule();
// 			jobFoodHandle.Complete();

// 			//for (int i = 0; i < group.entities.Length; i++) {
// 			//	Debug.Log(group.data[i].foodType);
// 			//}
// 			//Debug.Log(jobFood.matrixLength[0]);
// 			for (int i = 0; i < jobFood.matrixLength[0]; i++)
// 			{
// 				//Debug.Log(jobFood.foodType[i]);
// 				Graphics.DrawMesh(
// 					group.meshMat[0].meshs[jobFood.foodType[i]],
// 					jobFood.matrix[i],
// 					group.meshMat[0].materials[jobFood.foodType[i]], 0);
// 			}

// 			jobFood.matrixLength.Dispose();
// 			jobFood.matrix.Dispose();
// 			jobFood.foodType.Dispose();

// 			position.Dispose();
// 			data.Dispose();
// 			isInCamera.Dispose();

// 		}
// 	}
// }