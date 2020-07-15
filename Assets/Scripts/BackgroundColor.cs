using System;
using System.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = UnityEngine.Random;

public class BackgroundColor : MonoBehaviour
{
    [SerializeField] private TextAsset hexColorAsset;
    [SerializeField] private Color[] colors;

    [SerializeField] private float speedMultiplier = 1;


    private Camera mainCamera;
    private JobHandle colorJobHandle;

    private Color startColor;

    private bool CoroutineActive;

    private IEnumerator ChangeColor_Coroutine(Color current, Color target)
    {
        CoroutineActive = true;
        var t = 0f;
        while (t <= 1f)
        {
            current = Color.Lerp(startColor, target, t);
            t += Time.deltaTime * speedMultiplier;
            SetColor(current);
            yield return new WaitForEndOfFrame();
        }

        current = startColor = target;
        SetColor(current);
        CoroutineActive = false;
        yield return ChangeColor_Coroutine(startColor, NextColor);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        startColor = RandomColor;
    }


    private void Update()
    {
        StartCoroutine(ChangeColor_Coroutine(startColor, NextColor));
        enabled = false;
    }

    private Color RandomColor => colors[Random.Range(0, colors.Length)];

    private Color _nextColor = Color.black;
    private Color NextColor
    {
        get
        {
            while (_nextColor == Color.black || _nextColor == startColor)
            {
                _nextColor = RandomColor;
            }


            var color = _nextColor;
            _nextColor = RandomColor;
            return color;
        }
    }


    public void SetColor(Color color)
    {
        mainCamera.backgroundColor = color;
    }

    public void ImportColors()
    {
        var lines = hexColorAsset.text.Split('\n');
        colors = new Color[lines.Length];

        for (var index = 0; index < lines.Length; index++)
        {
            var line = lines[index];
            var hex = "#" + line.Trim();
            if (ColorUtility.TryParseHtmlString(hex, out var color))
            {
                colors[index] = color;
            }
        }
    }
}