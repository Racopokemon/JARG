using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PalmWrapper : MoveWrapper
{
    public float moveStart, moveEnd, holdEnd;

    public override Driver CreateDriver()
    {
        return new PalmDriver(this);
    }
}
