using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayer : MonoBehaviour
{
    protected enum State {
        off, waitingForLoad, finishedLoading, playing //we dont really need finishedLoading right now, instantly starts the map
    }

    public string filename = "level.json";
    public bool playOnStart = false;
    public AudioSource audioSource;

    protected MapManager manager;
    protected State state = State.off;
    // Start is called before the first frame update
    void Start()
    {
        if (playOnStart)
        {
            LoadLevel();
        }
    }

    // Update is called once per frame 
    void Update()
    {
        if (state == State.playing)
        {
            manager.Update();
        } else if (state == State.waitingForLoad)
        {
            if (manager.HasLoaded())
            {
                state = State.finishedLoading;
                StartLevel();
            }
        }
    }

    public void LoadLevel()
    {
        manager = new MapManager(filename, this);
        state = State.waitingForLoad;
    }

    public void StartLevel()
    {
        if (state != State.finishedLoading)
        {
            throw new System.Exception("Cannot start level, because the level has not loaded yet (or we are in a wrong state in another way)");
        }
        manager.MountToUnityAndStart(audioSource);
        state = State.playing;
    }
}
