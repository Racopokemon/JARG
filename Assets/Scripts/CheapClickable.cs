using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheapClickable : MonoBehaviour
{
    public UnityEvent onClick;

    public void Click()
    {
        onClick.Invoke();
    }

    public void Hover()
    {
        transform.localScale = Vector3.one * 1.1f;
    }

    public void Unhover()
    {
        transform.localScale = Vector3.one;
    }
}
