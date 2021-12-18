using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

//Subclass this with all public attributes this move needs, and create a driver from this in CreateDriver (you will probably just call new MyDriver(this)).
//Also, add the identification string for this move in MapManager in the static constructor here: (bit stretchy, but should work)
[Serializable]
public abstract class MoveWrapper
{
    public static Dictionary<string, ConstructorInfo> typeConstructorDictionary = new Dictionary<string, ConstructorInfo>();
    static MoveWrapper()
    {
        //Add your own empty constructor for your name and wrapper here: 
        typeConstructorDictionary.Add("palm", typeof(PalmWrapper).GetConstructor(new Type[] {}));
        typeConstructorDictionary.Add("text", typeof(TextWrapper).GetConstructors()[0]);
    }

    public abstract Driver CreateDriver();
}
