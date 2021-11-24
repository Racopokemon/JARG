using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmDriver : Driver
{
    protected PalmWrapper w;
    public PalmDriver(PalmWrapper wrapper)
    {
        w = wrapper; 
    }

    protected override void DoCleanUp()
    {
        throw new System.NotImplementedException();
    }

    protected override void InitializeTimestamps(float beatLength)
    {
        throw new System.NotImplementedException();
    }

    protected override void NextPhase()
    {
        throw new System.NotImplementedException();
    }

    protected override void PrepareForSpawn()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdatePhase(float a)
    {
        throw new System.NotImplementedException();
    }
}
