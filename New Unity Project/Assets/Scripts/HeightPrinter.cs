using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using System.IO;
public class HeightPrinter : MonoBehaviour
{
    public Transform targetToLogHeightFrom;

    List<String> recordedData = new List<String>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        recordedData.Add(targetToLogHeightFrom.position.y+ "; "+Time.timeSinceLevelLoad);
    }

    private void OnDestroy()
    {
        int index = 0;
        String filename;
        do {
            filename = "./heightrec" + index++ + ".csv";
        } while (File.Exists(filename));

        Debug.Log("Writing to file " + filename);

        File.WriteAllLines(filename, recordedData);
    }
}
