using MaterialProperty;
using Unity.Entities;

[UpdateAfter(typeof(InputSystem))]
public class SpriteSheetAnimation_System : SystemBase
{
    public EndSimulationEntityCommandBufferSystem CommandBufferSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        CommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;
        var commandBuffer = CommandBufferSystem.CreateCommandBuffer();
        var handle = Entities.WithAll<PopTag>().ForEach((Entity entity, ref SpriteSheetAnimation_Data data, ref
            PopMaterialProp_CurrentFrame currentFrame) =>
        {
            if (data.CurrentFrame >= data.FrameCount)
            {
                return;
            }

            currentFrame.Value = data.CurrentFrame;
            data.FrameTimer += deltaTime;

            while (data.FrameTimer >= data.FrameTimerMax)
            {
                data.FrameTimer -= data.FrameTimerMax;
                data.CurrentFrame++;
            }
        }).Schedule(Dependency);

        CommandBufferSystem.AddJobHandleForProducer(handle);
    }
}