using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PopAudio : MonoBehaviour
{
    [SerializeField] private AudioClip[] popClips;
    [SerializeField] private int popOnNthFrame = 1;
    private int clipCount;
    private AudioClip _nextClip;


    private static PopAudio Instance { get; set; }

    public struct PopAudio_Data : IComponentData
    {
    }

    private class PopAudio_System : SystemBase
    {
        protected override void OnUpdate()
        {
            var popOnFrame = Instance.popOnNthFrame;

            Entities.WithStructuralChanges().WithAll<PopAudio_Data>()
                .ForEach((Entity entity, ref Translation translation, ref SpriteSheetAnimation_Data data) =>
                {
                    if (data.CurrentFrame == popOnFrame)
                    {
                        PlayAt(translation.Value);
                        EntityManager.RemoveComponent<PopAudio_Data>(entity);
                    }
                }).Run();
        }
    }


    private void Start()
    {
        Instance = this;
        clipCount = popClips.Length;
    }

    private AudioClip RandomPopClip => popClips[Random.Range(0, clipCount)];


    private AudioClip NextClip
    {
        get
        {
            if (!_nextClip)
            {
                _nextClip = Instance.RandomPopClip;
            }

            var clip = _nextClip;
            _nextClip = Instance.RandomPopClip;
            return clip;
        }
    }

    private static void PlayAt(Vector3 position)
    {
        var clip = Instance.NextClip;
        AudioSource.PlayClipAtPoint(clip, position);
    }
}