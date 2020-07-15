using UnityEngine;

[RequireComponent(typeof(PopSpawner))]
public class PopAnimation : MonoBehaviour
{
    private static readonly int FrameCountShaderProp = Shader.PropertyToID("_FrameCount");

    [SerializeField] private float popBurstAnimationSpeed = 0.1f;
    private int frameCount;

    public static SpriteSheetAnimation_Data SpriteSheetAnimationData;

    private void Start()
    {
        var material = GetComponent<PopSpawner>().popPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        frameCount = (int) material.GetFloat(FrameCountShaderProp);
    }

    private void Update()
    {
        SpriteSheetAnimationData = new SpriteSheetAnimation_Data()
        {
            CurrentFrame = 0,
            FrameTimer = 0,
            FrameCount = frameCount,
            FrameTimerMax = popBurstAnimationSpeed
        };
    }
}