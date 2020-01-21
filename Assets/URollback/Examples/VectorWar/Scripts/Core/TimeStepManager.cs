using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// https://medium.com/@tglaiel/how-to-make-your-game-run-at-60fps-24c61210fe75
/// </summary>
public class TimeStepManager
{
    public delegate void UpdateAction(float dt);
    public event UpdateAction OnUpdate;

    protected float timestep = 60.0f;
    protected float timescale = 1.0f;

    protected float[] timestepClampTimes;
    protected float accumulator;

    protected bool resync;

    public TimeStepManager(float timestep, float timescale, float timestepUpperClamp, float timestepLowerClamp)
    {
        this.timestep = timestep;
        this.timescale = timescale;
        accumulator = 0;
        timestepClampTimes = new float[] { timestepUpperClamp, timestepLowerClamp };
        resync = false;
    }

    /// <summary>
    /// Should be called after swapping scenes or changing levels.
    /// </summary>
    public void Resync()
    {
        resync = true;
    }

    public void Clear()
    {
        accumulator = 0;
    }

    public void Update(float delta)
    {
        if (resync)
        {
            accumulator = 0;
            delta = timestep;
            resync = false;
        }

        if (Mathf.Abs(delta - timestepClampTimes[0]) < .0002)
        {
            delta = timestepClampTimes[0];
        }
        if (Mathf.Abs(delta - timestep) < .0002)
        {
            delta = timestep;
        }
        if (Mathf.Abs(delta - timestepClampTimes[1]) < .0002)
        {
            delta = timestepClampTimes[1];
        }

        accumulator += delta * timescale;

        while (accumulator >= timestep)
        {
            OnUpdate?.Invoke(timestep);
            accumulator -= timestep;
        }
    }

    public void ManualUpdate()
    {
        OnUpdate?.Invoke(timestep);
    }
}
