using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKneeFlasher : MonoBehaviour
{
    public Material flashMaterial;
    public Transform observationTarget;

    public List<MeshRenderer> flashTargets;

    public float minTravelDistance = 0.005f, fullTravelDistance = 0.02f;
    public float timeout = 0.4f;
    public float impactDistance = 0.033f;

    protected List<float> heights = new List<float>();
    float lastAvg;
    bool currentlyRising;
    float lastTopValue;
    float lastFlashTime;

    Color flashColor;

    float intensity = 0;

    // Start is called before the first frame update
    void Start()
    {
        //heights.Add(0);
        //heights.Add(0);
        heights.Add(0);
        heights.Add(0);
        heights.Add(0); // queue length

        flashColor = flashMaterial.GetColor("_EmissionColor");
    }

    // Update is called once per frame
    void Update()
    {
        heights.Add(observationTarget.position.y);
        heights.RemoveAt(0);

        float avg = 0;
        foreach (float f in heights)
        {
            avg += f;
        }
        avg /= heights.Count;

        if (currentlyRising)
        {
            if (avg < lastAvg)
            {
                currentlyRising = false;
                //local maximum, veeeeeery cheap
                lastTopValue = Mathf.Max(avg, lastTopValue);
            }
        } else
        {
            if (avg > lastAvg)
            {
                currentlyRising = true;
                //local minimum, veeeeeery cheap
                //flash the lights
                if (lastFlashTime + timeout < Time.time) //if the flash timeout is over
                {
                    bool flashed; 
                    float difference = lastTopValue - avg;
                    lastTopValue = -10000;
                    if (difference >= impactDistance)
                    {
                        intensity = 10;
                        flashed = true;
                    } else
                    {
                        float newIntensity = Mathf.Clamp01(Mathf.InverseLerp(minTravelDistance, fullTravelDistance, difference)) * 2.5f;
                        intensity = Mathf.Max(intensity, newIntensity);
                        flashed = intensity == newIntensity;
                    }
                    if (flashed)
                    {
                        //Our flash was strong enough to actually have a visual impact
                        lastFlashTime = Time.time;
                    }
                }
            }
        }

        lastAvg = avg;

        intensity *= 0.9f;
        intensity = Mathf.Max(0, intensity);
        Color newColor = Color.Lerp(Color.black, flashColor, intensity);
        foreach (MeshRenderer r in flashTargets)
        {
            r.material.SetColor("_EmissionColor", newColor);
        }
    }
}
