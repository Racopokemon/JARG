using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Threading;

using TobiasErichsen.teVirtualMIDI;

public class MIDIListener : MonoBehaviour
{
    public static int LIGHT_CHANNEL_COUNT = 12;

    protected Thread midiThread;

    protected TeVirtualMIDI port;

    public static byte[] lightStates = new byte[LIGHT_CHANNEL_COUNT]; //yee well this is so beautifully cheap

    // Start is called before the first frame update
    void Start()
    {
        port = new TeVirtualMIDI( "RythmGame lightshow" );
        
        midiThread = new Thread(MIDIUpdateLoop);
        midiThread.Start();
    }

    void MIDIUpdateLoop()
    {
        byte[] command;

        try
        {
            while (true)
            {
                command = port.getCommand();
                //port.sendCommand(command);
                //Debug.Log("command: " + command);
                uint channelErasingBitmask = 0b_1111_0000;
                uint messageType = command[0] & channelErasingBitmask;
                if (messageType == 144) //i didnt even try to calculate the bits (i actually failed) i just looked this up in debug and its 1:54 bruh time for bed -.-
                {
                    // Note on
                    lightStates[command[1] % LIGHT_CHANNEL_COUNT] = command[2];
                } else if (messageType == 128)
                {
                    // note off
                    lightStates[command[1] % LIGHT_CHANNEL_COUNT] = 0;
                }
                //1:41 and im hacking this stuff together here like nothin else
            }
        }
        catch (Exception ex)
        {
            Debug.Log("MIDI thread aborting: " + ex.Message);
        }

    }

    private void OnDestroy()
    {
        port.shutdown();
        midiThread.Join();
    }
}
//Okay fucking big shoutout to the guy who created this library. I mean, this is SO simple to use. The demo code did all I needed, I literally needed half an hour to get this fucking done