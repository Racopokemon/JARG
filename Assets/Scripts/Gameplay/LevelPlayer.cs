using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPlayer : MonoBehaviour
{
    protected enum State {
        off, waitingForLoad, readyToPlay, playing, ended //we dont really need readyToPlay right now, instantly starts the map
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
                state = State.readyToPlay;
                StartLevel();
            }
        }
    }

    public void LoadLevel()
    {
        manager = new MapManager(filename, this, audioSource);
        state = State.waitingForLoad;
    }

    public void StartLevel()
    {
        if (state != State.readyToPlay)
        {
            throw new System.Exception("Cannot start level, because the level has not loaded yet (or we are in a wrong state in another way)");
        }
        manager.Play();
        state = State.playing;
    }

    public void EndLevel()
    {
        if (state == State.readyToPlay || state == State.playing)
        {
            manager.CleanUp();
            state = State.ended;
        }
    }

    public void ResetLevel()
    {
        if (state == State.off || state == State.waitingForLoad)
        {
            throw new System.Exception("Can only reset level when it has finished loading. ");
        }
        if (state == State.playing || state == State.ended)
        {
            manager.Reset();
            state = State.readyToPlay;
        }
    }
}
