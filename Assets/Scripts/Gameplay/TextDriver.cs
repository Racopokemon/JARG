using UnityEngine;
using TMPro;

public class TextDriver : Driver
{
    protected TextWrapper w;
    protected GameObject g;

    public TextDriver(TextWrapper wrapper)
    {
        w = wrapper;
    }

    protected override void InitializeTimestamps(float beatLength)
    {
        timestamps = new float[] { beatLength*w.start, beatLength*w.end };
    }

    protected override void PrepareForSpawn()
    {
        g = (GameObject) Object.Instantiate(Resources.Load("TextMove", typeof(GameObject)));
        TextMeshPro tmp = g.GetComponentInChildren<TextMeshPro>();
        tmp.text = w.text;
        g.SetActive(false);
    }

    protected override void NextPhase()
    {
        g.SetActive(GetPhase() == 0);
    }

    protected override void UpdatePhase(float a) { }

    protected override void DoCleanUp()
    {
        Object.Destroy(g);
    }
}