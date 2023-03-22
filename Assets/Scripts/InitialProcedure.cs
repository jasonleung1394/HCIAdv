using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Tf2;

public class InitialProcedure : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // establish Ros Connection
        ROSConnection.GetOrCreateInstance().Subscribe<TFMessageMsg>("tf",ShowTF);
        // establish remote play

        // connect with optitrack

        // start the following sequence
    }
    void ShowTF(TFMessageMsg tFMessageMsg){
        // Debug.Log(tFMessageMsg);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
