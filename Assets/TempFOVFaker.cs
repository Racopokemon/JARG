using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempFOVFaker : MonoBehaviour
{
    public Transform target;

    public float seconds = 5f;
    public float start = 3f;
    public float end = 0.1f;

    private bool active = false;
    private float startTime, endTime; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            active = true;
            startTime = Time.time;
            endTime = Time.time + seconds;
        }
        if (active)
        {
            if (Time.time >= endTime)
            {
                active = false;
                target.localScale = Vector3.one;
            } else
            {
                float i = Mathf.InverseLerp(startTime, endTime, Time.time);
                float factor = Mathf.Lerp(start, end, i);
                target.localScale = new Vector3(1, 1, factor);
            }
        }
    }
}
