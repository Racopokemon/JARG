using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/*
 * Actually this is rather the DriverManager. Creates them, updates them, keeps track of them. 
 * 
 */
public class MapManager
{
    protected List<Driver> drivers = new List<Driver>();
    protected List<Driver> waitingDrivers = new List<Driver>(); //Sorted descending, first events are last here.
    protected List<Driver> activeDrivers = new List<Driver>();
    protected float bpm;
    protected string songPath;
    protected AudioClip clip;
    protected AudioSource source;

    public MapManager(string filename, MonoBehaviour coroutineExecuter)
    {
        //TODO: Argument: Filename? Stream? Decoded JSON? [could do this ourselves as well tbh]
        string entireFileAsString = System.IO.File.ReadAllText(filename); //Once I have 1000 blocks per map and 100.000 light events, I should switch to a 3rd party solution nevertheless, this is really just for sketching and simple songs
        MapWrapper map = JsonUtility.FromJson<MapWrapper>(entireFileAsString);

        songPath = "./" + map.songFilename;
        coroutineExecuter.StartCoroutine(LoadSongCoroutine());

        bpm = map.bpm;

        foreach (MapWrapper.SingleMoveWrapper w in map.moves)
        {
            ConstructorInfo constructor;
            if (!MoveWrapper.typeConstructorDictionary.TryGetValue(w.type, out constructor)) //"Good" data management here, we just know that there is some public dict hangin round
            {
                Debug.Log("Bro what did u do? Could not find move " + w.type + ". Ignoring it. ");
                continue;
            }
            UnityEngine.Object moveWrapper = (UnityEngine.Object) constructor.Invoke(new object[] { });

            JsonUtility.FromJsonOverwrite(w.json, moveWrapper);

            Driver driver = ((MoveWrapper)moveWrapper).CreateDriver();

            drivers.Add(driver);
            driver.Initialize(60f / bpm);
        }

        //sort by firstTimestamp (but so that the first elements stand last - lesser reordering!)
        drivers.OrderByDescending( driver => driver.GetInstantiationPrewarmTime()); //schon bisschen geil ey. vielleicht werde ich doch noch c# fan. 
        //ich bin auch mittlerweile soweit, dass ich gar nicht mehr richtig weiß, dass man in Java die { in derselben Zeile hat. Und dass man Methoden klein schreibt. 
    }

    //https://stackoverflow.com/questions/30852691/loading-mp3-files-at-runtime-in-unity bros u awesome ty <3
    protected IEnumerator LoadSongCoroutine()
    {
        string url = string.Format("file://{0}", songPath);
        WWW www = new WWW(url);
        yield return www;

        clip = www.GetAudioClip(false, false);
    }

    public bool HasLoaded()
    {
        //The loading of the moves happens (on the main thread) in the constructor, loading the song is the only thing that remains. 
        return clip != null;
    }

    public void Update()
    {
        float timestamp = source.time;
        while (waitingDrivers[waitingDrivers.Count-1].InstantiateIfTime(timestamp))
        {
            int index = waitingDrivers.Count - 1;
            activeDrivers.Add(waitingDrivers[index]);
            waitingDrivers.RemoveAt(index);
        }
        int i = 0;
        while (i <= activeDrivers.Count)
        {
            if (activeDrivers[i].Update(timestamp))
            {
                activeDrivers.RemoveAt(i);
            } else
            {
                i++;
            }
        }
    }

    public void CleanUp()
    {
        foreach (Driver d in activeDrivers)
        {
            d.CleanUp();
        }
    }

    //Init call, adds audio source etc. Call this after construction as soon as HasLoaded(), and then call Update() every frame. 
    public void MountToUnity(AudioSource source)
    {
        source.clip = clip;
        source (offset must be calculated anyway)
        source.Play();
    }
}
