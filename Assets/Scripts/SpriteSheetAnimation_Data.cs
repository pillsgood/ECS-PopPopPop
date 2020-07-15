using Unity.Entities;

public struct SpriteSheetAnimation_Data : IComponentData
{
    public int CurrentFrame;
    public float FrameTimer;
    public int FrameCount;
    public float FrameTimerMax;
}