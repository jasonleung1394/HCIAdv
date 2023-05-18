using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checkExitCollision : MonoBehaviour
{
    private GameObject mainScript;
    private Collider[] colliders;
    // Start is called before the first frame update
    void Start()
    {
        mainScript = GameObject.Find("MainScript");
    }

    private void OnCollisionEnter(Collision other)
    {
        LerpToInitialPose lerpToInitialPose = mainScript.GetComponent<LerpToInitialPose>();
        lerpToInitialPose.Lerp_Index = 0;
        Debug.Log(other);
    }
}
