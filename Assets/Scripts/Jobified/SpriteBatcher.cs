using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace vadersb.utils.unity.jobs
{
	public class SpriteBatcher<T> where T: struct, IRenderable
	{
		public const int BatchCountVertex = 256;
		public const int BatchCountIndex = 512;
		
		private static readonly int VertexDataSize = Marshal.SizeOf<RenderableJobsUtils.VertexData>();
		private static readonly int QuadVertexDataSize = Marshal.SizeOf<RenderableJobsUtils.QuadVertexData>();

		private static readonly int IndexDataSize = sizeof(uint);
		private static readonly int QuadIndexDataSize = Marshal.SizeOf<RenderableJobsUtils.QuadIndexData>();
		
		
		private readonly Mesh m_Mesh;

		private NativeArray<RenderableJobsUtils.QuadVertexData> m_VerticesArray;
		private NativeArray<RenderableJobsUtils.QuadIndexData> m_IndicesArray;

		private JobHandle m_VertexJobHandle;
		private JobHandle m_IndexJobHandle;

		private bool m_IsBatching;
		

		public SpriteBatcher(Mesh mesh)
		{
			Debug.Assert(VertexDataSize * 4 == QuadVertexDataSize);
			Debug.Assert(IndexDataSize * 6 == QuadIndexDataSize);

			Debug.Log("Vertex data size: " + VertexDataSize);
			Debug.Log("Quad vertex data size: " + QuadVertexDataSize);

			Debug.Log("Index data size: " + IndexDataSize);
			Debug.Log("Quad index data size: " + QuadIndexDataSize);
			
			Debug.Assert(mesh != null);
			Debug.Assert(mesh.indexFormat == IndexFormat.UInt32);

			m_Mesh = mesh;
			//m_Mesh = new Mesh();
			//m_Mesh.indexFormat = IndexFormat.UInt32;
			//m_Mesh.MarkDynamic();

			m_IsBatching = false;
		}


		public void BatchStart(NativeArray<T> items, int itemsCount, NativeArray<SpriteData> sprites, JobHandle jobToWaitFor, int batchCountVertex = BatchCountVertex, int batchCountIndex = BatchCountIndex)
		{
			Debug.Assert(m_IsBatching == false);
			Debug.Assert(itemsCount <= items.Length);
			Debug.Assert(itemsCount >= 0);
			
			int verticesCount = itemsCount * 4;
			int indicesCount = itemsCount * 6;
			
			//arrays allocation
			m_VerticesArray = new NativeArray<RenderableJobsUtils.QuadVertexData>(itemsCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			m_IndicesArray = new NativeArray<RenderableJobsUtils.QuadIndexData>(itemsCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
			
			//vertices job
			var vertexJob = new VertexJob(items, sprites, m_VerticesArray);
			m_VertexJobHandle = vertexJob.Schedule(itemsCount, batchCountVertex, jobToWaitFor);
			
			//indices job
			var indexJob = new IndexJob(m_IndicesArray);
			m_IndexJobHandle = indexJob.Schedule(itemsCount, batchCountIndex, jobToWaitFor);

			m_IsBatching = true;
		}


		public void BatchFinalize()
		{
			if (m_IsBatching == false)
			{
				return;
			}

			m_Mesh.Clear();
			
			//vertex buffer
			m_VertexJobHandle.Complete();

			int verticesCount = m_VerticesArray.Length * 4;

			var verticesArray = m_VerticesArray.Reinterpret<RenderableJobsUtils.VertexData>(QuadVertexDataSize);
			
			m_Mesh.SetVertexBufferParams(verticesCount, RenderableJobsUtils.VertexLayout);
			
			//todo maybe add some flags
			m_Mesh.SetVertexBufferData(verticesArray, 0, 0, verticesCount);

			
			//index buffer
			m_IndexJobHandle.Complete();

			int indicesCount = m_IndicesArray.Length * 6;

			var indicesArray = m_IndicesArray.Reinterpret<uint>(QuadIndexDataSize);
			
			m_Mesh.SetIndexBufferParams(indicesCount, IndexFormat.UInt32);
			m_Mesh.SetIndexBufferData(indicesArray, 0, 0, indicesCount);
			
			

			
			// SubMesh definition
			var meshDesc = new SubMeshDescriptor(0, indicesCount, MeshTopology.Triangles);
			m_Mesh.SetSubMesh(0, meshDesc);
			
			
			//arrays dispose
			m_VerticesArray.Dispose();
			m_IndicesArray.Dispose();
			
			
			//finally
			m_IsBatching = false;
		}


		public void Dispose()
		{
			if (m_IsBatching)
			{
				BatchFinalize();
			}
		}
		
		//------------------------------------------------------------------------------
		//JOBS
		[BurstCompile]
		struct VertexJob : IJobParallelFor
		{
			[ReadOnly]
			private NativeArray<T> m_Items;

			[ReadOnly]
			private NativeArray<SpriteData> m_Sprites;
			
			[WriteOnly]
			private NativeArray<RenderableJobsUtils.QuadVertexData> m_Output;


			public VertexJob(NativeArray<T> items, NativeArray<SpriteData> sprites, NativeArray<RenderableJobsUtils.QuadVertexData> output)
			{
				m_Items = items;
				m_Sprites = sprites;
				m_Output = output;
			}


			public void Execute(int index)
			{
				var item = m_Items[index];

				//vertex array indices
				// int indexV0 = index * 4;
				// int indexV1 = indexV0 + 1;
				// int indexV2 = indexV1 + 1;
				// int indexV3 = indexV2 + 1;

				float2 v0;
				float2 v1;
				float2 v2;
				float2 v3;

				Color32 color;
				
				float2 t0;
				float2 t1;
				float2 t2;
				float2 t3;
				
				if (item.IsVisible() == true)
				{
					var sprite = m_Sprites[item.GetSpriteIndex()];
					
					//position
					var worldPosition = item.GetPosition();
					var scale = item.GetScale();
					var rotationAngle = item.GetRotationAngle();
					var rotationAngleSin = math.sin(rotationAngle);
					var rotationAngleCos = math.cos(rotationAngle);
					

					v0 = RenderableJobsUtils.CalculatePosition(sprite.v0, worldPosition, scale, rotationAngleSin, rotationAngleCos);
					v1 = RenderableJobsUtils.CalculatePosition(sprite.v1, worldPosition, scale, rotationAngleSin, rotationAngleCos);
					v2 = RenderableJobsUtils.CalculatePosition(sprite.v2, worldPosition, scale, rotationAngleSin, rotationAngleCos);
					v3 = RenderableJobsUtils.CalculatePosition(sprite.v3, worldPosition, scale, rotationAngleSin, rotationAngleCos);

					//color
					color = item.GetColor();

					//texture coords
					t0 = sprite.t0;
					t1 = sprite.t1;
					t2 = sprite.t2;
					t3 = sprite.t3;
				}
				else
				{
					//position
					v0 = item.GetPosition();
					v1 = v0;
					v2 = v0;
					v3 = v0;
					
					//color
					color = new Color32(0, 0, 0, 0);
					
					//texture coords
					t0 = new Vector2(0.0f, 0.0f);
					t1 = t0;
					t2 = t0;
					t3 = t0;					
				}
				
				//output
				m_Output[index] = new RenderableJobsUtils.QuadVertexData(
					new RenderableJobsUtils.VertexData(new Vector3(v0.x, v0.y, 0.0f), color, t0),
				    new RenderableJobsUtils.VertexData(new Vector3(v1.x, v1.y, 0.0f), color, t1),
					new RenderableJobsUtils.VertexData(new Vector3(v2.x, v2.y, 0.0f), color, t2),
					new RenderableJobsUtils.VertexData(new Vector3(v3.x, v3.y, 0.0f), color, t3));
			}
		}

		[BurstCompile]
		struct IndexJob : IJobParallelFor
		{
			[WriteOnly]
			private NativeArray<RenderableJobsUtils.QuadIndexData> m_Output;


			public IndexJob(NativeArray<RenderableJobsUtils.QuadIndexData> output)
			{
				m_Output = output;
			}


			public void Execute(int index)
			{
				//vertex array indices
				uint vertex0 = (uint)index * 4;
				uint vertex1 = vertex0 + 1;
				uint vertex2 = vertex1 + 1;
				uint vertex3 = vertex2 + 1;
				
				//index array indices
				// int index0 = index * 6;
				// int index1 = index0 + 1;
				// int index2 = index1 + 1;
				// int index3 = index2 + 1;
				// int index4 = index3 + 1;
				// int index5 = index4 + 1;
				
				//0 1 2
				//2 1 3

				// m_Output[index0] = vertex0;
				// m_Output[index1] = vertex1;
				// m_Output[index2] = vertex2;
				//
				// m_Output[index3] = vertex2;
				// m_Output[index4] = vertex1;
				// m_Output[index5] = vertex3;

				m_Output[index] = new RenderableJobsUtils.QuadIndexData(vertex0, vertex1, vertex2, vertex2, vertex1, vertex3);
				
			}
		}
	}

}