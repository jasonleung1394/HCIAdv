using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{
    private GameObject mainScript;
    // Start is called before the first frame update
    void Start()
    {
        mainScript = GameObject.Find("MainScript");
    }

    // Update is called once per frame
    void Update()
    {
        //         LerpToInitialPose lerpToInitialPose = mainScript.GetComponent<LerpToInitialPose>();
        // lerpToInitialPose.Lerp_Index = 0;

    }


    private void OnTriggerExit(Collider other)
    {
        LerpToInitialPose lerpToInitialPose = mainScript.GetComponent<LerpToInitialPose>();
        lerpToInitialPose.Lerp_Index = 0;
    }


    private void OnTriggerStay(Collider other)
    {
        LerpToInitialPose lerpToInitialPose = mainScript.GetComponent<LerpToInitialPose>();
        lerpToInitialPose.Lerp_Index = 2;
        // Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag != "robot_mimic")
        {
            lerpToInitialPose.Lerp_Index = 0;

        }
    }
}
