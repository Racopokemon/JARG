using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CheapClickable : MonoBehaviour
{
    public UnityEvent onClick;
    protected Vector3 idleScale;
    public void Start()
    {
        idleScale = transform.localScale;
    }

    public void Click()
    {
        onClick.Invoke();
    }

    public void Hover()
    {
        transform.localScale = idleScale * 1.1f;
    }

    public void Unhover()
    {
        transform.localScale = idleScale;
    }
}
