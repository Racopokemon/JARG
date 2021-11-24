using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Call order: 
 * PrepareForSpawn()
 *      NextPhase() [several times until up to date, every call is a phase++]
 *      UpdatePhase() [once every frame]
 * 
 * DoCleanUp() is called once. When reaching the last timestamp, or already before. 
 */
public abstract class Driver
{
    protected float[] timestamps; //In seconds, dependent on the song. Expected to be sorted ascendingly
    protected int phase = -1;
    protected bool cleanedUp = false;

    public Driver()
    {
        
    }

    public float GetInstantiationPrewarmTime()
    {
        return timestamps[0] - 0.7f;
    }

    public bool InstantiateIfTime(float time)
    {
        if (time >= GetInstantiationPrewarmTime())
        {
            PrepareForSpawn();
            return true;
        } else
        {
            return false;
        }
    }

    //Returns true if this Driver can be deleted (was cleaned up)
    public bool Update(float timestamp)
    {
        if (cleanedUp) return true;
        while (timestamp >= timestamps[phase+1])
        {
            if (++phase >= timestamps.Length)
            {
                CleanUp();
                return true;
            } else
            {
                NextPhase();
            }
        }
        if (phase >= 0)
        {
            UpdatePhase(Mathf.InverseLerp(timestamps[phase], timestamps[phase+1], timestamp));
        }
        return false;
    }

    //Called before UpdatePhase, and for every raise by 1 in the phase. 
    //Also called before you get the first UpdatePhase call
    //(So if we progress several phases, this call comes several times, and then UpdatePhase)
    //phase already contains the new phase value. 
    //When passing the last timestamp, CleanUp() is called instead, if it wasnt called before. 
    protected abstract void NextPhase();

    //a is a value between 0 and <1, depending on how far we are with our phase. 
    protected abstract void UpdatePhase(float a);

    protected abstract void DoCleanUp();

    //If in doubt: Call this before you exit somewhere, it will clean up only 1 time anyway. 
    public void CleanUp()
    {
        if (!cleanedUp)
        {
            DoCleanUp();
            cleanedUp = true;
        }
    }

    //Called before any NextPhase comes. Used to allocate storage etc a short time before this driver actually has its first event.
    //Once this is called, this driver is in the loop and receives updates with the timestamp
    protected abstract void PrepareForSpawn();

    public void Initialize(float beatLength) {
        InitializeTimestamps(beatLength);
    }
    protected abstract void InitializeTimestamps(float beatLength);
}
