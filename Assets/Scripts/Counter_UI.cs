using System;
using TMPro;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class Counter_UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentCountText;
    [SerializeField] private TextMeshProUGUI totalCountText;

    private static Counter_UI Instance { get; set; }

    private int _currentCount = 0;
    private int _totalCount = 0;

    public static int CurrentCount
    {
        get => Instance._currentCount;
        set
        {
            Instance._currentCount = value;
            Instance.currentCountText.text = value.ToString();
        }
    }
    public static int TotalCount
    {
        get => Instance._totalCount;
        set
        {
            Instance._totalCount = value;
            Instance.totalCountText.text = value.ToString();
        }
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }

        Instance = this;
    }

    private void Start()
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InputSystem>().OnBurstHandler += (sender, args) =>
        {
            CurrentCount++;
        };
    }
}