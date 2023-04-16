using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpToInitialPose : MonoBehaviour
{
    private int lerp_Index = 0;
    public int Lerp_Index { get { return lerp_Index; } set { lerp_Index = value; } }

    private Transform arm_transform { get; set; }
    private Transform foreArm_transform { get; set; }
    private Transform hand_transform { get; set; }

    public float lerpSpeed = 1f;

    void Start()
    {
        arm_transform = GameObject.Find("Right Arm").transform;
        foreArm_transform = GameObject.Find("Right Forearm").transform;
        hand_transform = GameObject.Find("Right Hand").transform;
    }
    private Quaternion initial_Arm = Quaternion.Euler((1.59695f / 2) * Mathf.Rad2Deg, 0, 0);

    private Quaternion initial_ForeArm = Quaternion.Euler(0, 0, (-1.59695f / 2) * Mathf.Rad2Deg);

    private Quaternion initial_Hand = Quaternion.Euler(0, 0 * Mathf.Rad2Deg, 0);
    void Update()
    {

        if (Lerp_Index == 1)
        {
            arm_transform.localRotation = Quaternion.Lerp(arm_transform.localRotation, initial_Arm, lerpSpeed * Time.deltaTime);
            foreArm_transform.localRotation = Quaternion.Lerp(foreArm_transform.localRotation, initial_ForeArm, lerpSpeed * Time.deltaTime);
            hand_transform.localRotation = Quaternion.Lerp(hand_transform.localRotation, initial_Hand, lerpSpeed * Time.deltaTime);
        }

        if (arm_transform.localRotation == initial_Arm && foreArm_transform.localRotation == initial_ForeArm && hand_transform.localRotation == initial_Hand)
        {
            lerp_Index = 0;
        }
        else
        {
            arm_transform = GameObject.Find("Right Arm").transform;
            foreArm_transform = GameObject.Find("Right Forearm").transform;
            hand_transform = GameObject.Find("Right Hand").transform;
        }
    }
}
