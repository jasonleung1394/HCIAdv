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

    }


    private void OnTriggerEnter(Collider other)
    {
        LerpToInitialPose lerpToInitialPose = mainScript.GetComponent<LerpToInitialPose>();
        lerpToInitialPose.Lerp_Index = 1;
    }
}
