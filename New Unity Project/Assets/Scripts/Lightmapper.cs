using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightmapper : MonoBehaviour
{
    public List<GameObject> lightsWhatever;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < lightsWhatever.Count; i++)
        {
            lightsWhatever[i].SetActive(MIDIListener.lightStates[i] != 0);
        }
    }
}
