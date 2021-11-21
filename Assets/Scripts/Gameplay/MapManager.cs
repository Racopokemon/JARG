using System.Collections;
using System.Collections.Generic;

/*
 * Actually this is rather the DriverManager. Creates them, updates them, keeps track of them. 
 * 
 */
public class MapManager
{
    public List<Driver> drivers = new List<Driver>();
    public List<Driver> waitingDrivers = new List<Driver>(); //Sorted descending, first events are last here.
    public List<Driver> activeDrivers = new List<Driver>();
    public MapManager()
    {
        //TODO: Argument: Filename? Stream? Decoded JSON? [could do this ourselves as well tbh]

        //Load song and init [...]

        //Iterate over all JSON driver entries,
        //create new Drivers,
        //store em all in a drivers list,
        //sort by firstTimestamp (but so that the first elements stand last - lesser reordering!)
    }

    public void Update(float timestamp)
    {
        while (waitingDrivers[waitingDrivers.Count-1].InstantiateIfTime(timestamp))
        {
            int index = waitingDrivers.Count - 1;
            activeDrivers.Add(waitingDrivers[index]);
            waitingDrivers.RemoveAt(index);
        }
        int i = 0;
        while (i <= activeDrivers.Count)
        {
            if (activeDrivers[i].Update(timestamp))
            {
                activeDrivers.RemoveAt(i);
            } else
            {
                i++;
            }
        }
    }

    public void CleanUp()
    {
        foreach (Driver d in activeDrivers)
        {
            d.CleanUp();
        }
    }
}
