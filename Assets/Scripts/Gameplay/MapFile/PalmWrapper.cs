using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PalmWrapper : MoveWrapper
{
    public float moveStart, moveEnd, holdEnd;
    public float x, y, z; //Once this is fleshed out, ill probably limit the positions (as its done for the blocks in BSaber) as this is cancer ofc. 
    public float angle;
    public bool right; 

    public override Driver CreateDriver()
    {
        return new PalmDriver(this);
    }
}
