using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/*
 * InitializeTimestamps() <- Add your timestamps here, in ascending order. You need to initialize the timestamps array first!
 * 
 * Call order: 
 * PrepareForSpawn()
 *        [UpdatePhase(a = 1, forCompletionOnly = true)
 *        [NextPhase()
 *        [ these two are looped for every phase completed in the meantime, after that you always get this one UpdatePhase call: 
 *      UpdatePhase(a \in [0, 1], forCompletionOnly = false) [once every frame]
 * 
 * DoCleanUp() is called once. When reaching the last timestamp, or already before. 
 */
public abstract class Driver
{
    protected float[] timestamps; //In seconds, dependent on the song. Sorted ascendingly after the InitializeTimestamps call. 
    private int phase = -1;

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

    public void Initialize(float beatLength)
    {
        InitializeTimestamps(beatLength);
        //System.Array.Sort(timestamps); <-- should be ok
    }

    //Returns true if this Driver can be deleted (was cleaned up)
    public bool Update(float timestamp)
    {
        if (cleanedUp) return true;
        while (timestamp >= timestamps[phase+1])
        {
            if (++phase >= timestamps.Length-1)
            {
                CleanUp();
                return true;
            } else
            {
                if (phase > 0) UpdatePhase(1, true);
                NextPhase();
            }
        }
        if (phase >= 0)
        {
            UpdatePhase(Mathf.InverseLerp(timestamps[phase], timestamps[phase+1], timestamp), false);
        }
        return false;
    }

    protected int GetPhase()
    {
        return phase;
    }

    // Instantiate (!) and fill Driver.timestamps. Add them in ascending order. 
    protected abstract void InitializeTimestamps(float beatLength);

    //Called before any NextPhase comes. Used to allocate storage etc a short time before this driver actually has its first event.
    //Once this is called, this driver is in the loop and receives updates with the timestamp
    protected abstract void PrepareForSpawn();

    //Called for every phase that was completed in the meantime.
    //Before, UpdatePhase is called forCompletionOnly with alpha value 1, so that the previous animations reach their endpoint before switching to the next phase. 
    //Also called before you get the first UpdatePhase call.
    //Afterwards, there always follows one UpdatePhase call. 
    //(So if we progress several phases, there come pairs of UpdatePhase (forCompletionOnly) and then NextPhase, and then UpdatePhase)
    //phase already contains the new phase value. 
    //When passing the last timestamp, CleanUp() is called instead, if it wasnt called before. 
    //GetPhase() will come in handy
    protected abstract void NextPhase();

    //a is a value between 0 and <1, depending on how far we are with our phase. 
    //To finish animations at a==1 exactly, this is also called with forCompletionOnly=true and a=1 before every NextPhase call. 
    //GetPhase() returns already the new, freshly entered (or skipped, who knows) phase
    protected abstract void UpdatePhase(float a, bool forCompletionOnly);

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

    //Helper method, changes the alpha channel of the color in the renderer
    public void SetAlpha(Renderer r, float alpha)
    {
        Color c = r.material.color;
        r.material.color = new Color(c.r, c.g, c.b, alpha);
    }

}
