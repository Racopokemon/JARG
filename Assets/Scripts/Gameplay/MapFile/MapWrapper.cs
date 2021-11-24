using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct MapWrapper
{
    [Serializable]
    public struct SingleMoveWrapper
    {
        public string type;
        public string json; //Woah this is bad design. By design, from here on. But, I mean, who cares, by now this is not even a prototype and this is just oriented around the json library in unity. 
    }

    public string songFilename;
    public float BPM;
    public SingleMoveWrapper[] moves;
}
