using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

//Subclass this with all public attributes this move needs, and create a driver from this in CreateDriver (you will probably just call new MyDriver(this)).
//Also, add the type of your wrapper in the file into the typeDictionary. 
[Serializable]
public abstract class MoveWrapper
{
    public static Dictionary<string, Type> typeDictionary = new Dictionary<string, Type>();
    static MoveWrapper()
    {
        //Add your own empty constructor for your name and wrapper here: 
        typeDictionary.Add("palm", typeof(PalmWrapper));
        typeDictionary.Add("text", typeof(TextWrapper));
    }

    public abstract Driver CreateDriver();
}
