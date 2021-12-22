using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmDriver : Driver
{
    protected const float CIRCLE_START_SCALE = 1.3f;
    protected const float MAIN_ALPHA = 0.7f;

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
            w.holdEnd //no despawn effect yet. Maybe later. 
        };
        for (int i = 0; i < timestamps.Length; i++) timestamps[i] *= beatLength;
    }

    protected override void PrepareForSpawn()
    {
        g = (GameObject)Object.Instantiate(Resources.Load("PalmMove", typeof(GameObject)));
        m = g.GetComponent<PalmGOMapping>();
        g.transform.position = new Vector3(w.x, w.y, w.z);
        g.transform.rotation = Quaternion.Euler(0, 0, w.angle);
        g.SetActive(false);
    }

    protected override void NextPhase()
    {
        int phase = GetPhase();
        if (phase == 0)
        {
            g.SetActive(true);
            m.fullCircle.SetActive(false);
        }
        else if (phase == 1)
        {
            m.fullCircle.SetActive(true);
        }
        else if (phase == 2)
        {
            ;
        } else
        {
            g.SetActive(false);
        }
    }
    protected override void UpdatePhase(float a, bool forCompletionOnly)
    {
        int phase = GetPhase();
        if (phase == 0)
        {
            m.ring.transform.localScale = Vector3.one * Mathf.Lerp(CIRCLE_START_SCALE, 1, a);
            SetAlpha(m.ring.GetComponent<MeshRenderer>(), Mathf.Lerp(0, MAIN_ALPHA, a));
            SetAlpha(m.mainCircle.GetComponent<MeshRenderer>(), Mathf.Lerp(0, MAIN_ALPHA, a*a));
        } else if (phase == 1)
        {
            SetAlpha(m.fullCircle.GetComponent<MeshRenderer>(), Mathf.Lerp(0, 1, a));
            m.fullCircle.transform.localScale = Vector3.one * a;
        } else if (phase == 2)
        {
            ;
        }
    }

    protected override void DoCleanUp()
    {
        GameObject.Destroy(g);
    }
}
