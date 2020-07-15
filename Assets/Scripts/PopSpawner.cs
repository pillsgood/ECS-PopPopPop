using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class PopSpawner : MonoBehaviour
{
    private struct PopSpawn_Job : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> spawnLocationArray;
        public EntityCommandBuffer.Concurrent commandBuffer;

        public void Execute(int index)
        {
            var entity = commandBuffer.Instantiate(index, PopEntityPrefab);
            commandBuffer.SetComponent(index, entity, new Translation()
            {
                Value = spawnLocationArray[index]
            });
        }
    }

    [Header("Prefabs")]
    public GameObject popPrefab;

    [Header("Spawner Properties")]
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 5;

    private Grid grid;
    private Camera mainCamera;
    private static Entity PopEntityPrefab { get; set; }
    private static EntityManager Manager { get; set; }

    private void Awake()
    {
        grid = GetComponent<Grid>();
        Manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        PopEntityPrefab =
            GameObjectConversionUtility.ConvertGameObjectHierarchy(popPrefab,
                GameObjectConversionSettings.FromWorld(
                    World.DefaultGameObjectInjectionWorld, null));
        mainCamera = Camera.main;
    }

    private void Start()
    {
        var count = height * width;
        Counter_UI.TotalCount = count;

        var spawnPositions =
            new NativeArray<float3>(count, Allocator.TempJob);


        for (int y = 0, i = 0; y < height; y++)
        for (int x = 0; x < width; x++, i++)
        {
            var gridPos = new Vector3Int(x, y, 0);
            var pos = grid.GetCellCenterWorld(gridPos);
            spawnPositions[i] = pos;
        }

        var commandBufferSystem = World.DefaultGameObjectInjectionWorld
            .GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        var handle = new PopSpawn_Job()
        {
            commandBuffer = commandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            spawnLocationArray = spawnPositions
        }.Schedule(count, width);

        commandBufferSystem.AddJobHandleForProducer(handle);

        var cameraZ = mainCamera.transform.position.z;
        var cameraPosition = Vector3.Lerp(spawnPositions.First(),
            spawnPositions.Last(), 0.5f);
        cameraPosition.z = cameraZ;
        mainCamera.transform.position = cameraPosition;

        handle.Complete();
        spawnPositions.Dispose();
    }
}