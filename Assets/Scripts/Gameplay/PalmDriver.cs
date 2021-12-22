using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmDriver : Driver
{
    protected const float CIRCLE_START_SCALE = 1.25f;
    protected const float MAIN_ALPHA = 0.07f;
    protected const float TOUCH_ALPHA = 0.3f;
    protected const float PUSH_DISTANCE = 0.03f;

    protected PalmWrapper w;
    protected GameObject g;
    protected PalmGOMapping m;

    public PalmDriver(PalmWrapper wrapper)
    {
        w = wrapper; 
    }

    protected override void InitializeTimestamps(float beatLength)
    {
        timestamps = new float[] {
            w.moveStart - 2, //spawn time
            w.moveStart,
            w.moveEnd,
            w.holdEnd,
            w.holdEnd + 2
        };
        for (int i = 0; i < timestamps.Length; i++) timestamps[i] *= beatLength; //TODO maybe the spawn and despawn times should be not beat-dependent. 
    }

    protected override void PrepareForSpawn()
    {
        g = (GameObject)Object.Instantiate(Resources.Load("PalmMove", typeof(GameObject)));
        m = g.GetComponent<PalmGOMapping>();
        g.transform.position = new Vector3(w.x, w.y, w.z+0.5f);
        g.transform.rotation = Quaternion.Euler(0, 0, w.angle);
        g.SetActive(false);
    }

    protected override void NextPhase()
    {
        int phase = GetPhase();
        if (phase == 0) //start spawn
        {
            g.SetActive(true);
            m.fullCircle.SetActive(false);
        }
        else if (phase == 1) //start push
        {
            m.fullCircle.SetActive(true);
            SetAlpha(m.fullCircle.GetComponent<MeshRenderer>(), TOUCH_ALPHA);
        }
        else if (phase == 2) //start hold
        {
            ;
        }
        else if (phase == 3) //start fade out
        {
            m.fullCircle.SetActive(false);
            m.mainCircle.SetActive(false);
        } else //delete
        {
            g.SetActive(false);
        }
    }
    protected override void UpdatePhase(float a, bool forCompletionOnly)
    {
        int phase = GetPhase();
        if (phase == 0) //spawn
        {
            m.ring.transform.localScale = Vector3.one * Mathf.Lerp(CIRCLE_START_SCALE, 1, a);
            SetAlpha(m.ring.GetComponent<MeshRenderer>(), Mathf.Lerp(0, MAIN_ALPHA, a));
            SetAlpha(m.mainCircle.GetComponent<MeshRenderer>(), Mathf.Lerp(0, MAIN_ALPHA, a*a));
        } else if (phase == 1) //push
        {
            //SetAlpha(m.fullCircle.GetComponent<MeshRenderer>(), Mathf.Lerp(0, TOUCH_ALPHA, a));
            SetAlpha(m.mainCircle.GetComponent<MeshRenderer>(), Mathf.Lerp(TOUCH_ALPHA, 0, a));

            m.fullCircle.transform.localScale = Vector3.one * Mathf.Lerp(0.6f, 1, a); // from 0 it feels crappy somehow

            m.fullCircle.transform.localPosition = Vector3.back    * Mathf.Lerp(PUSH_DISTANCE*0.5f, 0, a);
            m.mainCircle.transform.localPosition = Vector3.forward * Mathf.Lerp(0, PUSH_DISTANCE, a);
        } else if (phase == 2) //hold
        {
            ;
        } else if (phase == 3) //fade out
        {
            m.ring.transform.localScale = Vector3.one * Mathf.Lerp(1, CIRCLE_START_SCALE, a);
            SetAlpha(m.ring.GetComponent<MeshRenderer>(), Mathf.Lerp(MAIN_ALPHA, 0, a));
        }
    }

    protected override void DoCleanUp()
    {
        GameObject.Destroy(g);
    }
}
