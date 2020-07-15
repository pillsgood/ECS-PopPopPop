using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public class MeshCulling_System : SystemBase
{
    private Camera mainCamera;
    private EndSimulationEntityCommandBufferSystem _commandBufferSystem;

    private struct CameraBounds : IComponentData
    {
        public AABB Value;

        public CameraBounds(Camera camera)
        {
            var aspect = camera.aspect;
            var orthographicSizeSize = camera.orthographicSize;
            Value.Center = camera.transform.position;
            Value.Extents = new float3(orthographicSizeSize * aspect, orthographicSizeSize, 5f) * 1.25f;
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        mainCamera = Camera.main;
        _commandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var commandBuffer = _commandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var bounds = new CameraBounds(Camera.main);

        var renderHandle = Entities.WithAll<DisableRendering>().ForEach(
            (Entity entity, in Translation translation) =>
            {
                if (bounds.Value.Contains(translation.Value))
                {
                    commandBuffer.RemoveComponent<DisableRendering>(entity.Index, entity);
                }
            }).ScheduleParallel(Dependency);
        var cullHandle = Entities.WithNone<DisableRendering>().WithAll<RenderMesh>().ForEach(
            (Entity entity, in Translation translation) =>
            {
                if (!bounds.Value.Contains(translation.Value))
                {
                    commandBuffer.AddComponent<DisableRendering>(entity.Index, entity);
                }
            }).ScheduleParallel(Dependency);


        _commandBufferSystem.AddJobHandleForProducer(renderHandle);
        _commandBufferSystem.AddJobHandleForProducer(cullHandle);
    }
}