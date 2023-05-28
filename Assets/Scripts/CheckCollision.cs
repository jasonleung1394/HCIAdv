using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CheckCollision : MonoBehaviour
{
    private GameObject mainScript;
    private CopyAvatarMovement copyAvatarMovement;

    private LerpToInitialPose lerpToInitialPose;

    private List<Collider> collidersList = new List<Collider>();
    // Start is called before the first frame update
    void Start()
    {
        mainScript = GameObject.Find("MainScript");
        copyAvatarMovement = mainScript.GetComponent<CopyAvatarMovement>();
        lerpToInitialPose = mainScript.GetComponent<LerpToInitialPose>();
    }

    // Update is called once per frame
    void Update()
    {
        //         LerpToInitialPose lerpToInitialPose = mainScript.GetComponent<LerpToInitialPose>();
        // lerpToInitialPose.Lerp_Index = 0;
        // copyAvatarMovement.angleStatusIndexs.ToList().ForEach(element => Debug.Log($"==>{element}"));
        // Debug.Log(collidersList.Count == 0);
        angleViolation = copyAvatarMovement.angleStatusIndexs.All(element => element == 0);

        Debug.Log(lerpToInitialPose.Lerp_Index);
        if (collidersList.Count == 0 && angleViolation && lerpToInitialPose.Lerp_Index != 1)
        {
            lerpToInitialPose.Lerp_Index = 0;
        }
    }
    private bool angleViolation;


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "robot_mimic")
        {
            collidersList.Remove(other);
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        lerpToInitialPose.Lerp_Index = 2;

        if (other.gameObject.tag != "robot_mimic")
        {
            collidersList.Remove(other);
        }
        else if (other.gameObject.tag == "robot_mimic")
        {
            collidersList.Add(other);
        }

    }
}
