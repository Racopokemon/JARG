using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

/*
 * Actually this is rather the DriverManager. Creates them, updates them, keeps track of them. 
 * 
 * Usage: 
 *   Give the path of the level file, just any active MonoBehaviour for a coroutine, and the audioSource that is used for playback (the clip gets automatically assigned)
 *   Call HasLoaded repeatedly until it returns true, then youre ready to go: 
 *   Call Play() to play the map. Make sure to call Update every frame while playing. (Just always forward your Update call is probably the easiest way) 
 *   You can (and this is neat to slightly reduce loading times when debugging maps) Reset() the manager while playing, 
 *     which stops the playback and removes all moves etc. Additionally, the map is reset so that you can just play from the beginning again with Play(). 
 *   
 *   If you just want to stop playback and get rid of this MapManager, call CleanUp(). 
 */
public class MapManager
{
    protected MoveWrapper[] allWrappers;

    protected List<Driver> drivers = new List<Driver>();
    protected List<Driver> waitingDrivers = new List<Driver>(); //Sorted descendingly, first events are last here.
    protected List<Driver> activeDrivers = new List<Driver>();
    protected float bpm;
    protected string songPath;
    protected AudioClip clip;
    protected AudioSource source;

    protected bool playing = false;
    protected bool readyToPlay = false; //if all lists (drivers) are in initial state. You also need to check that clip is != null! 

    public MapManager(string filename, MonoBehaviour coroutineExecuter, AudioSource source)
    {
        this.source = source;

        //TODO: Argument: Filename? Stream? Decoded JSON? [could do this ourselves as well tbh]
        string entireFileAsString = System.IO.File.ReadAllText(filename); //Once I have 1000 blocks per map and 100.000 light events, I should switch to a 3rd party solution nevertheless, this is really just for sketching and simple songs
        MapWrapper map = JsonUtility.FromJson<MapWrapper>(entireFileAsString);

        songPath = map.songFilename; //TODO: Make this relative to where the level itself sits. 
        coroutineExecuter.StartCoroutine(LoadSongCoroutine());

        bpm = map.bpm;

        allWrappers = new MoveWrapper[map.moves.Length];
        int i = 0;
        foreach (MapWrapper.SingleMoveWrapper w in map.moves)
        {
            Type type;
            if (!MoveWrapper.typeDictionary.TryGetValue(w.type, out type)) //"Good" data management here, we just know that there is some public dict hangin round
            {
                Debug.Log("Bro what did u do? Could not find move " + w.type + ". Ignoring it. ");
                continue;
            }

            allWrappers[i++] = (MoveWrapper)JsonUtility.FromJson(w.json, type); 
        }

        InitializeDrivers();
    }

    protected void InitializeDrivers()
    {
        drivers.Clear();
        drivers.Capacity = allWrappers.Length;
        foreach (MoveWrapper w in allWrappers)
        {
            Driver driver = w.CreateDriver();

            drivers.Add(driver);
            driver.Initialize(60f / bpm);
        }

        //sort by firstTimestamp (but so that the first elements stand last - lesser reordering!)
        drivers = drivers.OrderByDescending(driver => driver.GetInstantiationPrewarmTime()).ToList();
        //ich bin auch mittlerweile soweit, dass ich gar nicht mehr richtig weiß, dass man in Java die { in derselben Zeile hat. Und dass man Methoden klein schreibt. 

        waitingDrivers.Clear();
        waitingDrivers.AddRange(drivers);
        activeDrivers.Clear();

        readyToPlay = true; 
    }

    //https://stackoverflow.com/questions/30852691/loading-mp3-files-at-runtime-in-unity bros u awesome ty <3
    protected IEnumerator LoadSongCoroutine()
    {
        string url = string.Format("file:///{0}", songPath); //Bugfix: On windows, you need three ///
        WWW www = new WWW(url);
        yield return www;

        clip = www.GetAudioClip(false, false);
        readyToPlay = true; 
    }

    public bool HasLoaded()
    {
        //The loading of the moves happens (on the main thread) in the constructor, loading the song is the only thing that remains. 
        return clip != null;
    }

    public void Update()
    {
        if (playing)
        {
            float timestamp = source.time;
            while (waitingDrivers.Count > 0 && waitingDrivers[waitingDrivers.Count - 1].InstantiateIfTime(timestamp))
            {
                int index = waitingDrivers.Count - 1;
                activeDrivers.Add(waitingDrivers[index]);
                waitingDrivers.RemoveAt(index);
            }
            int i = 0;
            while (i < activeDrivers.Count)
            {
                if (activeDrivers[i].Update(timestamp))
                {
                    activeDrivers.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    //Ends playback and deletes all moves. 
    //If you want to play the map again from start, you may directly call Reset() (which also calls this but then also recreates the initial state)
    public void CleanUp()
    {
        foreach (Driver d in activeDrivers)
        {
            d.CleanUp();
        }
        source.Stop();

        //This also happens in other places
        drivers.Clear();
        activeDrivers.Clear();
        waitingDrivers.Clear();

        playing = false;
        readyToPlay = false;
    }

    //Stops playback and removes all moves with a CleanUp(), and then recreates the initial state so that you can Play() again from start. 
    public void Reset()
    {
        CleanUp();
        InitializeDrivers();
    }

    //Call this when HasLoaded(), and then call Update() every frame. The map is playing now. Stop playback with CleanUp() or Reset(). 
    public void Play()
    {
        if (clip && readyToPlay)
        {
            source.clip = clip;
            //TODO: Offset feature. By now, it just starts on the beat and you have to modify the song
            source.Play();

            readyToPlay = false;
            playing = true;
        } else
        {
            Debug.Log("[MapManager] Play() called, but was not ready to play");
        }

    }
}
