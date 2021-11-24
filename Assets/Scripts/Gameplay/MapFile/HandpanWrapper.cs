using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmWrapper : MoveWrapper
{
    public float moveStart, moveEnd, holdEnd;

    public override Driver CreateDriver()
    {
        return new PalmDriver(this);
    }
}
