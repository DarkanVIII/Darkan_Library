using System;
using System.Collections.Generic;
using UnityEngine;

public class FpsCalculator
{
    public FpsCalculator(float updateRate = 0.5f)
    {
        UpdateRate = updateRate;
    }

    readonly List<float> _cachedFrameRates = new(32);
    float _timer;

    public float UpdateRate { get; private set; } = .5f;
    public event Action<int> OnUpdateFPS;

    public void Tick()
    {
        _timer += Time.deltaTime;
        _cachedFrameRates.Add(1 / Time.deltaTime);

        if (_timer >= UpdateRate)
        {
            float fpsTotal = 0;

            foreach (float fps in _cachedFrameRates)
            {
                fpsTotal += fps;
            }

            float averageFps = fpsTotal / _cachedFrameRates.Count;

            _cachedFrameRates.Clear();
            _timer = 0;

            OnUpdateFPS?.Invoke((int)averageFps);
        }
    }
}