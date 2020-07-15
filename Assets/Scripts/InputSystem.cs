using System;
using MaterialProperty;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[AlwaysUpdateSystem]
public class InputSystem : SystemBase
{
    public event EventHandler OnBurstHandler;

    public struct OnBurstEvent
    {
    }

    private NativeQueue<OnBurstEvent> eventQueue;

    private static Camera MainCamera { get; set; }
    private static Vector3 MouseWorldPosition
    {
        get
        {
            var position = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            return position;
        }
    }

    private Vector3 inputPosition;
    private BeginSimulationEntityCommandBufferSystem _beginSimulationEntityCommandBufferSystem;
    private EndSimulationEntityCommandBufferSystem _endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _beginSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        _endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        eventQueue = new NativeQueue<OnBurstEvent>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        eventQueue.Dispose();
    }

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        MainCamera = Camera.main;
    }


    protected override void OnUpdate()
    {
        var position = MouseWorldPosition;
        var click = Input.GetMouseButtonDown(0);
        var beginCommandBuffer =
            _beginSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var endCommandBuffer = _endSimulationEntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
        var spriteSheetAnimationData = PopAnimation.SpriteSheetAnimationData;
        var eventQueueParallel = eventQueue.AsParallelWriter();
        var handle = Entities.WithAll<PopTag>().WithNone<SpriteSheetAnimation_Data>().ForEach(
            (Entity entity, ref WorldRenderBounds bounds, ref PopMaterialProp_Highlight highlight) =>
            {
                var contains = bounds.Value.Contains(position);
                if (click && contains)
                {
                    eventQueueParallel.Enqueue(new OnBurstEvent());
                    beginCommandBuffer.AddComponent(entity.Index, entity, spriteSheetAnimationData);
                    endCommandBuffer.AddComponent<PopAudio.PopAudio_Data>(entity.Index, entity);
                }

                highlight.Value = contains && !click;
            }
        ).ScheduleParallel(Dependency);

        _beginSimulationEntityCommandBufferSystem.AddJobHandleForProducer(handle);
        _endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(handle);

        while (eventQueue.TryDequeue(out var @event))
        {
            OnBurstHandler?.Invoke(this, EventArgs.Empty);
        }
    }
}