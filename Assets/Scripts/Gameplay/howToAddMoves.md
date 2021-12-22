# How to add new moves

## Creating the class files
Let XXX be your new move name. 
* The JSON structure is contained in MapFile/XXXWrapper.cs 
** This is a subclass of MapFile/MoveWrapper.cs
** It only describes the variables, and creates a driver for itself, probably just giving itself as info into the constructor
* The driver does the work, translates numbers into events, creates elements in the game, is then enqueued and asked for start timestamps and receives calls once this start stamp is reached
** Create your XXXDriver as subclass of Driver.cs
* Add the name of your move in the JSON (tag 'type') to the dictionary in the static part of MapFile/MoveWrapper.cs

## How to fill your JSON
Yeah, we instantly fill strings into the JSON, that can again be decompiled as JSON, the cancer begins right here (but I mean, we could easily change it if we needed to)
tag names: 
* type: XXX (or whatever the string is you set in the dictionary in the MoveWrapper)
* json: Here goes your string. 

## How to code the driver
* For the constructor, how about you just accept your wrapper as only argument? Just store it as variable w (its fine, we do it in all drivers) and access your data directly. 
* First line: using UnityEngine;
* Implement InitializeTimestamps: there is a timestamp-array (its actually an array and not a list, when I wrote this i was apparently really performance hungry), you initialize it and add timestamps in that the next phase of your move begins. You get a beat length as argument, so you can work with that. 
* Implement PrepareForSpawn(): Called first, half a second or so before the first timestamp. The idea is to do all the Instantiate calls here already, silently preload prefabs and stuff and make everything ready, when time does not yet matter. [Does this make sense...? Well, idk]
** In case you load exactly one GameObject, its okay to call it g (I decide that!)
** g = (GameObject) Object.Instantiate(Resources.Load("PrefabName", typeof(GameObject))); This loads a prefab from a ressources folder and instantiates it. You will call that here. 
** You probably want to disable the GO for now. 
* Implement NextPhase(): Its called for every timestamp that is passed, separately, and before UpdatePhase. Use GetPhase(). You dont need to finish animations here, UpdatePhase with a=1 is called before it.  
* Implement UpdatePhase(). Hopefully you just do some interpolation. a is between 0 (start timestamp) and 1 (target timestamp). As said, its also called before every new phase. Use GetPhase().
* Implement DoCleanup(). Can be called everytime, if PrepareForSpawn was already called. GetPhase did not change. Remove your clones and models. 

Right now the most simple example is the text "move" (its no move at all), you may take looks there. 

*Enjoy!*