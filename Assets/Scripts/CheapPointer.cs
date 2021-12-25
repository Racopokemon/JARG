using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheapPointer : MonoBehaviour
{
    protected CheapClickable hover;


    // Update is called once per frame
    void Update()
    {
        RaycastHit info;
        Physics.Raycast(transform.position, transform.forward, out info);
        CheapClickable click = info.transform.gameObject.GetComponent<CheapClickable>();
        if (click != hover)
        {
            if (hover)
            {
                hover.Unhover();
            }
            if (click)
            {
                click.Hover();
            }
        }
        if (SteamVR_Actions. && click) // Right control
        {
            click.Click();
        }
    }
}
