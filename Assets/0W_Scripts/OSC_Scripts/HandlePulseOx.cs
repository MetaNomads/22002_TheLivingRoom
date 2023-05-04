using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandlePulseOx : MonoBehaviour
{

    public OSC osc;
    // Start is called before the first frame update
    void Start()
    {
        osc.SetAddressHandler( "/currBPM" , OnReceiveBPM );
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnReceiveBPM(OscMessage message) {
        float x = message.GetFloat(0);
        Debug.Log("message recieved: "+ x);
    }
}
